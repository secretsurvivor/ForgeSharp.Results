using System.Diagnostics;

namespace ForgeSharp.Results.Monad;

/// <summary>
/// Extension methods that wrap pipeline execution with elapsed-time measurement.
/// </summary>
public static class MeasureExtension
{
    /// <summary>
    /// Executes the pipeline and records how long it took.
    /// </summary>
    /// <param name="pipeline">The pipeline to execute.</param>
    /// <returns>A <see cref="MeasureResult"/> containing the elapsed time and the result.</returns>
    public static MeasureResult ExecuteAndMeasure(this IPipeline pipeline)
    {
#if NET6_0_OR_GREATER
        long start = Stopwatch.GetTimestamp();
        var result = pipeline.Execute();
        var elapsed = Stopwatch.GetElapsedTime(start);

        return new MeasureResult(elapsed, result);
#else
        var stopwatch = Stopwatch.StartNew();
        var result = pipeline.Execute();
        stopwatch.Stop();

        return new MeasureResult(stopwatch.Elapsed, result);
#endif
    }

    /// <summary>
    /// Async version of <see cref="ExecuteAndMeasure(IPipeline)"/>.
    /// </summary>
    /// <param name="pipeline">The asynchronous pipeline to execute.</param>
    /// <returns>A <see cref="MeasureResult"/> containing the elapsed time and the result.</returns>
    public static async Task<MeasureResult> ExecuteAndMeasureAsync(this IAsyncPipeline pipeline)
    {
#if NET6_0_OR_GREATER
        long start = Stopwatch.GetTimestamp();
        var result = await pipeline.ExecuteAsync();
        var elapsed = Stopwatch.GetElapsedTime(start);

        return new MeasureResult(elapsed, result);
#else
        var stopwatch = Stopwatch.StartNew();
        var result = await pipeline.ExecuteAsync();
        stopwatch.Stop();

        return new MeasureResult(stopwatch.Elapsed, result);
#endif
    }

    /// <summary>
    /// Executes the typed pipeline and records how long it took.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="pipeline">The pipeline to execute.</param>
    /// <returns>A <see cref="MeasureResult{T}"/> containing the elapsed time and the result.</returns>
    public static MeasureResult<T> ExecuteAndMeasure<T>(this IPipeline<T> pipeline)
    {
#if NET6_0_OR_GREATER
        long start = Stopwatch.GetTimestamp();
        var result = pipeline.Execute();
        var elapsed = Stopwatch.GetElapsedTime(start);

        return new MeasureResult<T>(elapsed, result);
#else
        var stopwatch = Stopwatch.StartNew();
        var result = pipeline.Execute();
        stopwatch.Stop();

        return new MeasureResult<T>(stopwatch.Elapsed, result);
#endif
    }

    /// <summary>
    /// Async version of <see cref="ExecuteAndMeasure{T}(IPipeline{T})"/>.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="pipeline">The asynchronous pipeline to execute.</param>
    /// <returns>A <see cref="MeasureResult{T}"/> containing the elapsed time and the result.</returns>
    public static async Task<MeasureResult<T>> ExecuteAndMeasureAsync<T>(this IAsyncPipeline<T> pipeline)
    {
#if NET6_0_OR_GREATER
        long start = Stopwatch.GetTimestamp();
        var result = await pipeline.ExecuteAsync();
        var elapsed = Stopwatch.GetElapsedTime(start);

        return new MeasureResult<T>(elapsed, result);
#else
        var stopwatch = Stopwatch.StartNew();
        var result = await pipeline.ExecuteAsync();
        stopwatch.Stop();

        return new MeasureResult<T>(stopwatch.Elapsed, result);
#endif
    }

    /// <summary>
    /// Executes the discriminated pipeline and records how long it took.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="pipeline">The pipeline to execute.</param>
    /// <returns>A <see cref="MeasureResult{T, TError}"/> containing the elapsed time and the result.</returns>
    public static MeasureResult<T, TError> ExecuteAndMeasure<T, TError>(this IPipeline<T, TError> pipeline)
    {
#if NET6_0_OR_GREATER
        long start = Stopwatch.GetTimestamp();
        var result = pipeline.Execute();
        var elapsed = Stopwatch.GetElapsedTime(start);

        return new MeasureResult<T, TError>(elapsed, result);
#else
        var stopwatch = Stopwatch.StartNew();
        var result = pipeline.Execute();
        stopwatch.Stop();

        return new MeasureResult<T, TError>(stopwatch.Elapsed, result);
#endif
    }

