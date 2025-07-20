using System.Diagnostics;

namespace ForgeSharp.Results.Monad;

/// <summary>
/// Provides extension methods for repeating the execution of pipelines multiple times.
/// </summary>
public static class RepeatExtension
{
    /// <summary>
    /// Executes the specified synchronous pipeline a given number of times, yielding each <see cref="Result"/>.
    /// </summary>
    /// <param name="pipeline">The pipeline to execute.</param>
    /// <param name="count">The number of times to execute the pipeline. Must be greater than 0.</param>
    /// <returns>An <see cref="IEnumerable{Result}"/> containing the results of each execution.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="count"/> is less than 1.</exception>
    [DebuggerStepperBoundary]
    public static IEnumerable<Result> Repeat(this IPipeline pipeline, int count)
    {
        if (count < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Count must be greater than 0.");
        }

        for (int i = 0; i < count; i++)
        {
            yield return pipeline.Execute();
        }
    }

    /// <summary>
    /// Executes the specified synchronous pipeline a given number of times, yielding each <see cref="Result{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value produced by the pipeline.</typeparam>
    /// <param name="pipeline">The pipeline to execute.</param>
    /// <param name="count">The number of times to execute the pipeline. Must be greater than 0.</param>
    /// <returns>An <see cref="IEnumerable{Result{T}}"/> containing the results of each execution.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="count"/> is less than 1.</exception>
    [DebuggerStepperBoundary]
    public static IEnumerable<Result<T>> Repeat<T>(this IPipeline<T> pipeline, int count)
    {
        if (count < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Count must be greater than 0.");
        }

        for (int i = 0; i < count; i++)
        {
            yield return pipeline.Execute();
        }
    }

#if NETSTANDARD2_1 || NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER

    /// <summary>
    /// Asynchronously executes the specified pipeline a given number of times, yielding each <see cref="Result"/> as they complete.
    /// </summary>
    /// <param name="pipeline">The asynchronous pipeline to execute.</param>
    /// <param name="count">The number of times to execute the pipeline. Must be greater than 0.</param>
    /// <returns>An <see cref="IAsyncEnumerable{Result}"/> containing the results of each execution.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="count"/> is less than 1.</exception>
    [DebuggerStepperBoundary]
    public static async IAsyncEnumerable<Result> RepeatAsync(this IAsyncPipeline pipeline, int count)
    {
        if (count < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Count must be greater than 0.");
        }

        for (int i = 0; i < count; i++)
        {
            yield return await pipeline.ExecuteAsync();
        }
    }

    /// <summary>
    /// Asynchronously executes the specified pipeline a given number of times, yielding each <see cref="Result{T}"/> as they complete.
    /// </summary>
    /// <typeparam name="T">The type of the value produced by the pipeline.</typeparam>
    /// <param name="pipeline">The asynchronous pipeline to execute.</param>
    /// <param name="count">The number of times to execute the pipeline. Must be greater than 0.</param>
    /// <returns>An <see cref="IAsyncEnumerable{Result{T}}"/> containing the results of each execution.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="count"/> is less than 1.</exception>
    [DebuggerStepperBoundary]
    public static async IAsyncEnumerable<Result<T>> RepeatAsync<T>(this IAsyncPipeline<T> pipeline, int count)
    {
        if (count < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Count must be greater than 0.");
        }

        for (int i = 0; i < count; i++)
        {
            yield return await pipeline.ExecuteAsync();
        }
    }

#else

    /// <summary>
    /// Asynchronously executes the specified pipeline a given number of times, returning all <see cref="Result"/> objects as a collection.
    /// </summary>
    /// <param name="pipeline">The asynchronous pipeline to execute.</param>
    /// <param name="count">The number of times to execute the pipeline. Must be greater than 0.</param>
    /// <returns>A task representing the asynchronous operation, with a collection of results.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="count"/> is less than 1.</exception>
    [DebuggerStepperBoundary]
    public static async Task<IEnumerable<Result>> RepeatAsync(this IAsyncPipeline pipeline, int count)
    {
        if (count < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Count must be greater than 0.");
        }

        var results = new List<Result>();

        for (int i = 0; i < count; i++)
        {
            results.Add(await pipeline.ExecuteAsync());
        }

        return results;
    }

    /// <summary>
    /// Asynchronously executes the specified pipeline a given number of times, returning all <see cref="Result{T}"/> objects as a collection.
    /// </summary>
    /// <typeparam name="T">The type of the value produced by the pipeline.</typeparam>
    /// <param name="pipeline">The asynchronous pipeline to execute.</param>
    /// <param name="count">The number of times to execute the pipeline. Must be greater than 0.</param>
    /// <returns>A task representing the asynchronous operation, with a collection of results.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="count"/> is less than 1.</exception>
    [DebuggerStepperBoundary]
    public static async Task<IEnumerable<Result<T>>> RepeatAsync<T>(this IAsyncPipeline<T> pipeline, int count)
    {
        if (count < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Count must be greater than 0.");
        }

        var results = new List<Result<T>>();

        for (int i = 0; i < count; i++)
        {
            results.Add(await pipeline.ExecuteAsync());
        }

        return results;
    }

#endif
}
