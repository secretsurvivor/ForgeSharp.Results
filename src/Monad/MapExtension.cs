using ForgeSharp.Results.Infrastructure;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ForgeSharp.Results.Monad;

/// <summary>
/// Provides mapping helpers that transform the payloads inside successful <see cref="Result{T}"/> sequences
/// into a different form while preserving failure forwarding semantics.
/// </summary>
public static class MapExtension
{
    /// <summary>
    /// Maps the values of a successful result sequence to a new type.
    /// </summary>
    /// <typeparam name="T">The type of the input values.</typeparam>
    /// <typeparam name="TResult">The type of the result values.</typeparam>
    /// <param name="result">The result containing a sequence of values.</param>
    /// <param name="func">The mapping function.</param>
    /// <returns>A new result with mapped values.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<IEnumerable<TResult>> Map<T, TResult>(this Result<IEnumerable<T>> result, Func<T, TResult> func)
    {
        if (result.IsSuccess)
        {
            return Result.Ok(result.Value.Select(func));
        }

        return Result.ForwardFail<IEnumerable<TResult>>(result);
    }

    /// <summary>
    /// Maps the values of a successful awaited result sequence to a new type.
    /// </summary>
    /// <typeparam name="T">The type of the input values.</typeparam>
    /// <typeparam name="TResult">The type of the result values.</typeparam>
    /// <param name="resultTask">The result task containing a sequence of values.</param>
    /// <param name="func">The mapping function.</param>
    /// <returns>A new result with mapped values as a task.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result<IEnumerable<TResult>>> MapAsync<T, TResult>(this Task<Result<IEnumerable<T>>> resultTask, Func<T, TResult> func)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return Task.FromResult(Map(result, func));
        }

        return Impl(resultTask, func);

        static async Task<Result<IEnumerable<TResult>>> Impl(Task<Result<IEnumerable<T>>> resultTask, Func<T, TResult> func)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (result.IsSuccess)
            {
                return Result.Ok(result.Value.Select(func));
            }

            return Result.ForwardFail<IEnumerable<TResult>>(result);
        }
    }

#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER

    /// <summary>
    /// Maps the values of a successful awaited result sequence to a new type using a ValueTask.
    /// </summary>
    /// <typeparam name="T">The type of the input values.</typeparam>
    /// <typeparam name="TResult">The type of the result values.</typeparam>
    /// <param name="resultTask">The result value task containing a sequence of values.</param>
    /// <param name="func">The mapping function.</param>
    /// <returns>A new result with mapped values as a value task.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<Result<IEnumerable<TResult>>> MapAsync<T, TResult>(this ValueTask<Result<IEnumerable<T>>> resultTask, Func<T, TResult> func)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return AsyncHelper.CreateValueTask(Map(result, func));
        }

        return Impl(resultTask, func);

        static async ValueTask<Result<IEnumerable<TResult>>> Impl(ValueTask<Result<IEnumerable<T>>> resultTask, Func<T, TResult> func)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (result.IsSuccess)
            {
                return Result.Ok(result.Value.Select(func));
            }

            return Result.ForwardFail<IEnumerable<TResult>>(result);
        }
    }
#endif
}
