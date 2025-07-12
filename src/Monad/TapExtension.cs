using System.Diagnostics;

namespace ForgeSharp.Results.Monad;

public static class TapExtension
{
    /// <summary>
    /// Executes an action if the result is successful.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The original result.</returns>
    [DebuggerStepperBoundary]
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
    [DebuggerStepperBoundary]
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
            await action();
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
            await action(result.Value);
        }

        return result;
    }

    /// <summary>
    /// Executes an action if the awaited result is successful.
    /// </summary>
    /// <param name="resultTask">The result task.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The original result as a task.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result> TapAsync(this Task<Result> resultTask, Action action)
    {
        var result = await resultTask;

        if (result.IsSuccess)
        {
            action();
        }

        return result;
    }

    /// <summary>
    /// Executes an action if the awaited result is successful.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The original result as a task.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<T>> TapAsync<T>(this Task<Result<T>> resultTask, Action<T> action)
    {
        var result = await resultTask;

        if (result.IsSuccess)
        {
            action(result.Value);
        }

        return result;
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
        var result = await resultTask;

        if (result.IsSuccess)
        {
            await action();
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
        var result = await resultTask;

        if (result.IsSuccess)
        {
            await action(result.Value);
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
            await action();
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
            await action(result.Value);
        }

        return result;
    }

    /// <summary>
    /// Executes an action if the awaited ValueTask result is successful.
    /// </summary>
    /// <param name="resultTask">The ValueTask result.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The original result as a ValueTask.</returns>
    [DebuggerStepperBoundary]
    public static async ValueTask<Result> TapAsync(this ValueTask<Result> resultTask, Action action)
    {
        var result = await resultTask;

        if (result.IsSuccess)
        {
            action();
        }

        return result;
    }

    /// <summary>
    /// Executes an action if the awaited ValueTask result is successful.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="resultTask">The ValueTask result.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The original result as a ValueTask.</returns>
    [DebuggerStepperBoundary]
    public static async ValueTask<Result<T>> TapAsync<T>(this ValueTask<Result<T>> resultTask, Action<T> action)
    {
        var result = await resultTask;

        if (result.IsSuccess)
        {
            action(result.Value);
        }

        return result;
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
        var result = await resultTask;

        if (result.IsSuccess)
        {
            await action();
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
        var result = await resultTask;

        if (result.IsSuccess)
        {
            await action();
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
        var result = await resultTask;

        if (result.IsSuccess)
        {
            await action(result.Value);
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
        var result = await resultTask;

        if (result.IsSuccess)
        {
            await action(result.Value);
        }

        return result;
    }

#endif
}
