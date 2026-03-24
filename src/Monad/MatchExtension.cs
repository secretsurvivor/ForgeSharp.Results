using ForgeSharp.Results.Infrastructure;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ForgeSharp.Results.Monad;

/// <summary>
/// Match (fold) operators for exhaustively handling success and failure branches of a result.
/// </summary>
public static class MatchExtension
{
    #region Sync
    /// <summary>
    /// Matches a non-generic result, invoking the appropriate branch.
    /// </summary>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="onSuccess">The function to invoke on success.</param>
    /// <param name="onFailure">The function to invoke on failure, receiving the failed result.</param>
    /// <returns>The value produced by the matched branch.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult Match<TResult>(this Result result, Func<TResult> onSuccess, Func<Result, TResult> onFailure)
    {
        if (!result.IsSuccess)
        {
            return onFailure(result);
        }

        return onSuccess();
    }

    /// <summary>
    /// Matches a typed result, invoking the appropriate branch.
    /// </summary>
    /// <typeparam name="T">The input value type.</typeparam>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="onSuccess">The function to invoke on success, receiving the value.</param>
    /// <param name="onFailure">The function to invoke on failure, receiving the failed result.</param>
    /// <returns>The value produced by the matched branch.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult Match<T, TResult>(this Result<T> result, Func<T, TResult> onSuccess, Func<Result, TResult> onFailure)
    {
        if (!result.IsSuccess)
        {
            return onFailure((Result) result);
        }

        return onSuccess(result.Value);
    }

    /// <summary>
    /// Matches a discriminated result, invoking the appropriate branch.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="onSuccess">The function to invoke on success, receiving the value.</param>
    /// <param name="onFailure">The function to invoke on failure, receiving the error.</param>
    /// <returns>The value produced by the matched branch.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult Match<TValue, TResult, TError>(this Result<TValue, TError> result, Func<TValue, TResult> onSuccess, Func<TError, TResult> onFailure)
    {
        if (!result.IsSuccess)
        {
            return onFailure(result.Error);
        }

        return onSuccess(result.Value);
    }
    #endregion

    #region Task Parameter
    /// <summary>
    /// Async version of the corresponding Match overload.
    /// </summary>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="onSuccess">The asynchronous function to invoke on success.</param>
    /// <param name="onFailure">The asynchronous function to invoke on failure, receiving the failed result.</param>
    /// <returns>The value produced by the matched branch.</returns>
    [DebuggerStepperBoundary]
    public static async Task<TResult> MatchAsync<TResult>(this Result result, Func<Task<TResult>> onSuccess, Func<Result, Task<TResult>> onFailure)
    {
        if (!result.IsSuccess)
        {
            return await onFailure(result).ConfigureAwait(false);
        }

        return await onSuccess().ConfigureAwait(false);
    }

    /// <summary>
    /// Async version of the corresponding Match overload.
    /// </summary>
    /// <typeparam name="T">The input value type.</typeparam>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="onSuccess">The asynchronous function to invoke on success, receiving the value.</param>
    /// <param name="onFailure">The asynchronous function to invoke on failure, receiving the failed result.</param>
    /// <returns>The value produced by the matched branch.</returns>
    [DebuggerStepperBoundary]
    public static async Task<TResult> MatchAsync<T, TResult>(this Result<T> result, Func<T, Task<TResult>> onSuccess, Func<Result, Task<TResult>> onFailure)
    {
        if (!result.IsSuccess)
        {
            return await onFailure((Result) result).ConfigureAwait(false);
        }

        return await onSuccess(result.Value).ConfigureAwait(false);
    }

    /// <summary>
    /// Async version of the corresponding Match overload.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="onSuccess">The asynchronous function to invoke on success, receiving the value.</param>
    /// <param name="onFailure">The asynchronous function to invoke on failure, receiving the error.</param>
    /// <returns>The value produced by the matched branch.</returns>
    [DebuggerStepperBoundary]
    public static async Task<TResult> MatchAsync<TValue, TResult, TError>(this Result<TValue, TError> result, Func<TValue, Task<TResult>> onSuccess, Func<TError, Task<TResult>> onFailure)
    {
        if (!result.IsSuccess)
        {
            return await onFailure(result.Error).ConfigureAwait(false);
        }

        return await onSuccess(result.Value).ConfigureAwait(false);
    }
    #endregion

