using System.Diagnostics;

namespace ForgeSharp.Results.Monad;

/// <summary>
/// Timeout operators for capping pipeline execution time.
/// </summary>
public static class TimeoutExtension
{
    private sealed class TimeoutImpl(IPipeline pipeline, TimeSpan timeout) : IPipeline
    {
        public Result Execute()
        {
            var pip = pipeline;
            var task = Task.Run(pip.Execute);

            return task.Wait(timeout) ? task.Result : Result.Fail("Operation timed out.");
        }
    }

    /// <summary>
    /// Wraps the pipeline with a timeout, failing if it doesn't complete in time.
    /// </summary>
    /// <param name="pipeline">The pipeline to execute.</param>
    /// <param name="timeout">The maximum duration to wait. Must be positive.</param>
    /// <returns>A pipeline that enforces the specified timeout.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="timeout"/> is less than or equal to <see cref="TimeSpan.Zero"/>.</exception>
    [DebuggerStepperBoundary]
    public static IPipeline Timeout(this IPipeline pipeline, TimeSpan timeout)
    {
        if (timeout <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(timeout), "Timeout must be a non-negative TimeSpan.");
        }

        return new TimeoutImpl(pipeline, timeout);
    }

    private sealed class TimeoutImpl<T>(IPipeline<T> pipeline, TimeSpan timeout) : IPipeline<T>
    {
        public Result<T> Execute()
        {
            var pip = pipeline;
            var task = Task.Run(pip.Execute);

            return task.Wait(timeout) ? task.Result : Result.Fail<T>("Operation timed out.");
        }
    }

    /// <summary>
    /// Wraps the pipeline with a timeout, failing if it doesn't complete in time.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="pipeline">The pipeline to execute.</param>
    /// <param name="timeout">The maximum duration to wait. Must be positive.</param>
    /// <returns>A pipeline that enforces the specified timeout.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="timeout"/> is less than or equal to <see cref="TimeSpan.Zero"/>.</exception>
    [DebuggerStepperBoundary]
    public static IPipeline<T> Timeout<T>(this IPipeline<T> pipeline, TimeSpan timeout)
    {
        if (timeout <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(timeout), "Timeout must be a non-negative TimeSpan.");
        }

        return new TimeoutImpl<T>(pipeline, timeout);
    }

    private sealed class TimeoutImpl<T, TError>(IPipeline<T, TError> pipeline, TimeSpan timeout, TError onTimeout) : IPipeline<T, TError>
    {
        public Result<T, TError> Execute()
        {
            var pip = pipeline;
            var task = Task.Run(pip.Execute);

            return task.Wait(timeout) ? task.Result : Result.Fail<T, TError>(onTimeout);
        }
    }

    /// <summary>
    /// Wraps the pipeline with a timeout, returning the specified error if it doesn't complete in time.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="pipeline">The pipeline to execute.</param>
    /// <param name="timeout">The maximum duration to wait. Must be positive.</param>
    /// <param name="onTimeout">The error to return if the pipeline times out.</param>
    /// <returns>A pipeline that enforces the specified timeout.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="timeout"/> is less than or equal to <see cref="TimeSpan.Zero"/>.</exception>
    [DebuggerStepperBoundary]
    public static IPipeline<T, TError> Timeout<T, TError>(this IPipeline<T, TError> pipeline, TimeSpan timeout, TError onTimeout)
    {
        if (timeout <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(timeout), "Timeout must be a non-negative TimeSpan.");
        }

        return new TimeoutImpl<T, TError>(pipeline, timeout, onTimeout);
    }

    private sealed class AsyncTimeoutImpl(IAsyncPipeline pipeline, TimeSpan timeout) : IAsyncPipeline
    {
        public async Task<Result> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var task = pipeline.ExecuteAsync(cancellationToken);
            var completed = await Task.WhenAny(task, Task.Delay(timeout, cancellationToken)).ConfigureAwait(false);

            return completed == task ? await task.ConfigureAwait(false) : Result.Fail("Operation timed out.");
        }
    }

    /// <summary>
    /// Async version of <see cref="Timeout(IPipeline, TimeSpan)"/>.
    /// </summary>
    /// <param name="pipeline">The asynchronous pipeline to execute.</param>
    /// <param name="timeout">The maximum duration to wait. Must be positive.</param>
    /// <returns>An async pipeline that enforces the specified timeout.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="timeout"/> is less than or equal to <see cref="TimeSpan.Zero"/>.</exception>
    [DebuggerStepperBoundary]
    public static IAsyncPipeline TimeoutAsync(this IAsyncPipeline pipeline, TimeSpan timeout)
    {
        if (timeout <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(timeout), "Timeout must be a non-negative TimeSpan.");
        }

        return new AsyncTimeoutImpl(pipeline, timeout);
    }

    private sealed class AsyncTimeoutImpl<T>(IAsyncPipeline<T> pipeline, TimeSpan timeout) : IAsyncPipeline<T>
    {
        public async Task<Result<T>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var task = pipeline.ExecuteAsync(cancellationToken);
            var completed = await Task.WhenAny(task, Task.Delay(timeout, cancellationToken)).ConfigureAwait(false);

            return completed == task ? await task.ConfigureAwait(false) : Result.Fail<T>("Operation timed out.");
        }
    }

    /// <summary>
    /// Async version of <see cref="Timeout{T}(IPipeline{T}, TimeSpan)"/>.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="pipeline">The asynchronous pipeline to execute.</param>
    /// <param name="timeout">The maximum duration to wait. Must be positive.</param>
    /// <returns>An async pipeline that enforces the specified timeout.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="timeout"/> is less than or equal to <see cref="TimeSpan.Zero"/>.</exception>
    [DebuggerStepperBoundary]
    public static IAsyncPipeline<T> TimeoutAsync<T>(this IAsyncPipeline<T> pipeline, TimeSpan timeout)
    {
        if (timeout <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(timeout), "Timeout must be a non-negative TimeSpan.");
        }

        return new AsyncTimeoutImpl<T>(pipeline, timeout);
    }

    private sealed class AsyncTimeoutImpl<T, TError>(IAsyncPipeline<T, TError> pipeline, TimeSpan timeout, TError onTimeout) : IAsyncPipeline<T, TError>
    {
        public async Task<Result<T, TError>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var task = pipeline.ExecuteAsync(cancellationToken);
            var completed = await Task.WhenAny(task, Task.Delay(timeout, cancellationToken)).ConfigureAwait(false);

            return completed == task ? await task.ConfigureAwait(false) : Result.Fail<T, TError>(onTimeout);
        }
    }

    /// <summary>
    /// Async version of timeout with a discriminated error type, returning the specified error on timeout.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="pipeline">The asynchronous pipeline to execute.</param>
    /// <param name="timeout">The maximum duration to wait. Must be positive.</param>
    /// <param name="onTimeout">The error to return if the pipeline times out.</param>
    /// <returns>An async pipeline that enforces the specified timeout.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="timeout"/> is less than or equal to <see cref="TimeSpan.Zero"/>.</exception>
    [DebuggerStepperBoundary]
    public static IAsyncPipeline<T, TError> TimeoutAsync<T, TError>(this IAsyncPipeline<T, TError> pipeline, TimeSpan timeout, TError onTimeout)
    {
        if (timeout <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(timeout), "Timeout must be a non-negative TimeSpan.");
        }

        return new AsyncTimeoutImpl<T, TError>(pipeline, timeout, onTimeout);
    }
}
