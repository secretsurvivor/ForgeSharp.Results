using ForgeSharp.Results.Infrastructure;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ForgeSharp.Results.Monad;

/// <summary>
/// Provides "tap" operators for <see cref="Result"/> and <see cref="Result{T}"/> which allow running
/// side-effecting actions when a result is successful while returning the original result unchanged.
/// </summary>
public static class TapExtension
{
    /// <summary>
    /// Executes an action if the result is successful.
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
    /// Executes an action if the result is successful.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
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
    /// Executes an asynchronous action if the result is successful.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="action">The asynchronous action to execute.</param>
    /// <returns>The original result as a task.</returns>
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
    /// Executes an asynchronous action if the result is successful.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="action">The asynchronous action to execute.</param>
    /// <returns>The original result as a task.</returns>
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
    /// Executes an action if the awaited result is successful.
    /// </summary>
    /// <param name="resultTask">The result task.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The original result as a task.</returns>
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
    /// Executes an action if the awaited result is successful.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The original result as a task.</returns>
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
    /// Executes an asynchronous action if the awaited result is successful.
    /// </summary>
    /// <param name="resultTask">The result task.</param>
    /// <param name="action">The asynchronous action to execute.</param>
    /// <returns>The original result as a task.</returns>
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
    /// Executes an asynchronous action if the awaited result is successful.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="action">The asynchronous action to execute.</param>
    /// <returns>The original result as a task.</returns>
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

#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER

    /// <summary>
    /// Executes an asynchronous ValueTask action if the result is successful.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="action">The asynchronous ValueTask action to execute.</param>
    /// <returns>The original result as a Task.</returns>
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
    /// Executes an asynchronous ValueTask action if the result is successful.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="action">The asynchronous ValueTask action to execute.</param>
    /// <returns>The original result as a ValueTask.</returns>
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
    /// Executes an action if the awaited ValueTask result is successful.
    /// </summary>
    /// <param name="resultTask">The ValueTask result.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The original result as a ValueTask.</returns>
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
    /// Executes an action if the awaited ValueTask result is successful.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="resultTask">The ValueTask result.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The original result as a ValueTask.</returns>
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
    /// Executes an asynchronous ValueTask action if the awaited ValueTask result is successful.
    /// </summary>
    /// <param name="resultTask">The ValueTask result.</param>
    /// <param name="action">The asynchronous ValueTask action to execute.</param>
    /// <returns>The original result as a ValueTask.</returns>
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
    /// Executes an asynchronous Task action if the awaited ValueTask result is successful.
    /// </summary>
    /// <param name="resultTask">The ValueTask result.</param>
    /// <param name="action">The asynchronous Task action to execute.</param>
    /// <returns>The original result as a Task.</returns>
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
    /// Executes an asynchronous ValueTask action if the awaited ValueTask result is successful.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="resultTask">The ValueTask result.</param>
    /// <param name="action">The asynchronous ValueTask action to execute.</param>
    /// <returns>The original result as a ValueTask.</returns>
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
    /// Executes an asynchronous Task action if the awaited ValueTask result is successful.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="resultTask">The ValueTask result.</param>
    /// <param name="action">The asynchronous Task action to execute.</param>
    /// <returns>The original result as a Task.</returns>
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
