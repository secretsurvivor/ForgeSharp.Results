using ForgeSharp.Results.Infrastructure;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ForgeSharp.Results.Monad;

/// <summary>
/// Provides LINQ-like projection helpers for <see cref="EnumerableResult"/> and <see cref="EnumerableResult{T}"/>.
/// The projection is performed lazily as the returned <see cref="IEnumerable{T}"/> is iterated.
/// </summary>
public static class SelectExtension
{
    /// <summary>
    /// Projects each <see cref="Result"/> in <paramref name="enumerableResult"/> into a new form using the supplied <paramref name="selector"/>.
    /// </summary>
    /// <typeparam name="T">The element type produced by the selector.</typeparam>
    /// <param name="enumerableResult">The source <see cref="EnumerableResult"/> whose elements will be projected.</param>
    /// <param name="selector">A transform function to apply to each <see cref="Result"/>. This function is invoked for each element during enumeration.</param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> that yields the projected values in the same order as the source results.
    /// Enumeration is lazy and will invoke <paramref name="selector"/> per element when iterated.
    /// </returns>
    /// <remarks>
    /// The method does not perform argument validation — providing a <c>null</c> <paramref name="selector"/> will cause an exception when the returned sequence is enumerated.
    /// </remarks>
    public static IEnumerable<T> Select<T>(this EnumerableResult enumerableResult, Func<Result, T> selector)
    {
        var results = enumerableResult._results;

        for (var i = 0; i < results.Length; i++)
        {
            yield return selector(results[i]);
        }
    }

    /// <summary>
    /// Projects each <see cref="Result{T}"/> in <paramref name="enumerableResult"/> into a new form using the supplied <paramref name="selector"/>.
    /// </summary>
    /// <typeparam name="T">The payload type carried by each source <see cref="Result{T}"/>.</typeparam>
    /// <typeparam name="TResult">The element type produced by the selector.</typeparam>
    /// <param name="enumerableResult">The source <see cref="EnumerableResult{T}"/> whose elements will be projected.</param>
    /// <param name="selector">A transform function to apply to each <see cref="Result{T}"/>. This function is invoked for each element during enumeration.</param>
    /// <returns>
    /// An <see cref="IEnumerable{TResult}"/> that yields the projected values in the same order as the source results.
    /// Enumeration is lazy and will invoke <paramref name="selector"/> per element when iterated.
    /// </returns>
    /// <remarks>
    /// The method does not perform argument validation — providing a <c>null</c> <paramref name="selector"/> will cause an exception when the returned sequence is enumerated.
    /// </remarks>
    public static IEnumerable<TResult> Select<T, TResult>(this EnumerableResult<T> enumerableResult, Func<Result<T>, TResult> selector)
    {
        var results = enumerableResult._results;

        for (var i = 0; i < results.Length; i++)
        {
            yield return selector(results[i]);
        }
    }

    /// <summary>
    /// Selects the values of a successful result sequence to a new type.
    /// </summary>
    /// <typeparam name="T">The type of the input values.</typeparam>
    /// <typeparam name="TResult">The type of the result values.</typeparam>
    /// <param name="result">The result containing a sequence of values.</param>
    /// <param name="func">The selection function.</param>
    /// <returns>A new result with selected values.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<IEnumerable<TResult>> Select<T, TResult>(this Result<IEnumerable<T>> result, Func<T, TResult> func)
    {
        if (result.IsSuccess)
        {
            return Result.Ok(result.Value.Select(func));
        }

        return Result.ForwardFail<IEnumerable<TResult>>(result);
    }

    /// <summary>
    /// Selects the values of a successful awaited result sequence to a new type.
    /// </summary>
    /// <typeparam name="T">The type of the input values.</typeparam>
    /// <typeparam name="TResult">The type of the result values.</typeparam>
    /// <param name="resultTask">The result task containing a sequence of values.</param>
    /// <param name="func">The selection function.</param>
    /// <returns>A new result with selected values as a task.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result<IEnumerable<TResult>>> SelectAsync<T, TResult>(this Task<Result<IEnumerable<T>>> resultTask, Func<T, TResult> func)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return Task.FromResult(Select(result, func));
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

    /// <summary>
    /// Selects the values of a successful result sequence to a new type.
    /// </summary>
    /// <typeparam name="T">The type of the input values.</typeparam>
    /// <typeparam name="TResult">The type of the result values.</typeparam>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="result">The result containing a sequence of values.</param>
    /// <param name="func">The selection function.</param>
    /// <returns>A new result with selected values.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<IEnumerable<TResult>, TError> Select<T, TResult, TError>(this Result<IEnumerable<T>, TError> result, Func<T, TResult> func)
    {
        if (!result.IsSuccess)
        {
            return Result.Fail<IEnumerable<TResult>, TError>(result.Error);
        }

        return Result.Ok<IEnumerable<TResult>, TError>(result.Value.Select(func));
    }

    /// <summary>
    /// Selects the values of a successful awaited result sequence to a new type.
    /// </summary>
    /// <typeparam name="T">The type of the input values.</typeparam>
    /// <typeparam name="TResult">The type of the result values.</typeparam>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="resultTask">The result task containing a sequence of values.</param>
    /// <param name="func">The selection function.</param>
    /// <returns>A new result with selected values as a task.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result<IEnumerable<TResult>, TError>> SelectAsync<T, TResult, TError>(this Task<Result<IEnumerable<T>, TError>> resultTask, Func<T, TResult> func)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return Task.FromResult(Select(result, func));
        }

        return Impl(resultTask, func);

        static async Task<Result<IEnumerable<TResult>, TError>> Impl(Task<Result<IEnumerable<T>, TError>> resultTask, Func<T, TResult> func)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                return Result.Fail<IEnumerable<TResult>, TError>(result.Error);
            }

            return Result.Ok<IEnumerable<TResult>, TError>(result.Value.Select(func));
        }
    }

#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER

    /// <summary>
    /// Selects the values of a successful awaited result sequence to a new type using a ValueTask.
    /// </summary>
    /// <typeparam name="T">The type of the input values.</typeparam>
    /// <typeparam name="TResult">The type of the result values.</typeparam>
    /// <param name="resultTask">The result value task containing a sequence of values.</param>
    /// <param name="func">The selection function.</param>
    /// <returns>A new result with selected values as a value task.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<Result<IEnumerable<TResult>>> SelectAsync<T, TResult>(this ValueTask<Result<IEnumerable<T>>> resultTask, Func<T, TResult> func)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return AsyncHelper.CreateValueTask(Select(result, func));
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
