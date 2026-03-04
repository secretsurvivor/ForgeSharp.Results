using ForgeSharp.Results.Infrastructure;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ForgeSharp.Results.Monad;

/// <summary>
/// TapError operators for running side-effects on failure without changing the result.
/// </summary>
public static class TapErrorExtension
{
    #region Sync
    /// <summary>
    /// Runs a side-effect on failure.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="action">The action to execute, receiving the failed result.</param>
    /// <returns>The original result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result TapError(this Result result, Action<Result> action)
    {
        if (!result.IsSuccess)
        {
            action(result);
        }

        return result;
    }

    /// <summary>
    /// Runs a side-effect on failure.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="action">The action to execute, receiving the failed result.</param>
    /// <returns>The original result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> TapError<T>(this Result<T> result, Action<Result<T>> action)
    {
        if (!result.IsSuccess)
        {
            action(result);
        }

        return result;
    }

    /// <summary>
    /// Runs a side-effect with the error on failure.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="action">The action to execute, receiving the error.</param>
    /// <returns>The original result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T, TError> TapError<T, TError>(this Result<T, TError> result, Action<TError> action)
    {
        if (!result.IsSuccess)
        {
            action(result.Error);
        }

        return result;
    }
    #endregion

    #region Async Parameter
    /// <summary>
    /// Async version of the corresponding TapError overload.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="action">The asynchronous action to execute, receiving the failed result.</param>
    /// <returns>The original result.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result> TapErrorAsync(this Result result, Func<Result, Task> action)
    {
        if (!result.IsSuccess)
        {
            await action(result).ConfigureAwait(false);
        }

        return result;
    }

    /// <summary>
    /// Async version of the corresponding TapError overload.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="action">The asynchronous action to execute, receiving the failed result.</param>
    /// <returns>The original result.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<T>> TapErrorAsync<T>(this Result<T> result, Func<Result<T>, Task> action)
    {
        if (!result.IsSuccess)
        {
            await action(result).ConfigureAwait(false);
        }

        return result;
    }

    /// <summary>
    /// Async version of the corresponding TapError overload.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="action">The asynchronous action to execute, receiving the error.</param>
    /// <returns>The original result.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<T, TError>> TapErrorAsync<T, TError>(this Result<T, TError> result, Func<TError, Task> action)
    {
        if (!result.IsSuccess)
        {
            await action(result.Error).ConfigureAwait(false);
        }

        return result;
    }
    #endregion

    #region Async Result
    /// <summary>
    /// Awaits the result, then taps on failure.
    /// </summary>
    /// <param name="resultTask">The result task.</param>
    /// <param name="action">The action to execute, receiving the failed result.</param>
    /// <returns>The original result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result> TapErrorAsync(this Task<Result> resultTask, Action<Result> action)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return Task.FromResult(TapError(result, action));
        }

        return Impl(resultTask, action);

        static async Task<Result> Impl(Task<Result> resultTask, Action<Result> action)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                action(result);
            }

            return result;
        }
    }

    /// <summary>
    /// Awaits the result, then taps on failure.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="action">The action to execute, receiving the failed result.</param>
    /// <returns>The original result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result<T>> TapErrorAsync<T>(this Task<Result<T>> resultTask, Action<Result<T>> action)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return Task.FromResult(TapError(result, action));
        }

        return Impl(resultTask, action);

        static async Task<Result<T>> Impl(Task<Result<T>> resultTask, Action<Result<T>> action)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                action(result);
            }

            return result;
        }
    }

    /// <summary>
    /// Awaits the result, then taps on failure.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="action">The action to execute, receiving the error.</param>
    /// <returns>The original result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result<T, TError>> TapErrorAsync<T, TError>(this Task<Result<T, TError>> resultTask, Action<TError> action)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return Task.FromResult(TapError(result, action));
        }

        return Impl(resultTask, action);

        static async Task<Result<T, TError>> Impl(Task<Result<T, TError>> resultTask, Action<TError> action)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                action(result.Error);
            }

            return result;
        }
    }
    #endregion

    #region Async Result and Parameter
    /// <summary>
    /// Awaits the result, then runs the async side-effect on failure.
    /// </summary>
    /// <param name="resultTask">The result task.</param>
    /// <param name="action">The asynchronous action to execute, receiving the failed result.</param>
    /// <returns>The original result.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result> TapErrorAsync(this Task<Result> resultTask, Func<Result, Task> action)
    {
        var result = await resultTask.ConfigureAwait(false);

        if (!result.IsSuccess)
        {
            await action(result).ConfigureAwait(false);
        }

        return result;
    }

    /// <summary>
    /// Awaits the result, then runs the async side-effect on failure.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="action">The asynchronous action to execute, receiving the failed result.</param>
    /// <returns>The original result.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<T>> TapErrorAsync<T>(this Task<Result<T>> resultTask, Func<Result<T>, Task> action)
    {
        var result = await resultTask.ConfigureAwait(false);

        if (!result.IsSuccess)
        {
            await action(result).ConfigureAwait(false);
        }

        return result;
    }

    /// <summary>
    /// Awaits the result, then runs the async side-effect on failure.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="action">The asynchronous action to execute, receiving the error.</param>
    /// <returns>The original result.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<T, TError>> TapErrorAsync<T, TError>(this Task<Result<T, TError>> resultTask, Func<TError, Task> action)
    {
        var result = await resultTask.ConfigureAwait(false);

        if (!result.IsSuccess)
        {
            await action(result.Error).ConfigureAwait(false);
        }

        return result;
    }
    #endregion

