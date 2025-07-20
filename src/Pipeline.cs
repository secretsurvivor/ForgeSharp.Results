using System.Runtime.CompilerServices;

namespace ForgeSharp.Results;

/// <summary>
/// Represents a synchronous pipeline that produces a <see cref="Result"/>.
/// </summary>
public interface IPipeline
{
    /// <summary>
    /// Executes the pipeline and returns a <see cref="Result"/>.
    /// </summary>
    /// <returns>The result of the pipeline execution.</returns>
    public Result Execute();
}

/// <summary>
/// Represents an asynchronous pipeline that produces a <see cref="Result"/>.
/// </summary>
public interface IAsyncPipeline
{
    /// <summary>
    /// Executes the pipeline asynchronously and returns a <see cref="Result"/>.
    /// </summary>
    /// <returns>A task representing the asynchronous operation, with the result of the pipeline execution.</returns>
    public Task<Result> ExecuteAsync();
}

/// <summary>
/// Represents a synchronous pipeline that produces a <see cref="Result{T}"/>.
/// </summary>
/// <typeparam name="T">The type of the value produced by the pipeline.</typeparam>
public interface IPipeline<T>
{
    /// <summary>
    /// Executes the pipeline and returns a <see cref="Result{T}"/>.
    /// </summary>
    /// <returns>The result of the pipeline execution.</returns>
    public Result<T> Execute();
}

/// <summary>
/// Represents an asynchronous pipeline that produces a <see cref="Result{T}"/>.
/// </summary>
/// <typeparam name="T">The type of the value produced by the pipeline.</typeparam>
public interface IAsyncPipeline<T>
{
    /// <summary>
    /// Executes the pipeline asynchronously and returns a <see cref="Result{T}"/>.
    /// </summary>
    /// <returns>A task representing the asynchronous operation, with the result of the pipeline execution.</returns>
    public Task<Result<T>> ExecuteAsync();
}

/// <summary>
/// Provides a synchronous pipeline implementation.
/// </summary>
public readonly struct Pipeline : IPipeline
{
    internal readonly Func<Result> _pipeline;

    internal Pipeline(Func<Result> pipeline)
    {
        _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
    }

    /// <summary>
    /// Gets a successful starting <see cref="Result"/> for a pipeline.
    /// </summary>
    public static Result Start => Result.Ok();

    /// <summary>
    /// Creates a new <see cref="IPipeline"/> from the specified delegate.
    /// </summary>
    /// <param name="pipeline">The pipeline delegate.</param>
    /// <returns>An <see cref="IPipeline"/> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IPipeline Create(Func<Result> pipeline)
    {
        return new Pipeline(pipeline);
    }

    /// <summary>
    /// Creates a new <see cref="IAsyncPipeline"/> from the specified delegate.
    /// </summary>
    /// <param name="pipeline">The asynchronous pipeline delegate.</param>
    /// <returns>An <see cref="IAsyncPipeline"/> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IAsyncPipeline Create(Func<Task<Result>> pipeline)
    {
        return new AsyncPipeline(pipeline);
    }

    /// <summary>
    /// Creates a new <see cref="IPipeline{T}"/> from the specified delegate.
    /// </summary>
    /// <typeparam name="T">The type of the value produced by the pipeline.</typeparam>
    /// <param name="pipeline">The pipeline delegate.</param>
    /// <returns>An <see cref="IPipeline{T}"/> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IPipeline<T> Create<T>(Func<Result<T>> pipeline)
    {
        return new Pipeline<T>(pipeline);
    }

    /// <summary>
    /// Creates a new <see cref="IAsyncPipeline{T}"/> from the specified delegate.
    /// </summary>
    /// <typeparam name="T">The type of the value produced by the pipeline.</typeparam>
    /// <param name="pipeline">The asynchronous pipeline delegate.</param>
    /// <returns>An <see cref="IAsyncPipeline{T}"/> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IAsyncPipeline<T> Create<T>(Func<Task<Result<T>>> pipeline)
    {
        return new AsyncPipeline<T>(pipeline);
    }

    /// <summary>
    /// Executes the pipeline and returns a <see cref="Result"/>.
    /// </summary>
    /// <returns>The result of the pipeline execution.</returns>
    public Result Execute()
    {
        return _pipeline.Invoke();
    }
}

internal readonly struct AsyncPipeline : IAsyncPipeline
{
    internal readonly Func<Task<Result>> _pipeline;

    internal AsyncPipeline(Func<Task<Result>> pipeline)
    {
        _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
    }

    public Task<Result> ExecuteAsync()
    {
        return _pipeline.Invoke();
    }
}

internal readonly struct Pipeline<T> : IPipeline<T>
{
    internal readonly Func<Result<T>> _pipeline;

    internal Pipeline(Func<Result<T>> pipeline)
    {
        _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
    }

    public Result<T> Execute()
    {
        return _pipeline.Invoke();
    }
}

