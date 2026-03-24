using System.Runtime.CompilerServices;

namespace ForgeSharp.Results;

/// <summary>
/// A pipeline that produces a <see cref="Result"/>.
/// </summary>
public interface IPipeline
{
    /// <summary>
    /// Executes the pipeline.
    /// </summary>
    /// <returns>The execution result.</returns>
    public Result Execute();
}

/// <summary>
/// Async counterpart of <see cref="IPipeline"/>.
/// </summary>
public interface IAsyncPipeline
{
    /// <summary>
    /// Executes the pipeline asynchronously.
    /// </summary>
    /// <returns>The execution result.</returns>
    public Task<Result> ExecuteAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// A pipeline that produces a <see cref="Result{T}"/>.
/// </summary>
/// <typeparam name="T">The value type.</typeparam>
public interface IPipeline<T>
{
    /// <summary>
    /// Executes the pipeline.
    /// </summary>
    /// <returns>The execution result.</returns>
    public Result<T> Execute();
}

/// <summary>
/// Async counterpart of <see cref="IPipeline{T}"/>.
/// </summary>
/// <typeparam name="T">The value type.</typeparam>
public interface IAsyncPipeline<T>
{
    /// <summary>
    /// Executes the pipeline asynchronously.
    /// </summary>
    /// <returns>The execution result.</returns>
    public Task<Result<T>> ExecuteAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// A pipeline that produces a <see cref="Result{T, TError}"/> with a custom error type.
/// </summary>
/// <typeparam name="T">The value type.</typeparam>
/// <typeparam name="TError">The error type.</typeparam>
public interface IPipeline<T, TError>
{
    /// <summary>
    /// Executes the pipeline.
    /// </summary>
    /// <returns>The execution result.</returns>
    public Result<T, TError> Execute();
}

/// <summary>
/// Async counterpart of <see cref="IPipeline{T, TError}"/>.
/// </summary>
/// <typeparam name="T">The value type.</typeparam>
/// <typeparam name="TError">The error type.</typeparam>
public interface IAsyncPipeline<T, TError>
{
    /// <summary>
    /// Executes the pipeline asynchronously.
    /// </summary>
    /// <returns>The execution result.</returns>
    public Task<Result<T, TError>> ExecuteAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Default <see cref="IPipeline"/> implementation.
/// </summary>
public sealed class Pipeline : IPipeline
{
    internal readonly Func<Result> _pipeline;

    internal Pipeline(Func<Result> pipeline)
    {
        _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
    }

    /// <summary>
    /// A successful starting result for pipeline chains.
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
    /// <typeparam name="T">The value type.</typeparam>
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
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="pipeline">The asynchronous pipeline delegate.</param>
    /// <returns>An <see cref="IAsyncPipeline{T}"/> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IAsyncPipeline<T> Create<T>(Func<Task<Result<T>>> pipeline)
    {
        return new AsyncPipeline<T>(pipeline);
    }

    /// <summary>
    /// Creates a new <see cref="IPipeline{T, TError}"/> from the specified delegate.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="pipeline">The pipeline delegate.</param>
    /// <returns>An <see cref="IPipeline{T, TError}"/> instance.</returns>
    public static IPipeline<T, TError> Create<T, TError>(Func<Result<T, TError>> pipeline)
    {
        return new Pipeline<T, TError>(pipeline);
    }

    /// <summary>
    /// Creates a new <see cref="IAsyncPipeline{T, TError}"/> from the specified delegate.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="pipeline">The asynchronous pipeline delegate.</param>
    /// <returns>An <see cref="IAsyncPipeline{T, TError}"/> instance.</returns>
    public static IAsyncPipeline<T, TError> Create<T, TError>(Func<Task<Result<T, TError>>> pipeline)
    {
        return new AsyncPipeline<T, TError>(pipeline);
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

internal sealed class AsyncPipeline : IAsyncPipeline
{
    internal readonly Func<Task<Result>> _pipeline;

    internal AsyncPipeline(Func<Task<Result>> pipeline)
    {
        _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
    }

    public Task<Result> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return _pipeline.Invoke();
    }
}

internal sealed class Pipeline<T> : IPipeline<T>
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

internal sealed class AsyncPipeline<T> : IAsyncPipeline<T>
{
    internal readonly Func<Task<Result<T>>> _pipeline;

    internal AsyncPipeline(Func<Task<Result<T>>> pipeline)
    {
        _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
    }

    public Task<Result<T>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return _pipeline.Invoke();
    }
}

internal sealed class Pipeline<T, TError> : IPipeline<T, TError>
{
    internal readonly Func<Result<T, TError>> _pipeline;

    internal Pipeline(Func<Result<T, TError>> pipeline)
    {
        _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
    }

    public Result<T, TError> Execute()
    {
        return _pipeline.Invoke();
    }
}

internal sealed class AsyncPipeline<T, TError> : IAsyncPipeline<T, TError>
{
    internal readonly Func<Task<Result<T, TError>>> _pipeline;

    internal AsyncPipeline(Func<Task<Result<T, TError>>> pipeline)
    {
        _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
    }

    public Task<Result<T, TError>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return _pipeline.Invoke();
    }
}

/// <summary>
/// Extension methods for wrapping delegates as pipelines and composing fallback behaviour.
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
    /// <typeparam name="T">The value type.</typeparam>
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
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="pipeline">The asynchronous pipeline delegate.</param>
    /// <returns>An <see cref="IAsyncPipeline{T}"/> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IAsyncPipeline<T> AsPipeline<T>(this Func<Task<Result<T>>> pipeline)
    {
        return Pipeline.Create(pipeline);
    }

    /// <summary>
    /// Runs the primary pipeline; falls back to the backup on failure.
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
    /// Async version of <see cref="Fallback(IPipeline, IPipeline)"/>.
    /// </summary>
    /// <param name="primary">The primary async pipeline.</param>
    /// <param name="backup">The backup async pipeline.</param>
    /// <returns>The result of the primary or backup pipeline.</returns>
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
    /// Runs the primary pipeline; falls back to the backup on failure.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
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
    /// Async version of <see cref="Fallback{T}(IPipeline{T}, IPipeline{T})"/>.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="primary">The primary async pipeline.</param>
    /// <param name="backup">The backup async pipeline.</param>
    /// <returns>The result of the primary or backup pipeline.</returns>
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