    #region Task Result
    /// <summary>
    /// Awaits the result, then matches with sync branches.
    /// </summary>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="onSuccess">The function to invoke on success.</param>
    /// <param name="onFailure">The function to invoke on failure, receiving the failed result.</param>
    /// <returns>The value produced by the matched branch.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<TResult> MatchAsync<TResult>(this Task<Result> resultTask, Func<TResult> onSuccess, Func<Result, TResult> onFailure)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return Task.FromResult(Match(result, onSuccess, onFailure));
        }

        return Impl(resultTask, onSuccess, onFailure);

        static async Task<TResult> Impl(Task<Result> resultTask, Func<TResult> onSuccess, Func<Result, TResult> onFailure)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                return onFailure(result);
            }

            return onSuccess();
        }
    }

    /// <summary>
    /// Awaits the result, then matches with sync branches.
    /// </summary>
    /// <typeparam name="T">The input value type.</typeparam>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="onSuccess">The function to invoke on success, receiving the value.</param>
    /// <param name="onFailure">The function to invoke on failure, receiving the failed result.</param>
    /// <returns>The value produced by the matched branch.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<TResult> MatchAsync<T, TResult>(this Task<Result<T>> resultTask, Func<T, TResult> onSuccess, Func<Result, TResult> onFailure)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return Task.FromResult(Match(result, onSuccess, onFailure));
        }

        return Impl(resultTask, onSuccess, onFailure);

        static async Task<TResult> Impl(Task<Result<T>> resultTask, Func<T, TResult> onSuccess, Func<Result, TResult> onFailure)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                return onFailure((Result) result);
            }

            return onSuccess(result.Value);
        }
    }

    /// <summary>
    /// Awaits the result, then matches with sync branches.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="onSuccess">The function to invoke on success, receiving the value.</param>
    /// <param name="onFailure">The function to invoke on failure, receiving the error.</param>
    /// <returns>The value produced by the matched branch.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<TResult> MatchAsync<TValue, TResult, TError>(this Task<Result<TValue, TError>> resultTask, Func<TValue, TResult> onSuccess, Func<TError, TResult> onFailure)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return Task.FromResult(Match(result, onSuccess, onFailure));
        }

        return Impl(resultTask, onSuccess, onFailure);

        static async Task<TResult> Impl(Task<Result<TValue, TError>> resultTask, Func<TValue, TResult> onSuccess, Func<TError, TResult> onFailure)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                return onFailure(result.Error);
            }

            return onSuccess(result.Value);
        }
    }
    #endregion

    #region Task Result and Parameter
    /// <summary>
    /// Awaits the result, then matches with async branches.
    /// </summary>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="onSuccess">The asynchronous function to invoke on success.</param>
    /// <param name="onFailure">The asynchronous function to invoke on failure, receiving the failed result.</param>
    /// <returns>The value produced by the matched branch.</returns>
    [DebuggerStepperBoundary]
    public static async Task<TResult> MatchAsync<TResult>(this Task<Result> resultTask, Func<Task<TResult>> onSuccess, Func<Result, Task<TResult>> onFailure)
    {
        var result = await resultTask.ConfigureAwait(false);

        if (!result.IsSuccess)
        {
            return await onFailure(result).ConfigureAwait(false);
        }

        return await onSuccess().ConfigureAwait(false);
    }

    /// <summary>
    /// Awaits the result, then matches with async branches.
    /// </summary>
    /// <typeparam name="T">The input value type.</typeparam>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="onSuccess">The asynchronous function to invoke on success, receiving the value.</param>
    /// <param name="onFailure">The asynchronous function to invoke on failure, receiving the failed result.</param>
    /// <returns>The value produced by the matched branch.</returns>
    [DebuggerStepperBoundary]
    public static async Task<TResult> MatchAsync<T, TResult>(this Task<Result<T>> resultTask, Func<T, Task<TResult>> onSuccess, Func<Result, Task<TResult>> onFailure)
    {
        var result = await resultTask.ConfigureAwait(false);

        if (!result.IsSuccess)
        {
            return await onFailure((Result) result).ConfigureAwait(false);
        }

        return await onSuccess(result.Value).ConfigureAwait(false);
    }

    /// <summary>
    /// Awaits the result, then matches with async branches.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="onSuccess">The asynchronous function to invoke on success, receiving the value.</param>
    /// <param name="onFailure">The asynchronous function to invoke on failure, receiving the error.</param>
    /// <returns>The value produced by the matched branch.</returns>
    [DebuggerStepperBoundary]
    public static async Task<TResult> MatchAsync<TValue, TResult, TError>(this Task<Result<TValue, TError>> resultTask, Func<TValue, Task<TResult>> onSuccess, Func<TError, Task<TResult>> onFailure)
    {
        var result = await resultTask.ConfigureAwait(false);

        if (!result.IsSuccess)
        {
            return await onFailure(result.Error).ConfigureAwait(false);
        }

        return await onSuccess(result.Value).ConfigureAwait(false);
    }
    #endregion

