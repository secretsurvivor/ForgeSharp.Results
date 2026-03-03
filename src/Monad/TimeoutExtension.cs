using System.Diagnostics;

namespace ForgeSharp.Results.Monad;

/// <summary>
/// Timeout operators for capping pipeline execution time.
/// </summary>
public static class TimeoutExtension
{
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
        return Pipeline.Create(() =>
        {
            if (timeout <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout), "Timeout must be a non-negative TimeSpan.");
            }

            var task = Task.Run(() => pipeline.Execute());

            return task.Wait(timeout) ? task.Result : Result.Fail("Operation timed out.");
        });
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
        return Pipeline.Create(async () =>
        {
            if (timeout <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout), "Timeout must be a non-negative TimeSpan.");
            }

            var task = pipeline.ExecuteAsync();
            var completed = await Task.WhenAny(task, Task.Delay(timeout)).ConfigureAwait(false);

            return completed == task ? await task.ConfigureAwait(false) : Result.Fail("Operation timed out.");
        });
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
        return Pipeline.Create<T>(() =>
        {
            if (timeout <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout), "Timeout must be a non-negative TimeSpan.");
            }

            var task = Task.Run(() => pipeline.Execute());

            return task.Wait(timeout) ? task.Result : Result.Fail<T>("Operation timed out.");
        });
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
        return Pipeline.Create<T>(async () =>
        {
            if (timeout <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout), "Timeout must be a non-negative TimeSpan.");
            }

            var task = pipeline.ExecuteAsync();
            var completed = await Task.WhenAny(task, Task.Delay(timeout)).ConfigureAwait(false);

            return completed == task ? await task.ConfigureAwait(false) : Result.Fail<T>("Operation timed out.");
        });
    }
}
