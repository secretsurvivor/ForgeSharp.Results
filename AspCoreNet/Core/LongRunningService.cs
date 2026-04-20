using ForgeSharp.Results.AspNetCore.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Threading.Channels;

namespace ForgeSharp.Results.AspNetCore.Core;

public interface IJobRegistry
{
    bool TryGetState(string jobToken, [MaybeNullWhen(false)] out JobState state);
    bool TryCancel(string jobToken);
}

public sealed class LongRunningService(
    Channel<LongRunningRequest> requestChannel,
    Channel<JobProgressUpdate> progressChannel,
    IServiceProvider provider,
    ILogger<LongRunningService> logger) : IHostedService, IDisposable, IJobRegistry
{
    private CancellationTokenSource _tokenSource = new CancellationTokenSource();
    internal readonly ConcurrentDictionary<string, RegisteredJob> _runningJobs = [];
    private Task _requestProcessor = Task.CompletedTask;
    private Task _progressUpdater = Task.CompletedTask;

    private async Task ProgressUpdater()
    {
        while (await progressChannel.Reader.WaitToReadAsync(_tokenSource.Token))
        {
            var update = await progressChannel.Reader.ReadAsync(_tokenSource.Token);

            if (_runningJobs.TryGetValue(update.JobToken, out var registeredJob))
            {
                registeredJob.UpdateState(state => state with { Message = update.Message });
            }
        }
    }

    private async Task RequestProcessor()
    {
        while (await requestChannel.Reader.WaitToReadAsync(_tokenSource.Token))
        {
            while (requestChannel.Reader.TryRead(out var request))
            {
                var cancellationSource = CancellationTokenSource.CreateLinkedTokenSource(_tokenSource.Token);

                if (!_runningJobs.TryAdd(request.JobToken, new RegisteredJob
                {
                    _state = new JobState { JobToken = request.JobToken, CurrentState = JobState.State.Running },
                    CancellationTokenSource = cancellationSource,
                    RunningTask = Task.Run(async () => LongRunningTask(request, cancellationSource.Token), cancellationSource.Token),
                }))
                {
                    logger.LogError("A long running job with the {JobToken} was already registered", request.JobToken);
                    continue;
                }
            }
        }
    }

    private async Task LongRunningTask(LongRunningRequest request, CancellationToken cancellationToken)
    {
        await using var scope = provider.CreateAsyncScope();
        request.CapturedState.Restore(scope.ServiceProvider);

        var invoker = request.InvokerFactory.Invoke(scope.ServiceProvider);
        bool isSuccess;
        IResult result;

        try
        {
            (isSuccess, result) = await invoker.ExecuteAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            isSuccess = false;
            result = new ResultEndpoint(Result.Fail(ex));
        }

        if (_runningJobs.TryGetValue(request.JobToken, out var registeredJob))
        {
            if (cancellationToken.IsCancellationRequested)
            {
                registeredJob.UpdateState(static state => state with { CurrentState = JobState.State.Canceled });
            }
            else
            {
                registeredJob.UpdateState(state => state with { CurrentState = isSuccess ? JobState.State.Completed : JobState.State.Failed, Result = result });
            }
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _progressUpdater = Task.Run(ProgressUpdater, _tokenSource.Token);
        _requestProcessor = Task.Run(RequestProcessor, _tokenSource.Token);

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _tokenSource.Cancel();

        try
        {
            await _requestProcessor;
            await _progressUpdater;
            await Task.WhenAll(_runningJobs.Values.Select(x => x.RunningTask));
        }
        catch
        {

        }
    }

    public void Dispose()
    {
        _tokenSource.Dispose();

        foreach (var job in _runningJobs.Values)
        {
            job.Dispose();
        }
    }

    public bool TryGetState(string jobToken, [MaybeNullWhen(false)] out JobState state)
    {
        if (_runningJobs.TryGetValue(jobToken, out var registeredJob))
        {
            state = registeredJob._state;
            return true;
        }

        state = default;
        return false;
    }

    public bool TryCancel(string jobToken)
    {
        if (_runningJobs.TryGetValue(jobToken, out var registeredJob))
        {
            registeredJob.CancellationTokenSource.Cancel();
            return true;
        }

        return false;
    }

    internal class RegisteredJob : IDisposable
    {
        private readonly Lock _lock = new Lock();
        internal required volatile JobState _state;

        public required CancellationTokenSource CancellationTokenSource { get; init; }
        public required Task RunningTask { get; init; }

        internal void UpdateState(Func<JobState, JobState> update)
        {
            lock (_lock)
            {
                _state = update(_state);
            }
        }

        public void Dispose() => CancellationTokenSource.Dispose();
    }
}

public record LongRunningRequest
{
    public required string JobToken { get; init; }
    public required Func<IServiceProvider, IPipelineInvoker> InvokerFactory { get; init; }
    public required ICapturedState CapturedState { get; init; }
}

public record JobProgressUpdate
{
    public required string JobToken { get; init; }
    public required string Message { get; init; }
}

internal sealed class LongRunningResponse : IResult
{
    public required string JobToken { get; init; }

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = StatusCodes.Status200OK;

        string BuildUrl(string path) => UriHelper.BuildAbsolute(httpContext.Request.Scheme, httpContext.Request.Host, pathBase: httpContext.Request.PathBase, path: path);

        var envelope = new
        {
            Token = JobToken,
            Poll = BuildUrl($"/__longrunning/{JobToken}"),
            Stream = BuildUrl($"/__longrunning/{JobToken}/stream"),
        };

        await JsonSerializer.SerializeAsync(
            httpContext.Response.Body,
            envelope,
            envelope.GetType(),
            JsonSerializerOptions.Web);
    }
}
