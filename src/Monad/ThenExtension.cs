using System.Diagnostics;

namespace ForgeSharp.Results.Monad;

public static class ThenExtension
{
    /// <summary>
    /// Executes a function if the result is successful, otherwise forwards the failure.
    /// </summary>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="func">The function to execute.</param>
    /// <returns>A new result.</returns>
    [DebuggerStepperBoundary]
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
    [DebuggerStepperBoundary]
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
            return Result.Ok(await func());
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
            return Result.Ok(await func(result.Value));
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
    [DebuggerStepperBoundary]
    public static async Task<Result<TResult>> ThenAsync<TResult>(this Task<Result> resultTask, Func<TResult> func)
    {
        var result = await resultTask;

        if (result.IsSuccess)
        {
            return Result.Ok(func());
        }

        return Result.ForwardFail<TResult>(result);
    }

    /// <summary>
    /// Executes a function if the awaited result is successful, otherwise forwards the failure.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="func">The function to execute.</param>
    /// <returns>A task representing the asynchronous operation, with a new result.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<TResult>> ThenAsync<T, TResult>(this Task<Result<T>> resultTask, Func<T, TResult> func)
    {
        var result = await resultTask;

        if (result.IsSuccess)
        {
            return Result.Ok(func(result.Value));
        }

        return Result.ForwardFail<TResult>(result);
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
        var result = await resultTask;

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
        var result = await resultTask;

        if (result.IsSuccess)
        {
            return Result.Ok(await func(result.Value));
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
        var result = await resultTask;

        if (result.IsSuccess)
        {
            return Result.Ok(await func(result.Value));
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
            return Result.Ok(await func());
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
            return Result.Ok(await func(result.Value));
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
    [DebuggerStepperBoundary]
    public static async ValueTask<Result<TResult>> ThenAsync<TResult>(this ValueTask<Result> resultTask, Func<TResult> func)
    {
        var result = await resultTask;

        if (result.IsSuccess)
        {
            return Result.Ok(func());
        }

        return Result.ForwardFail<TResult>(result);
    }

    /// <summary>
    /// Executes a function if the awaited result is successful, otherwise forwards the failure. Supports ValueTask.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="func">The function to execute.</param>
    /// <returns>A ValueTask representing the asynchronous operation, with a new result.</returns>
    [DebuggerStepperBoundary]
    public static async ValueTask<Result<TResult>> ThenAsync<T, TResult>(this ValueTask<Result<T>> resultTask, Func<T, TResult> func)
    {
        var result = await resultTask;

        if (result.IsSuccess)
        {
            return Result.Ok(func(result.Value));
        }

        return Result.ForwardFail<TResult>(result);
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
        var result = await resultTask;

        if (result.IsSuccess)
        {
            return Result.Ok(await func());
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
        var result = await resultTask;

        if (result.IsSuccess)
        {
            return Result.Ok(await func(result.Value));
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
        var result = await resultTask;

        if (result.IsSuccess)
        {
            return Result.Ok(await func());
        }

        return Result.ForwardFail<TResult>(result);
    }

#endif
}
