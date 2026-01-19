using ForgeSharp.Results.Infrastructure;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ForgeSharp.Results.Monad;

/// <summary>
/// Provides helpers to "flatten" nested <see cref="Result"/> and sequences of <see cref="Result"/> into
/// a single <see cref="Result"/> or <see cref="Result{T}"/>. Useful for composing operations that may produce
/// nested results or collections of results.
/// </summary>
public static class FlattenExtension
{
    /// <summary>
    /// Flattens a nested non-generic <see cref="Result"/> contained inside a <see cref="Result"/> to a single <see cref="Result"/>.
    /// </summary>
    /// <param name="result">The nested result to flatten.</param>
    /// <returns>
    /// If <paramref name="result"/> is successful, returns the inner <see cref="Result"/> value; otherwise forwards the failure.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result Flatten(this Result<Result> result)
    {
        if (result.IsSuccess)
        {
            return result.Value;
        }

        return Result.ForwardFail(result);
    }

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
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result<T>> FlattenAsync<T>(this Task<Result<Result<T>>> resultTask)
    {
        if (resultTask.TryGetResult(out var result))
        {
            Task.FromResult(Flatten(result));
        }

        return Impl(resultTask);

        static async Task<Result<T>> Impl(Task<Result<Result<T>>> resultTask)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (result.IsSuccess)
            {
                return result.Value;
            }

            return Result.ForwardFail<T>(result);
        }
    }

    /// <summary>
    /// Asynchronously flattens a nested <see cref="Result"/> inside a <see cref="Result{Result}"/> to a single <see cref="Result"/>.
    /// </summary>
    /// <param name="resultTask">The task containing the nested result.</param>
    /// <returns>The flattened result as a task.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result> FlattenAsync(this Task<Result<Result>> resultTask)
    {
        if (resultTask.TryGetResult(out var result))
        {
            Task.FromResult(Flatten(result));
        }

        return Impl(resultTask);

        static async Task<Result> Impl(Task<Result<Result>> resultTask)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (result.IsSuccess)
            {
                return result.Value;
            }

            return (Result)result;
        }
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
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result> FlattenAsync<T>(this Task<IEnumerable<Result>> resultTask)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return Task.FromResult(Flatten(result));
        }

        return Impl(resultTask);

        static async Task<Result> Impl(Task<IEnumerable<Result>> resultTask)
        {
            var results = await resultTask.ConfigureAwait(false);

            if (results.Any(x => !x.IsSuccess))
            {
                return results.First(x => !x.IsSuccess);
            }

            return Result.Ok();
        }
    }

#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER

    /// <summary>
    /// Asynchronously flattens a nested <see cref="Result{T}"/> inside a <see cref="Result{Result{T}}"/> to a single <see cref="Result{T}"/> using a ValueTask.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="resultTask">The value task containing the nested result.</param>
    /// <returns>The flattened result as a value task.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<Result<T>> FlattenAsync<T>(this ValueTask<Result<Result<T>>> resultTask)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return AsyncHelper.CreateValueTask(Flatten(result));
        }

        return Impl(resultTask);

        static async ValueTask<Result<T>> Impl(ValueTask<Result<Result<T>>> resultTask)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (result.IsSuccess)
            {
                return result.Value;
            }

            return Result.ForwardFail<T>(result);
        }
    }

    /// <summary>
    /// Asynchronously flattens a nested <see cref="Result"/> inside a <see cref="Result{Result}"/> to a single <see cref="Result"/> using a ValueTask.
    /// </summary>
    /// <param name="resultTask">The value task containing the nested result.</param>
    /// <returns>The flattened result as a value task.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<Result> FlattenAsync(this ValueTask<Result<Result>> resultTask)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return AsyncHelper.CreateValueTask(Flatten(result));
        }

        return Impl(resultTask);

        static async ValueTask<Result> Impl(ValueTask<Result<Result>> resultTask)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (result.IsSuccess)
            {
                return result.Value;
            }

            return (Result)result;
        }
    }

    /// <summary>
    /// Asynchronously flattens a sequence of <see cref="Result"/> objects, returning the first failed result or a successful result if all are successful, using a ValueTask.
    /// </summary>
    /// <typeparam name="T">The type of the value (unused).</typeparam>
    /// <param name="resultTask">The value task containing the sequence of results.</param>
    /// <returns>The first failed result, or a successful result if all are successful, as a value task.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<Result> FlattenAsync<T>(this ValueTask<IEnumerable<Result>> resultTask)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return AsyncHelper.CreateValueTask(Flatten(result));
        }

        return Impl(resultTask);

        static async ValueTask<Result> Impl(ValueTask<IEnumerable<Result>> resultTask)
        {
            var results = await resultTask.ConfigureAwait(false);

            if (results.Any(x => !x.IsSuccess))
            {
                return results.First(x => !x.IsSuccess);
            }

            return Result.Ok();
        }
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
