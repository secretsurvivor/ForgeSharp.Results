using ForgeSharp.Results.Infrastructure;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ForgeSharp.Results.Monad;

/// <summary>
/// Map operators for transforming result values, forwarding failures unchanged.
/// </summary>
public static class MapExtension
{
    #region Sync
    /// <summary>
    /// Maps a non-generic success into a typed result.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="func">The function to execute.</param>
    /// <returns>The mapped result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<TResult> Map<TResult>(this Result result, Func<TResult> func)
    {
        if (result.IsSuccess)
        {
            return Result.Ok(func());
        }

        return Result.ForwardFail<TResult>(result);
    }

    /// <summary>
    /// Transforms the value on success, forwarding failures.
    /// </summary>
    /// <typeparam name="T">The input type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="func">The function to execute.</param>
    /// <returns>The mapped result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<TResult> Map<T, TResult>(this Result<T> result, Func<T, TResult> func)
    {
        if (result.IsSuccess)
        {
            return Result.Ok(func(result.Value));
        }

        return Result.ForwardFail<TResult>(result);
    }

    /// <summary>
    /// Transforms the value of a custom-error result on success.
    /// </summary>
    /// <typeparam name="T">The input type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="action">The function to execute.</param>
    /// <returns>The mapped result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<TResult, TError> Map<T, TResult, TError>(this Result<T, TError> result, Func<T, TResult> action)
    {
        if (!result.IsSuccess)
        {
            return Result.Fail<TResult, TError>(result.Error);
        }

        return Result.Ok<TResult, TError>(action(result.Value));
    }
    #endregion

    #region Task Parameter
    /// <summary>
    /// Async version of the corresponding Map overload.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="func">The asynchronous function to execute.</param>
    /// <returns>The mapped result.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<TResult>> MapAsync<TResult>(this Result result, Func<Task<TResult>> func)
    {
        if (result.IsSuccess)
        {
            return Result.Ok(await func().ConfigureAwait(false));
        }

        return Result.ForwardFail<TResult>(result);
    }

    /// <summary>
    /// Async version of the corresponding Map overload.
    /// </summary>
    /// <typeparam name="T">The input type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="func">The asynchronous function to execute.</param>
    /// <returns>The mapped result.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<TResult>> MapAsync<T, TResult>(this Result<T> result, Func<T, Task<TResult>> func)
    {
        if (result.IsSuccess)
        {
            return Result.Ok(await func(result.Value).ConfigureAwait(false));
        }

        return Result.ForwardFail<TResult>(result);
    }

    /// <summary>
    /// Async version of the corresponding Map overload.
    /// </summary>
    /// <typeparam name="T">The input type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="func">The asynchronous function to execute.</param>
    /// <returns>The mapped result.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<TResult, TError>> MapAsync<T, TResult, TError>(this Result<T, TError> result, Func<T, Task<TResult>> func)
    {
        if (!result.IsSuccess)
        {
            return Result.Fail<TResult, TError>(result.Error);
        }

        return Result.Ok<TResult, TError>(await func(result.Value));
    }
    #endregion

    #region Task Result
    /// <summary>
    /// Awaits the result, then maps on success.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="func">The function to execute.</param>
    /// <returns>The mapped result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result<TResult>> MapAsync<TResult>(this Task<Result> resultTask, Func<TResult> func)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return Task.FromResult(Map(result, func));
        }

        return Impl(resultTask, func);

        static async Task<Result<TResult>> Impl(Task<Result> resultTask, Func<TResult> func)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (result.IsSuccess)
            {
                return Result.Ok(func());
            }

            return Result.ForwardFail<TResult>(result);
        }
    }

    /// <summary>
    /// Awaits the result, then maps on success.
    /// </summary>
    /// <typeparam name="T">The input type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="func">The function to execute.</param>
    /// <returns>The mapped result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result<TResult>> MapAsync<T, TResult>(this Task<Result<T>> resultTask, Func<T, TResult> func)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return Task.FromResult(Map(result, func));
        }

        return Impl(resultTask, func);

        static async Task<Result<TResult>> Impl(Task<Result<T>> resultTask, Func<T, TResult> func)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (result.IsSuccess)
            {
                return Result.Ok(func(result.Value));
            }

            return Result.ForwardFail<TResult>(result);
        }
    }

    /// <summary>
    /// Awaits the result, then maps on success.
    /// </summary>
    /// <typeparam name="T">The input type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="func">The function to execute.</param>
    /// <returns>The mapped result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result<TResult, TError>> MapAsync<T, TResult, TError>(this Task<Result<T, TError>> resultTask, Func<T, TResult> func)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return Task.FromResult(Map(result, func));
        }

        return Impl(resultTask, func);

        static async Task<Result<TResult, TError>> Impl(Task<Result<T, TError>> resultTask, Func<T, TResult> func)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                return Result.Fail<TResult, TError>(result.Error);
            }

            return Result.Ok<TResult, TError>(func(result.Value));
        }
    }
    #endregion

    #region Task Result and Parameter
    /// <summary>
    /// Awaits the result, then runs the async function on success.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="func">The asynchronous function to execute.</param>
    /// <returns>The mapped result.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<TResult>> MapAsync<TResult>(this Task<Result> resultTask, Func<Task<TResult>> func)
    {
        var result = await resultTask.ConfigureAwait(false);

        if (result.IsSuccess)
        {
            return Result.Ok(await func());
        }

        return Result.ForwardFail<TResult>(result);
    }

    /// <summary>
    /// Awaits the result, then runs the async function on success.
    /// </summary>
    /// <typeparam name="T">The input type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="func">The asynchronous function to execute.</param>
    /// <returns>The mapped result.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<TResult>> MapAsync<T, TResult>(this Task<Result<T>> resultTask, Func<T, Task<TResult>> func)
    {
        var result = await resultTask.ConfigureAwait(false);

        if (result.IsSuccess)
        {
            return Result.Ok(await func(result.Value).ConfigureAwait(false));
        }

        return Result.ForwardFail<TResult>(result);
    }

    /// <summary>
    /// Awaits the result, then runs the async function on success.
    /// </summary>
    /// <typeparam name="T">The input type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="func">The asynchronous function to execute.</param>
    /// <returns>The mapped result.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<TResult, TError>> MapAsync<T, TResult, TError>(this Task<Result<T, TError>> resultTask, Func<T, Task<TResult>> func)
    {
        var result = await resultTask.ConfigureAwait(false);

        if (!result.IsSuccess)
        {
            return Result.Fail<TResult, TError>(result.Error);
        }

        return Result.Ok<TResult, TError>(await func(result.Value));
    }
    #endregion

