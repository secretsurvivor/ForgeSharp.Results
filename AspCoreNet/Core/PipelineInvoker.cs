using Microsoft.AspNetCore.Http;

namespace ForgeSharp.Results.AspNetCore.Core;

public interface IPipelineInvoker
{
    ValueTask<(bool isSuccess, IResult result)> ExecuteAsync(CancellationToken cancellationToken = default);
}

public class PipelineInvoker : IPipelineInvoker
{
    private readonly IPipeline _pipeline;

    private PipelineInvoker(IPipeline pipeline) => _pipeline = pipeline;

    public static IPipelineInvoker Wrap(IPipeline pipeline) => new PipelineInvoker(pipeline);
    public static IPipelineInvoker Wrap<T>(IPipeline<T> pipeline) => new PipelineInvoker<T>(pipeline);
    public static IPipelineInvoker Wrap<T, TError>(IPipeline<T, TError> pipeline) => new PipelineInvoker<T, TError>(pipeline);
    public static IPipelineInvoker Wrap(IAsyncPipeline pipeline) => new AsyncPipelineInvoker(pipeline);
    public static IPipelineInvoker Wrap<T>(IAsyncPipeline<T> pipeline) => new AsyncPipelineInvoker<T>(pipeline);
    public static IPipelineInvoker Wrap<T, TError>(IAsyncPipeline<T, TError> pipeline) => new AsyncPipelineInvoker<T, TError>(pipeline);

    public ValueTask<(bool isSuccess, IResult result)> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var result = _pipeline.Execute();
        return ValueTask.FromResult((result.IsSuccess, (IResult) new ResultEndpoint(result)));
    }
}

file class AsyncPipelineInvoker(IAsyncPipeline asyncPipeline) : IPipelineInvoker
{
    public async ValueTask<(bool isSuccess, IResult result)> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var result = await asyncPipeline.ExecuteAsync(cancellationToken);
        return (result.IsSuccess, new ResultEndpoint(result));
    }
}

file class PipelineInvoker<T>(IPipeline<T> pipeline) : IPipelineInvoker
{
    public ValueTask<(bool isSuccess, IResult result)> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var result = pipeline.Execute();
        return ValueTask.FromResult((result.IsSuccess, (IResult) new ResultEndpoint<T>(result)));
    }
}

file class AsyncPipelineInvoker<T>(IAsyncPipeline<T> pipeline) : IPipelineInvoker
{
    public async ValueTask<(bool isSuccess, IResult result)> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var result = await pipeline.ExecuteAsync(cancellationToken);
        return (result.IsSuccess, new ResultEndpoint<T>(result));
    }
}

file class PipelineInvoker<T, TError>(IPipeline<T, TError> pipeline) : IPipelineInvoker
{
    public ValueTask<(bool isSuccess, IResult result)> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var result = pipeline.Execute();
        return ValueTask.FromResult((result.IsSuccess, (IResult) new ResultEndpoint<T, TError>(result)));
    }
}

file class AsyncPipelineInvoker<T, TError>(IAsyncPipeline<T, TError> pipeline) : IPipelineInvoker
{
    public async ValueTask<(bool isSuccess, IResult result)> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var result = await pipeline.ExecuteAsync(cancellationToken);
        return (result.IsSuccess, new ResultEndpoint<T, TError>(result));
    }
}
