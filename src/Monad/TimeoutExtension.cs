using System.Diagnostics;

namespace ForgeSharp.Results.Monad;

/// <summary>
/// Provides extension methods for applying a timeout to pipeline executions.
/// </summary>
public static class TimeoutExtension
{
    /// <summary>
    /// Executes the specified synchronous pipeline with a timeout. If the pipeline does not complete within the given timeout, a failed <see cref="Result"/> is returned.
    /// </summary>
    /// <param name="pipeline">The pipeline to execute.</param>
    /// <param name="timeout">The maximum duration to wait for the pipeline to complete. Must be positive.</param>
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
    /// Executes the specified asynchronous pipeline with a timeout. If the pipeline does not complete within the given timeout, a failed <see cref="Result"/> is returned.
    /// </summary>
    /// <param name="pipeline">The asynchronous pipeline to execute.</param>
    /// <param name="timeout">The maximum duration to wait for the pipeline to complete. Must be positive.</param>
    /// <returns>An asynchronous pipeline that enforces the specified timeout.</returns>
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
    /// Executes the specified synchronous pipeline with a timeout. If the pipeline does not complete within the given timeout, a failed <see cref="Result{T}"/> is returned.
    /// </summary>
    /// <typeparam name="T">The type of the value produced by the pipeline.</typeparam>
    /// <param name="pipeline">The pipeline to execute.</param>
    /// <param name="timeout">The maximum duration to wait for the pipeline to complete. Must be positive.</param>
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
    /// Executes the specified asynchronous pipeline with a timeout. If the pipeline does not complete within the given timeout, a failed <see cref="Result{T}"/> is returned.
    /// </summary>
    /// <typeparam name="T">The type of the value produced by the pipeline.</typeparam>
    /// <param name="pipeline">The asynchronous pipeline to execute.</param>
    /// <param name="timeout">The maximum duration to wait for the pipeline to complete. Must be positive.</param>
    /// <returns>An asynchronous pipeline that enforces the specified timeout.</returns>
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
