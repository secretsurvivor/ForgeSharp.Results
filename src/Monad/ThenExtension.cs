using ForgeSharp.Results.Infrastructure;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ForgeSharp.Results.Monad;

/// <summary>
/// Provides chaining operators that execute transformations when a <see cref="Result"/> or <see cref="Result{T}"/> is successful.
/// These methods return new <see cref="Result"/> instances while forwarding failures unchanged.
/// </summary>
public static class ThenExtension
{
    /// <summary>
    /// Executes a function if the result is successful, otherwise forwards the failure.
    /// </summary>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="func">The function to execute.</param>
    /// <returns>A new result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<TResult> Then<TResult>(this Result result, Func<TResult> func)
    {
        if (result.IsSuccess)
        {
            return Result.Ok(func());
        }

        return Result.ForwardFail<TResult>(result);
    }

    /// <summary>
    /// Executes a function if the result is successful, otherwise forwards the failure.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="func">The function to execute.</param>
    /// <returns>A new result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<TResult> Then<T, TResult>(this Result<T> result, Func<T, TResult> func)
    {
        if (result.IsSuccess)
        {
            return Result.Ok(func(result.Value));
        }

        return Result.ForwardFail<TResult>(result);
    }

    /// <summary>
    /// Executes an asynchronous function if the result is successful, otherwise forwards the failure.
    /// </summary>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="func">The asynchronous function to execute.</param>
    /// <returns>A task representing the asynchronous operation, with a new result.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<TResult>> ThenAsync<TResult>(this Result result, Func<Task<TResult>> func)
    {
        if (result.IsSuccess)
        {
            return Result.Ok(await func().ConfigureAwait(false));
        }

        return Result.ForwardFail<TResult>(result);
    }

    /// <summary>
    /// Executes an asynchronous function if the result is successful, otherwise forwards the failure.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="func">The asynchronous function to execute.</param>
    /// <returns>A task representing the asynchronous operation, with a new result.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<TResult>> ThenAsync<T, TResult>(this Result<T> result, Func<T, Task<TResult>> func)
    {
        if (result.IsSuccess)
        {
            return Result.Ok(await func(result.Value).ConfigureAwait(false));
        }

        return Result.ForwardFail<TResult>(result);
    }

    /// <summary>
    /// Executes a function if the awaited result is successful, otherwise forwards the failure.
    /// </summary>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="func">The function to execute.</param>
    /// <returns>A task representing the asynchronous operation, with a new result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result<TResult>> ThenAsync<TResult>(this Task<Result> resultTask, Func<TResult> func)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return Task.FromResult(Then(result, func));
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
    /// Executes a function if the awaited result is successful, otherwise forwards the failure.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="func">The function to execute.</param>
    /// <returns>A task representing the asynchronous operation, with a new result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result<TResult>> ThenAsync<T, TResult>(this Task<Result<T>> resultTask, Func<T, TResult> func)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return Task.FromResult(Then(result, func));
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
    /// Executes an asynchronous function if the awaited result is successful, otherwise forwards the failure.
    /// </summary>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="func">The asynchronous function to execute.</param>
    /// <returns>A task representing the asynchronous operation, with a new result.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<TResult>> ThenAsync<TResult>(this Task<Result> resultTask, Func<Task<TResult>> func)
    {
        var result = await resultTask.ConfigureAwait(false);

        if (result.IsSuccess)
        {
            return Result.Ok(await func());
        }

        return Result.ForwardFail<TResult>(result);
    }

    /// <summary>
    /// Executes an asynchronous function if the awaited result is successful, otherwise forwards the failure.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="func">The asynchronous function to execute.</param>
    /// <returns>A task representing the asynchronous operation, with a new result.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<TResult>> ThenAsync<T, TResult>(this Task<Result<T>> resultTask, Func<T, Task<TResult>> func)
    {
        var result = await resultTask.ConfigureAwait(false);

        if (result.IsSuccess)
        {
            return Result.Ok(await func(result.Value).ConfigureAwait(false));
        }

        return Result.ForwardFail<TResult>(result);
    }

#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER

