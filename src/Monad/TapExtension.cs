using ForgeSharp.Results.Infrastructure;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ForgeSharp.Results.Monad;

/// <summary>
/// Tap operators for running side-effects on success without changing the result.
/// </summary>
public static class TapExtension
{
    #region Sync
    /// <summary>
    /// Runs a side-effect on success.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The original result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result Tap(this Result result, Action action)
    {
        if (result.IsSuccess)
        {
            action();
        }

        return result;
    }

    /// <summary>
    /// Runs a side-effect with the value on success.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The original result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> Tap<T>(this Result<T> result, Action<T> action)
    {
        if (result.IsSuccess)
        {
            action(result.Value);
        }

        return result;
    }

    /// <summary>
    /// Runs a side-effect with the value of a custom-error result on success.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The original result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T, TError> Tap<T, TError>(this Result<T, TError> result, Action<T> action)
    {
        if (result.IsSuccess)
        {
            action(result.Value);
        }

        return result;
    }
    #endregion

    #region Async Parameter
    /// <summary>
    /// Async version of the corresponding Tap overload.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="action">The asynchronous action to execute.</param>
    /// <returns>The original result.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result> TapAsync(this Result result, Func<Task> action)
    {
        if (result.IsSuccess)
        {
            await action().ConfigureAwait(false);
        }

        return result;
    }

    /// <summary>
    /// Async version of the corresponding Tap overload.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="action">The asynchronous action to execute.</param>
    /// <returns>The original result.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<T>> TapAsync<T>(this Result<T> result, Func<T, Task> action)
    {
        if (result.IsSuccess)
        {
            await action(result.Value).ConfigureAwait(false);
        }

        return result;
    }

    /// <summary>
    /// Async version of the corresponding Tap overload.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="action">The asynchronous action to execute.</param>
    /// <returns>The original result.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<T, TError>> TapAsync<T, TError>(this Result<T, TError> result, Func<T, Task> action)
    {
        if (result.IsSuccess)
        {
            await action(result.Value).ConfigureAwait(false);
        }

        return result;
    }
    #endregion

    #region Async Result
    /// <summary>
    /// Awaits the result, then taps on success.
    /// </summary>
    /// <param name="resultTask">The result task.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The original result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result> TapAsync(this Task<Result> resultTask, Action action)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return Task.FromResult(Tap(result, action));
        }

        return Impl(resultTask, action);

        static async Task<Result> Impl(Task<Result> resultTask, Action action)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (result.IsSuccess)
            {
                action();
            }

            return result;
        }
    }

    /// <summary>
    /// Awaits the result, then taps on success.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The original result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result<T>> TapAsync<T>(this Task<Result<T>> resultTask, Action<T> action)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return Task.FromResult(Tap(result, action));
        }

        return Impl(resultTask, action);

        static async Task<Result<T>> Impl(Task<Result<T>> resultTask, Action<T> action)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (result.IsSuccess)
            {
                action(result.Value);
            }

            return result;
        }
    }

    /// <summary>
    /// Awaits the result, then taps on success.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The original result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result<T, TError>> TapAsync<T, TError>(this Task<Result<T, TError>> resultTask, Action<T> action)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return Task.FromResult(Tap(result, action));
        }

        return Impl(resultTask, action);

        static async Task<Result<T, TError>> Impl(Task<Result<T, TError>> resultTask, Action<T> action)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (result.IsSuccess)
            {
                action(result.Value);
            }

            return result;
        }
    }
    #endregion

    #region Async Result and Parameter
    /// <summary>
    /// Awaits the result, then runs the async side-effect on success.
    /// </summary>
    /// <param name="resultTask">The result task.</param>
    /// <param name="action">The asynchronous action to execute.</param>
    /// <returns>The original result.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result> TapAsync(this Task<Result> resultTask, Func<Task> action)
    {
        var result = await resultTask.ConfigureAwait(false);

        if (result.IsSuccess)
        {
            await action().ConfigureAwait(false);
        }

        return result;
    }

    /// <summary>
    /// Awaits the result, then runs the async side-effect on success.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="action">The asynchronous action to execute.</param>
    /// <returns>The original result.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<T>> TapAsync<T>(this Task<Result<T>> resultTask, Func<T, Task> action)
    {
        var result = await resultTask.ConfigureAwait(false);

        if (result.IsSuccess)
        {
            await action(result.Value).ConfigureAwait(false);
        }

        return result;
    }

    /// <summary>
    /// Awaits the result, then runs the async side-effect on success.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="action">The asynchronous action to execute.</param>
    /// <returns>The original result.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<T, TError>> TapAsync<T, TError>(this Task<Result<T, TError>> resultTask, Func<T, Task> action)
    {
        var result = await resultTask.ConfigureAwait(false);

        if (result.IsSuccess)
        {
            await action(result.Value).ConfigureAwait(false);
        }

        return result;
    }
    #endregion

