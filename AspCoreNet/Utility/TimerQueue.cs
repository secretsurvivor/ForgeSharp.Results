using System.Diagnostics;

namespace ForgeSharp.Results.AspNetCore.Utility;

internal sealed class TimerQueue<T>(Func<T, CancellationToken, Task> timerEvent) : IAsyncDisposable
{
    public CancellationToken CancellationToken { private get; init; } = CancellationToken.None;
    private readonly PriorityQueue<T, DateTime> _timerQueue = new PriorityQueue<T, DateTime>();
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    private ActiveTimer? _activeTimer;
    private Task? _timerTask;
    private bool _disposed;

    public async ValueTask Add(TimeSpan timer, T value)
    {
        await _semaphore.WaitAsync();
        try
        {
            // Disposed check is inside the semaphore to close the race with DisposeAsync
            ObjectDisposedException.ThrowIf(_disposed, this);

            var targetTime = DateTime.UtcNow.Add(timer);
            _timerQueue.Enqueue(value, targetTime);

            if (_activeTimer is null)
            {
                _activeTimer = new ActiveTimer(CancellationTokenSource.CreateLinkedTokenSource(CancellationToken), (targetTime, value));
                _timerTask = Task.Run(RunTimer);
                return;
            }

            var (currentTimer, _, _) = _activeTimer;
            if (targetTime < currentTimer)
            {
                _activeTimer.Dispose();
                _activeTimer = new ActiveTimer(CancellationTokenSource.CreateLinkedTokenSource(CancellationToken), (targetTime, value));
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task RunTimer()
    {
        while (true)
        {
            // Snapshot _activeTimer under the semaphore to avoid a torn read-then-deconstruct
            await _semaphore.WaitAsync();
            var activeTimer = _activeTimer;
            _semaphore.Release();

            if (activeTimer is null || CancellationToken.IsCancellationRequested)
            {
                break;
            }

            var (targetTime, targetValue, cancellationToken) = activeTimer;

            if (targetTime > DateTime.UtcNow)
            {
                try
                {
                    await Task.Delay(targetTime - DateTime.UtcNow, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    // A nearer item was enqueued or we are being disposed — re-evaluate from top
                    continue;
                }
            }

            try
            {
                await timerEvent(targetValue, CancellationToken);
            }
            catch (Exception ex)
            {
                Debug.Fail($"{nameof(timerEvent)} threw an unhandled exception: {ex}");
            }

            await _semaphore.WaitAsync();
            try
            {
                // Guard against firing again after DisposeAsync cleared the queue
                if (_disposed)
                {
                    break;
                }

                _timerQueue.Dequeue();
                _activeTimer!.Dispose();
                _activeTimer = _timerQueue.TryPeek(out var nextValue, out var nextTime)
                    ? new ActiveTimer(CancellationTokenSource.CreateLinkedTokenSource(CancellationToken), (nextTime, nextValue))
                    : null;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        await _semaphore.WaitAsync();
        _timerTask = null;
        _semaphore.Release();
    }

    public async ValueTask DisposeAsync()
    {
        await _semaphore.WaitAsync();

        _disposed = true;
        _timerQueue.Clear();
        _activeTimer?.Dispose();

        var task = _timerTask;

        _semaphore.Release();

        if (task is not null)
        {
            await task;
        }
    }

    class ActiveTimer(CancellationTokenSource tokenSource, (DateTime time, T value) target) : IDisposable
    {
        public (DateTime time, T value) Target => target;

        public void Deconstruct(out DateTime targetTime, out T targetValue, out CancellationToken cancellationToken)
        {
            targetTime = target.time;
            targetValue = target.value;
            cancellationToken = tokenSource.Token;
        }

        public void Dispose()
        {
            if (!tokenSource.IsCancellationRequested)
            {
                tokenSource.Cancel();
            }

            tokenSource.Dispose();
        }
    }
}