internal readonly struct AsyncPipeline<T> : IAsyncPipeline<T>
{
    internal readonly Func<Task<Result<T>>> _pipeline;

    internal AsyncPipeline(Func<Task<Result<T>>> pipeline)
    {
        _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
    }

    public Task<Result<T>> ExecuteAsync()
    {
        return _pipeline.Invoke();
    }
}

/// <summary>
/// Provides extension methods for working with pipelines.
/// </summary>
public static class PipelineExtensions
{
    /// <summary>
    /// Converts a <see cref="Func{Result}"/> to an <see cref="IPipeline"/>.
    /// </summary>
    /// <param name="pipeline">The pipeline delegate.</param>
    /// <returns>An <see cref="IPipeline"/> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IPipeline AsPipeline(this Func<Result> pipeline)
    {
        return Pipeline.Create(pipeline);
    }

    /// <summary>
    /// Converts a <see cref="Func{Task{Result}}"/> to an <see cref="IAsyncPipeline"/>.
    /// </summary>
    /// <param name="pipeline">The asynchronous pipeline delegate.</param>
    /// <returns>An <see cref="IAsyncPipeline"/> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IAsyncPipeline AsPipeline(this Func<Task<Result>> pipeline)
    {
        return Pipeline.Create(pipeline);
    }

    /// <summary>
    /// Converts a <see cref="Func{Result{T}}"/> to an <see cref="IPipeline{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value produced by the pipeline.</typeparam>
    /// <param name="pipeline">The pipeline delegate.</param>
    /// <returns>An <see cref="IPipeline{T}"/> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IPipeline<T> AsPipeline<T>(this Func<Result<T>> pipeline)
    {
        return Pipeline.Create(pipeline);
    }

    /// <summary>
    /// Converts a <see cref="Func{Task{Result{T}}}"/> to an <see cref="IAsyncPipeline{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value produced by the pipeline.</typeparam>
    /// <param name="pipeline">The asynchronous pipeline delegate.</param>
    /// <returns>An <see cref="IAsyncPipeline{T}"/> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IAsyncPipeline<T> AsPipeline<T>(this Func<Task<Result<T>>> pipeline)
    {
        return Pipeline.Create(pipeline);
    }

    /// <summary>
    /// Executes the primary pipeline, and if it fails, executes the backup pipeline.
    /// </summary>
    /// <param name="primary">The primary pipeline.</param>
    /// <param name="backup">The backup pipeline.</param>
    /// <returns>The result of the primary or backup pipeline.</returns>
    public static Result Fallback(this IPipeline primary, IPipeline backup)
    {
        var result = primary.Execute();

        if (!result.IsSuccess)
        {
            return backup.Execute();
        }

        return result;
    }

    /// <summary>
    /// Executes the primary asynchronous pipeline, and if it fails, executes the backup asynchronous pipeline.
    /// </summary>
    /// <param name="primary">The primary asynchronous pipeline.</param>
    /// <param name="backup">The backup asynchronous pipeline.</param>
    /// <returns>A task representing the asynchronous operation, with the result of the primary or backup pipeline.</returns>
    public static async Task<Result> FallbackAsync(this IAsyncPipeline primary, IAsyncPipeline backup)
    {
        var result = await primary.ExecuteAsync();

        if (!result.IsSuccess)
        {
            return await backup.ExecuteAsync();
        }

        return result;
    }

    /// <summary>
    /// Executes the primary pipeline, and if it fails, executes the backup pipeline.
    /// </summary>
    /// <typeparam name="T">The type of the value produced by the pipeline.</typeparam>
    /// <param name="primary">The primary pipeline.</param>
    /// <param name="backup">The backup pipeline.</param>
    /// <returns>The result of the primary or backup pipeline.</returns>
    public static Result<T> Fallback<T>(this IPipeline<T> primary, IPipeline<T> backup)
    {
        var result = primary.Execute();

        if (!result.IsSuccess)
        {
            return backup.Execute();
        }

        return result;
    }

    /// <summary>
    /// Executes the primary asynchronous pipeline, and if it fails, executes the backup asynchronous pipeline.
    /// </summary>
    /// <typeparam name="T">The type of the value produced by the pipeline.</typeparam>
    /// <param name="primary">The primary asynchronous pipeline.</param>
    /// <param name="backup">The backup asynchronous pipeline.</param>
    /// <returns>A task representing the asynchronous operation, with the result of the primary or backup pipeline.</returns>
    public static async Task<Result<T>> FallbackAsync<T>(this IAsyncPipeline<T> primary, IAsyncPipeline<T> backup)
    {
        var result = await primary.ExecuteAsync();

        if (!result.IsSuccess)
        {
            return await backup.ExecuteAsync();
        }

        return result;
    }
}