#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER

    /// <summary>
    /// ValueTask overload of the corresponding Tap.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="action">The asynchronous ValueTask action to execute.</param>
    /// <returns>The original result.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result> TapAsync(this Result result, Func<ValueTask> action)
    {
        if (result.IsSuccess)
        {
            await action().ConfigureAwait(false);
        }

        return result;
    }

    /// <summary>
    /// ValueTask overload of the corresponding Tap.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="action">The asynchronous ValueTask action to execute.</param>
    /// <returns>The original result.</returns>
    [DebuggerStepperBoundary]
    public static async ValueTask<Result<T>> TapAsync<T>(this Result<T> result, Func<T, ValueTask> action)
    {
        if (result.IsSuccess)
        {
            await action(result.Value).ConfigureAwait(false);
        }

        return result;
    }

    /// <summary>
    /// ValueTask overload of the corresponding Tap.
    /// </summary>
    /// <param name="resultTask">The ValueTask result.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The original result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<Result> TapAsync(this ValueTask<Result> resultTask, Action action)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return AsyncHelper.CreateValueTask(Tap(result, action));
        }

        return Impl(resultTask, action);

        static async ValueTask<Result> Impl(ValueTask<Result> resultTask, Action action)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (result.IsSuccess)
            {
                action();
            }

            return result;
        }
    }

    /// <summary>
    /// ValueTask overload of the corresponding Tap.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="resultTask">The ValueTask result.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The original result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<Result<T>> TapAsync<T>(this ValueTask<Result<T>> resultTask, Action<T> action)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return AsyncHelper.CreateValueTask(Tap(result, action));
        }

        return Impl(resultTask, action);

        static async ValueTask<Result<T>> Impl(ValueTask<Result<T>> resultTask, Action<T> action)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (result.IsSuccess)
            {
                action(result.Value);
            }

            return result;
        }
    }

    /// <summary>
    /// ValueTask overload of the corresponding Tap.
    /// </summary>
    /// <param name="resultTask">The ValueTask result.</param>
    /// <param name="action">The asynchronous ValueTask action to execute.</param>
    /// <returns>The original result.</returns>
    [DebuggerStepperBoundary]
    public static async ValueTask<Result> TapAsync(this ValueTask<Result> resultTask, Func<ValueTask> action)
    {
        var result = await resultTask.ConfigureAwait(false);

        if (result.IsSuccess)
        {
            await action().ConfigureAwait(false);
        }

        return result;
    }

    /// <summary>
    /// ValueTask overload of the corresponding Tap.
    /// </summary>
    /// <param name="resultTask">The ValueTask result.</param>
    /// <param name="action">The asynchronous Task action to execute.</param>
    /// <returns>The original result.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result> TapAsync(this ValueTask<Result> resultTask, Func<Task> action)
    {
        var result = await resultTask.ConfigureAwait(false);

        if (result.IsSuccess)
        {
            await action().ConfigureAwait(false);
        }

        return result;
    }

    /// <summary>
    /// ValueTask overload of the corresponding Tap.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="resultTask">The ValueTask result.</param>
    /// <param name="action">The asynchronous ValueTask action to execute.</param>
    /// <returns>The original result.</returns>
    [DebuggerStepperBoundary]
    public static async ValueTask<Result<T>> TapAsync<T>(this ValueTask<Result<T>> resultTask, Func<T, ValueTask> action)
    {
        var result = await resultTask.ConfigureAwait(false);

        if (result.IsSuccess)
        {
            await action(result.Value).ConfigureAwait(false);
        }

        return result;
    }

    /// <summary>
    /// ValueTask overload of the corresponding Tap.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="resultTask">The ValueTask result.</param>
    /// <param name="action">The asynchronous Task action to execute.</param>
    /// <returns>The original result.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<T>> TapAsync<T>(this ValueTask<Result<T>> resultTask, Func<T, Task> action)
    {
        var result = await resultTask.ConfigureAwait(false);

        if (result.IsSuccess)
        {
            await action(result.Value).ConfigureAwait(false);
        }

        return result;
    }

#endif
}