    /// <summary>
    /// Executes an asynchronous function returning a <see cref="ValueTask{TResult}"/> if the awaited result is successful, otherwise forwards the failure.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="func">The asynchronous function to execute.</param>
    /// <returns>A task representing the asynchronous operation, with a new result.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<TResult>> ThenAsync<T, TResult>(this ValueTask<Result<T>> resultTask, Func<T, Task<TResult>> func)
    {
        var result = await resultTask.ConfigureAwait(false);

        if (result.IsSuccess)
        {
            return Result.Ok(await func(result.Value).ConfigureAwait(false));
        }

        return Result.ForwardFail<TResult>(result);
    }

    /// <summary>
    /// Executes an asynchronous function returning a <see cref="ValueTask{TResult}"/> if the result is successful, otherwise forwards the failure.
    /// </summary>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="func">The asynchronous function to execute.</param>
    /// <returns>A task representing the asynchronous operation, with a new result.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<TResult>> ThenAsync<TResult>(this Result result, Func<ValueTask<TResult>> func)
    {
        if (result.IsSuccess)
        {
            return Result.Ok(await func().ConfigureAwait(false));
        }

        return Result.ForwardFail<TResult>(result);
    }

    /// <summary>
    /// Executes an asynchronous function returning a <see cref="ValueTask{TResult}"/> if the result is successful, otherwise forwards the failure.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="func">The asynchronous function to execute.</param>
    /// <returns>A task representing the asynchronous operation, with a new result.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<TResult>> ThenAsync<T, TResult>(this Result<T> result, Func<T, ValueTask<TResult>> func)
    {
        if (result.IsSuccess)
        {
            return Result.Ok(await func(result.Value).ConfigureAwait(false));
        }

        return Result.ForwardFail<TResult>(result);
    }

    /// <summary>
    /// Executes a function if the awaited result is successful, otherwise forwards the failure. Supports ValueTask.
    /// </summary>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="func">The function to execute.</param>
    /// <returns>A ValueTask representing the asynchronous operation, with a new result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<Result<TResult>> ThenAsync<TResult>(this ValueTask<Result> resultTask, Func<TResult> func)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return AsyncHelper.CreateValueTask(Then(result, func));
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
    /// Executes a function if the awaited result is successful, otherwise forwards the failure. Supports ValueTask.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="func">The function to execute.</param>
    /// <returns>A ValueTask representing the asynchronous operation, with a new result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<Result<TResult>> ThenAsync<T, TResult>(this ValueTask<Result<T>> resultTask, Func<T, TResult> func)
    {
        if (resultTask.TryGetResult(out var result))
        {
            AsyncHelper.CreateValueTask(Then(result, func));
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
    /// Executes an asynchronous function returning a <see cref="ValueTask{TResult}"/> if the awaited result is successful, otherwise forwards the failure.
    /// </summary>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="func">The asynchronous function to execute.</param>
    /// <returns>A ValueTask representing the asynchronous operation, with a new result.</returns>
    [DebuggerStepperBoundary]
    public static async ValueTask<Result<TResult>> ThenAsync<TResult>(this ValueTask<Result> resultTask, Func<ValueTask<TResult>> func)
    {
        var result = await resultTask.ConfigureAwait(false);

        if (result.IsSuccess)
        {
            return Result.Ok(await func().ConfigureAwait(false));
        }

        return Result.ForwardFail<TResult>(result);
    }

    /// <summary>
    /// Executes an asynchronous function returning a <see cref="ValueTask{TResult}"/> if the awaited result is successful, otherwise forwards the failure.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="func">The asynchronous function to execute.</param>
    /// <returns>A ValueTask representing the asynchronous operation, with a new result.</returns>
    [DebuggerStepperBoundary]
    public static async ValueTask<Result<TResult>> ThenAsync<T, TResult>(this ValueTask<Result<T>> resultTask, Func<T, ValueTask<TResult>> func)
    {
        var result = await resultTask.ConfigureAwait(false);

        if (result.IsSuccess)
        {
            return Result.Ok(await func(result.Value).ConfigureAwait(false));
        }

        return Result.ForwardFail<TResult>(result);
    }

    /// <summary>
    /// Executes an asynchronous function returning a <see cref="Task{TResult}"/> if the awaited result is successful, otherwise forwards the failure.
    /// </summary>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="func">The asynchronous function to execute.</param>
    /// <returns>A Task representing the asynchronous operation, with a new result.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<TResult>> ThenAsync<TResult>(this ValueTask<Result> resultTask, Func<Task<TResult>> func)
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