    /// <summary>
    /// Async version of <see cref="ExecuteAndMeasure{T, TError}(IPipeline{T, TError})"/>.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="pipeline">The asynchronous pipeline to execute.</param>
    /// <returns>A <see cref="MeasureResult{T, TError}"/> containing the elapsed time and the result.</returns>
    public static async Task<MeasureResult<T, TError>> ExecuteAndMeasureAsync<T, TError>(this IAsyncPipeline<T, TError> pipeline)
    {
#if NET6_0_OR_GREATER
        long start = Stopwatch.GetTimestamp();
        var result = await pipeline.ExecuteAsync();
        var elapsed = Stopwatch.GetElapsedTime(start);

        return new MeasureResult<T, TError>(elapsed, result);
#else
        var stopwatch = Stopwatch.StartNew();
        var result = await pipeline.ExecuteAsync();
        stopwatch.Stop();

        return new MeasureResult<T, TError>(stopwatch.Elapsed, result);
#endif
    }
}

/// <summary>
/// Pairs a non-generic <see cref="Result"/> with the wall-clock time it took to produce.
/// </summary>
/// <param name="elapsed">How long the pipeline took to execute.</param>
/// <param name="result">The result of the pipeline execution.</param>
public readonly struct MeasureResult(TimeSpan elapsed, Result result)
{
    /// <summary>
    /// How long the pipeline took to execute.
    /// </summary>
    public TimeSpan Elapsed => elapsed;

    /// <summary>
    /// The result produced by the pipeline.
    /// </summary>
    public Result Result => result;

    /// <summary>
    /// Deconstructs the diagnostic result into its components.
    /// </summary>
    /// <param name="elapsed">Receives the elapsed time.</param>
    /// <param name="result">Receives the result.</param>
    public void Deconstruct(out TimeSpan elapsed, out Result result)
    {
        elapsed = Elapsed;
        result = Result;
    }
}

/// <summary>
/// Pairs a <see cref="Result{T}"/> with the wall-clock time it took to produce.
/// </summary>
/// <typeparam name="T">The value type.</typeparam>
/// <param name="elapsed">How long the pipeline took to execute.</param>
/// <param name="result">The result of the pipeline execution.</param>
public readonly struct MeasureResult<T>(TimeSpan elapsed, Result<T> result)
{
    /// <summary>
    /// How long the pipeline took to execute.
    /// </summary>
    public TimeSpan Elapsed => elapsed;

    /// <summary>
    /// The result produced by the pipeline.
    /// </summary>
    public Result<T> Result => result;

    /// <summary>
    /// Deconstructs the diagnostic result into its components.
    /// </summary>
    /// <param name="elapsed">Receives the elapsed time.</param>
    /// <param name="result">Receives the result.</param>
    public void Deconstruct(out TimeSpan elapsed, out Result<T> result)
    {
        elapsed = Elapsed;
        result = Result;
    }
}

/// <summary>
/// Pairs a <see cref="Result{T, TError}"/> with the wall-clock time it took to produce.
/// </summary>
/// <typeparam name="T">The value type.</typeparam>
/// <typeparam name="TError">The error type.</typeparam>
/// <param name="elapsed">How long the pipeline took to execute.</param>
/// <param name="result">The result of the pipeline execution.</param>
public readonly struct MeasureResult<T, TError>(TimeSpan elapsed, Result<T, TError> result)
{
    /// <summary>
    /// How long the pipeline took to execute.
    /// </summary>
    public TimeSpan Elapsed => elapsed;

    /// <summary>
    /// The result produced by the pipeline.
    /// </summary>
    public Result<T, TError> Result => result;

    /// <summary>
    /// Deconstructs the diagnostic result into its components.
    /// </summary>
    /// <param name="elapsed">Receives the elapsed time.</param>
    /// <param name="result">Receives the result.</param>
    public void Deconstruct(out TimeSpan elapsed, out Result<T, TError> result)
    {
        elapsed = Elapsed;
        result = Result;
    }
}