#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER

    /// <summary>
    /// ValueTask overload of the corresponding TapError.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="action">The asynchronous ValueTask action to execute, receiving the failed result.</param>
    /// <returns>The original result.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result> TapErrorAsync(this Result result, Func<Result, ValueTask> action)
    {
        if (!result.IsSuccess)
        {
            await action(result).ConfigureAwait(false);
        }

        return result;
    }

    /// <summary>
    /// ValueTask overload of the corresponding TapError.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="action">The asynchronous ValueTask action to execute, receiving the failed result.</param>
    /// <returns>The original result.</returns>
    [DebuggerStepperBoundary]
    public static async ValueTask<Result<T>> TapErrorAsync<T>(this Result<T> result, Func<Result<T>, ValueTask> action)
    {
        if (!result.IsSuccess)
        {
            await action(result).ConfigureAwait(false);
        }

        return result;
    }

    /// <summary>
    /// ValueTask overload of the corresponding TapError.
    /// </summary>
    /// <param name="resultTask">The ValueTask result.</param>
    /// <param name="action">The action to execute, receiving the failed result.</param>
    /// <returns>The original result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<Result> TapErrorAsync(this ValueTask<Result> resultTask, Action<Result> action)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return AsyncHelper.CreateValueTask(TapError(result, action));
        }

        return Impl(resultTask, action);

        static async ValueTask<Result> Impl(ValueTask<Result> resultTask, Action<Result> action)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                action(result);
            }

            return result;
        }
    }

    /// <summary>
    /// ValueTask overload of the corresponding TapError.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="resultTask">The ValueTask result.</param>
    /// <param name="action">The action to execute, receiving the failed result.</param>
    /// <returns>The original result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<Result<T>> TapErrorAsync<T>(this ValueTask<Result<T>> resultTask, Action<Result<T>> action)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return AsyncHelper.CreateValueTask(TapError(result, action));
        }

        return Impl(resultTask, action);

        static async ValueTask<Result<T>> Impl(ValueTask<Result<T>> resultTask, Action<Result<T>> action)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                action(result);
            }

            return result;
        }
    }

    /// <summary>
    /// ValueTask overload of the corresponding TapError.
    /// </summary>
    /// <param name="resultTask">The ValueTask result.</param>
    /// <param name="action">The asynchronous ValueTask action to execute, receiving the failed result.</param>
    /// <returns>The original result.</returns>
    [DebuggerStepperBoundary]
    public static async ValueTask<Result> TapErrorAsync(this ValueTask<Result> resultTask, Func<Result, ValueTask> action)
    {
        var result = await resultTask.ConfigureAwait(false);

        if (!result.IsSuccess)
        {
            await action(result).ConfigureAwait(false);
        }

        return result;
    }

    /// <summary>
    /// ValueTask overload of the corresponding TapError.
    /// </summary>
    /// <param name="resultTask">The ValueTask result.</param>
    /// <param name="action">The asynchronous Task action to execute, receiving the failed result.</param>
    /// <returns>The original result.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result> TapErrorAsync(this ValueTask<Result> resultTask, Func<Result, Task> action)
    {
        var result = await resultTask.ConfigureAwait(false);

        if (!result.IsSuccess)
        {
            await action(result).ConfigureAwait(false);
        }

        return result;
    }

    /// <summary>
    /// ValueTask overload of the corresponding TapError.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="resultTask">The ValueTask result.</param>
    /// <param name="action">The asynchronous ValueTask action to execute, receiving the failed result.</param>
    /// <returns>The original result.</returns>
    [DebuggerStepperBoundary]
    public static async ValueTask<Result<T>> TapErrorAsync<T>(this ValueTask<Result<T>> resultTask, Func<Result<T>, ValueTask> action)
    {
        var result = await resultTask.ConfigureAwait(false);

        if (!result.IsSuccess)
        {
            await action(result).ConfigureAwait(false);
        }

        return result;
    }

    /// <summary>
    /// ValueTask overload of the corresponding TapError.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="resultTask">The ValueTask result.</param>
    /// <param name="action">The asynchronous Task action to execute, receiving the failed result.</param>
    /// <returns>The original result.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<T>> TapErrorAsync<T>(this ValueTask<Result<T>> resultTask, Func<Result<T>, Task> action)
    {
        var result = await resultTask.ConfigureAwait(false);

        if (!result.IsSuccess)
        {
            await action(result).ConfigureAwait(false);
        }

        return result;
    }

#endif
}