#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER

    /// <summary>
    /// ValueTask overload of the corresponding Match.
    /// </summary>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="onSuccess">The asynchronous function to invoke on success.</param>
    /// <param name="onFailure">The asynchronous function to invoke on failure, receiving the failed result.</param>
    /// <returns>The value produced by the matched branch.</returns>
    [DebuggerStepperBoundary]
    public static async Task<TResult> MatchAsync<TResult>(this Result result, Func<ValueTask<TResult>> onSuccess, Func<Result, ValueTask<TResult>> onFailure)
    {
        if (!result.IsSuccess)
        {
            return await onFailure(result).ConfigureAwait(false);
        }

        return await onSuccess().ConfigureAwait(false);
    }

    /// <summary>
    /// ValueTask overload of the corresponding Match.
    /// </summary>
    /// <typeparam name="T">The input value type.</typeparam>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="onSuccess">The asynchronous function to invoke on success, receiving the value.</param>
    /// <param name="onFailure">The asynchronous function to invoke on failure, receiving the failed result.</param>
    /// <returns>The value produced by the matched branch.</returns>
    [DebuggerStepperBoundary]
    public static async Task<TResult> MatchAsync<T, TResult>(this Result<T> result, Func<T, ValueTask<TResult>> onSuccess, Func<Result, ValueTask<TResult>> onFailure)
    {
        if (!result.IsSuccess)
        {
            return await onFailure((Result) result).ConfigureAwait(false);
        }

        return await onSuccess(result.Value).ConfigureAwait(false);
    }

    /// <summary>
    /// ValueTask overload: awaits the result, then matches with sync branches.
    /// </summary>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="onSuccess">The function to invoke on success.</param>
    /// <param name="onFailure">The function to invoke on failure, receiving the failed result.</param>
    /// <returns>The value produced by the matched branch.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<TResult> MatchAsync<TResult>(this ValueTask<Result> resultTask, Func<TResult> onSuccess, Func<Result, TResult> onFailure)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return AsyncHelper.CreateValueTask(Match(result, onSuccess, onFailure));
        }

        return Impl(resultTask, onSuccess, onFailure);

        static async ValueTask<TResult> Impl(ValueTask<Result> resultTask, Func<TResult> onSuccess, Func<Result, TResult> onFailure)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                return onFailure(result);
            }

            return onSuccess();
        }
    }

    /// <summary>
    /// ValueTask overload: awaits the result, then matches with sync branches.
    /// </summary>
    /// <typeparam name="T">The input value type.</typeparam>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="onSuccess">The function to invoke on success, receiving the value.</param>
    /// <param name="onFailure">The function to invoke on failure, receiving the failed result.</param>
    /// <returns>The value produced by the matched branch.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<TResult> MatchAsync<T, TResult>(this ValueTask<Result<T>> resultTask, Func<T, TResult> onSuccess, Func<Result, TResult> onFailure)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return AsyncHelper.CreateValueTask(Match(result, onSuccess, onFailure));
        }

        return Impl(resultTask, onSuccess, onFailure);

        static async ValueTask<TResult> Impl(ValueTask<Result<T>> resultTask, Func<T, TResult> onSuccess, Func<Result, TResult> onFailure)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                return onFailure((Result) result);
            }

            return onSuccess(result.Value);
        }
    }

    /// <summary>
    /// ValueTask overload: awaits the result, then matches with async ValueTask branches.
    /// </summary>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="onSuccess">The asynchronous function to invoke on success.</param>
    /// <param name="onFailure">The asynchronous function to invoke on failure, receiving the failed result.</param>
    /// <returns>The value produced by the matched branch.</returns>
    [DebuggerStepperBoundary]
    public static async ValueTask<TResult> MatchAsync<TResult>(this ValueTask<Result> resultTask, Func<ValueTask<TResult>> onSuccess, Func<Result, ValueTask<TResult>> onFailure)
    {
        var result = await resultTask.ConfigureAwait(false);

        if (!result.IsSuccess)
        {
            return await onFailure(result).ConfigureAwait(false);
        }

        return await onSuccess().ConfigureAwait(false);
    }

    /// <summary>
    /// ValueTask overload: awaits the result, then matches with async ValueTask branches.
    /// </summary>
    /// <typeparam name="T">The input value type.</typeparam>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="onSuccess">The asynchronous function to invoke on success, receiving the value.</param>
    /// <param name="onFailure">The asynchronous function to invoke on failure, receiving the failed result.</param>
    /// <returns>The value produced by the matched branch.</returns>
    [DebuggerStepperBoundary]
    public static async ValueTask<TResult> MatchAsync<T, TResult>(this ValueTask<Result<T>> resultTask, Func<T, ValueTask<TResult>> onSuccess, Func<Result, ValueTask<TResult>> onFailure)
    {
        var result = await resultTask.ConfigureAwait(false);

        if (!result.IsSuccess)
        {
            return await onFailure((Result) result).ConfigureAwait(false);
        }

        return await onSuccess(result.Value).ConfigureAwait(false);
    }

    /// <summary>
    /// ValueTask overload: awaits the result, then matches with async Task branches.
    /// </summary>
    /// <typeparam name="T">The input value type.</typeparam>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="onSuccess">The asynchronous function to invoke on success, receiving the value.</param>
    /// <param name="onFailure">The asynchronous function to invoke on failure, receiving the failed result.</param>
    /// <returns>The value produced by the matched branch.</returns>
    [DebuggerStepperBoundary]
    public static async Task<TResult> MatchAsync<T, TResult>(this ValueTask<Result<T>> resultTask, Func<T, Task<TResult>> onSuccess, Func<Result, Task<TResult>> onFailure)
    {
        var result = await resultTask.ConfigureAwait(false);

        if (!result.IsSuccess)
        {
            return await onFailure((Result) result).ConfigureAwait(false);
        }

        return await onSuccess(result.Value).ConfigureAwait(false);
    }

    /// <summary>
    /// ValueTask overload: awaits the result, then matches with async Task branches.
    /// </summary>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="onSuccess">The asynchronous function to invoke on success.</param>
    /// <param name="onFailure">The asynchronous function to invoke on failure, receiving the failed result.</param>
    /// <returns>The value produced by the matched branch.</returns>
    [DebuggerStepperBoundary]
    public static async Task<TResult> MatchAsync<TResult>(this ValueTask<Result> resultTask, Func<Task<TResult>> onSuccess, Func<Result, Task<TResult>> onFailure)
    {
        var result = await resultTask.ConfigureAwait(false);

        if (!result.IsSuccess)
        {
            return await onFailure(result).ConfigureAwait(false);
        }

        return await onSuccess().ConfigureAwait(false);
    }

#endif
}