#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER

    /// <summary>
    /// ValueTask overload of the corresponding Map.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="func">The asynchronous function to execute.</param>
    /// <returns>The mapped result.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<TResult>> MapAsync<TResult>(this Result result, Func<ValueTask<TResult>> func)
    {
        if (result.IsSuccess)
        {
            return Result.Ok(await func().ConfigureAwait(false));
        }

        return Result.ForwardFail<TResult>(result);
    }

    /// <summary>
    /// ValueTask overload of the corresponding Map.
    /// </summary>
    /// <typeparam name="T">The input type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="func">The asynchronous function to execute.</param>
    /// <returns>The mapped result.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<TResult>> MapAsync<T, TResult>(this Result<T> result, Func<T, ValueTask<TResult>> func)
    {
        if (result.IsSuccess)
        {
            return Result.Ok(await func(result.Value).ConfigureAwait(false));
        }

        return Result.ForwardFail<TResult>(result);
    }

    /// <summary>
    /// ValueTask overload: awaits the result, then maps on success.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="func">The function to execute.</param>
    /// <returns>The mapped result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<Result<TResult>> MapAsync<TResult>(this ValueTask<Result> resultTask, Func<TResult> func)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return AsyncHelper.CreateValueTask(Map(result, func));
        }

        return Impl(resultTask, func);

        static async ValueTask<Result<TResult>> Impl(ValueTask<Result> resultTask, Func<TResult> func)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (result.IsSuccess)
            {
                return Result.Ok(func());
            }

            return Result.ForwardFail<TResult>(result);
        }
    }

    /// <summary>
    /// ValueTask overload: awaits the result, then maps on success.
    /// </summary>
    /// <typeparam name="T">The input type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="func">The function to execute.</param>
    /// <returns>The mapped result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<Result<TResult>> MapAsync<T, TResult>(this ValueTask<Result<T>> resultTask, Func<T, TResult> func)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return AsyncHelper.CreateValueTask(Map(result, func));
        }

        return Impl(resultTask, func);

        static async ValueTask<Result<TResult>> Impl(ValueTask<Result<T>> resultTask, Func<T, TResult> func)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (result.IsSuccess)
            {
                return Result.Ok(func(result.Value));
            }

            return Result.ForwardFail<TResult>(result);
        }
    }

    /// <summary>
    /// ValueTask overload: awaits the result, then runs the async function on success.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="func">The asynchronous function to execute.</param>
    /// <returns>The mapped result.</returns>
    [DebuggerStepperBoundary]
    public static async ValueTask<Result<TResult>> MapAsync<TResult>(this ValueTask<Result> resultTask, Func<ValueTask<TResult>> func)
    {
        var result = await resultTask.ConfigureAwait(false);

        if (result.IsSuccess)
        {
            return Result.Ok(await func().ConfigureAwait(false));
        }

        return Result.ForwardFail<TResult>(result);
    }

    /// <summary>
    /// ValueTask overload: awaits the result, then runs the async function on success.
    /// </summary>
    /// <typeparam name="T">The input type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="func">The asynchronous function to execute.</param>
    /// <returns>The mapped result.</returns>
    [DebuggerStepperBoundary]
    public static async ValueTask<Result<TResult>> MapAsync<T, TResult>(this ValueTask<Result<T>> resultTask, Func<T, ValueTask<TResult>> func)
    {
        var result = await resultTask.ConfigureAwait(false);

        if (result.IsSuccess)
        {
            return Result.Ok(await func(result.Value).ConfigureAwait(false));
        }

        return Result.ForwardFail<TResult>(result);
    }

    /// <summary>
    /// ValueTask overload: awaits the result, then runs the async Task function on success.
    /// </summary>
    /// <typeparam name="T">The input type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="func">The asynchronous function to execute.</param>
    /// <returns>The mapped result.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<TResult>> MapAsync<T, TResult>(this ValueTask<Result<T>> resultTask, Func<T, Task<TResult>> func)
    {
        var result = await resultTask.ConfigureAwait(false);

        if (result.IsSuccess)
        {
            return Result.Ok(await func(result.Value).ConfigureAwait(false));
        }

        return Result.ForwardFail<TResult>(result);
    }

    /// <summary>
    /// ValueTask overload: awaits the result, then runs the async Task function on success.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="func">The asynchronous function to execute.</param>
    /// <returns>The mapped result.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<TResult>> MapAsync<TResult>(this ValueTask<Result> resultTask, Func<Task<TResult>> func)
    {
        var result = await resultTask.ConfigureAwait(false);

        if (result.IsSuccess)
        {
            return Result.Ok(await func().ConfigureAwait(false));
        }

        return Result.ForwardFail<TResult>(result);
    }

#endif
}
