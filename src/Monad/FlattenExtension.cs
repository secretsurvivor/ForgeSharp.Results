using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ForgeSharp.Results.Monad;

public static class FlattenExtension
{
    /// <summary>
    /// Flattens a nested <see cref="Result{T}"/> inside a <see cref="Result{Result{T}}"/> to a single <see cref="Result{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="result">The nested result.</param>
    /// <returns>The flattened result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> Flatten<T>(this Result<Result<T>> result)
    {
        if (result.IsSuccess)
        {
            return result.Value;
        }

        return Result.ForwardFail<T>(result);
    }

    /// <summary>
    /// Asynchronously flattens a nested <see cref="Result{T}"/> inside a <see cref="Result{Result{T}}"/> to a single <see cref="Result{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="resultTask">The task containing the nested result.</param>
    /// <returns>The flattened result as a task.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<T>> FlattenAsync<T>(this Task<Result<Result<T>>> resultTask)
    {
        var result = await resultTask.ConfigureAwait(false);

        if (result.IsSuccess)
        {
            return result.Value;
        }

        return Result.ForwardFail<T>(result);
    }

    /// <summary>
    /// Asynchronously flattens a nested <see cref="Result"/> inside a <see cref="Result{Result}"/> to a single <see cref="Result"/>.
    /// </summary>
    /// <param name="resultTask">The task containing the nested result.</param>
    /// <returns>The flattened result as a task.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result> FlattenAsync(this Task<Result<Result>> resultTask)
    {
        var result = await resultTask.ConfigureAwait(false);

        if (result.IsSuccess)
        {
            return result.Value;
        }

        return (Result)result;
    }

    /// <summary>
    /// Flattens a sequence of <see cref="Result"/> objects, returning the first failed result or a successful result if all are successful.
    /// </summary>
    /// <param name="results">The sequence of results.</param>
    /// <returns>The first failed result, or a successful result if all are successful.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result Flatten(this IEnumerable<Result> results)
    {
        if (results.Any(x => !x.IsSuccess))
        {
            return results.First(x => !x.IsSuccess);
        }

        return Result.Ok();
    }

    /// <summary>
    /// Asynchronously flattens a sequence of <see cref="Result"/> objects, returning the first failed result or a successful result if all are successful.
    /// </summary>
    /// <typeparam name="T">The type of the value (unused).</typeparam>
    /// <param name="resultTask">The task containing the sequence of results.</param>
    /// <returns>The first failed result, or a successful result if all are successful, as a task.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result> FlattenAsync<T>(this Task<IEnumerable<Result>> resultTask)
    {
        var results = await resultTask.ConfigureAwait(false);

        if (results.Any(x => !x.IsSuccess))
        {
            return results.First(x => !x.IsSuccess);
        }

        return Result.Ok();
    }

#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER

    /// <summary>
    /// Asynchronously flattens a nested <see cref="Result{T}"/> inside a <see cref="Result{Result{T}}"/> to a single <see cref="Result{T}"/> using a ValueTask.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="resultTask">The value task containing the nested result.</param>
    /// <returns>The flattened result as a value task.</returns>
    [DebuggerStepperBoundary]
    public static async ValueTask<Result<T>> FlattenAsync<T>(this ValueTask<Result<Result<T>>> resultTask)
    {
        var result = await resultTask.ConfigureAwait(false);

        if (result.IsSuccess)
        {
            return result.Value;
        }

        return Result.ForwardFail<T>(result);
    }

    /// <summary>
    /// Asynchronously flattens a nested <see cref="Result"/> inside a <see cref="Result{Result}"/> to a single <see cref="Result"/> using a ValueTask.
    /// </summary>
    /// <param name="resultTask">The value task containing the nested result.</param>
    /// <returns>The flattened result as a value task.</returns>
    [DebuggerStepperBoundary]
    public static async ValueTask<Result> FlattenAsync(this ValueTask<Result<Result>> resultTask)
    {
        var result = await resultTask.ConfigureAwait(false);

        if (result.IsSuccess)
        {
            return result.Value;
        }

        return (Result)result;
    }

    /// <summary>
    /// Asynchronously flattens a sequence of <see cref="Result"/> objects, returning the first failed result or a successful result if all are successful, using a ValueTask.
    /// </summary>
    /// <typeparam name="T">The type of the value (unused).</typeparam>
    /// <param name="resultTask">The value task containing the sequence of results.</param>
    /// <returns>The first failed result, or a successful result if all are successful, as a value task.</returns>
    [DebuggerStepperBoundary]
    public static async ValueTask<Result> FlattenAsync<T>(this ValueTask<IEnumerable<Result>> resultTask)
    {
        var results = await resultTask.ConfigureAwait(false);

        if (results.Any(x => !x.IsSuccess))
        {
            return results.First(x => !x.IsSuccess);
        }

        return Result.Ok();
    }

    /// <summary>
    /// Asynchronously flattens a sequence of <see cref="Result"/> objects from an <see cref="IAsyncEnumerable{Result}"/>, returning the first failed result or a successful result if all are successful.
    /// </summary>
    /// <param name="results">The asynchronous sequence of results.</param>
    /// <returns>A task representing the asynchronous operation, with the first failed result or a successful result if all are successful.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result> FlattenAsync(this IAsyncEnumerable<Result> results)
    {
        await foreach (var result in results.ConfigureAwait(false))
        {
            if (!result.IsSuccess)
            {
                return result;
            }
        }

        return Result.Ok();
    }

#endif
}
