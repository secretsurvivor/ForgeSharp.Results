using ForgeSharp.Results.Infrastructure;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ForgeSharp.Results.Monad;

/// <summary>
/// Provides bind operators for <see cref="Result"/> and <see cref="Result{T}"/> which allow chaining
/// operations that themselves return results, enabling monadic composition.
/// </summary>
public static class BindExtension
{
    #region Sync
    /// <summary>
    /// Binds the result to an action that returns a new result.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="action">The action to bind.</param>
    /// <returns>The result of the action if the input result is successful, otherwise the input result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result Bind(this Result result, Func<Result> action)
    {
        if (!result.IsSuccess)
        {
            return result;
        }

        return action();
    }

    /// <summary>
    /// Binds a result containing a value to a function that transforms the value into a new result.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="action">The function to bind.</param>
    /// <returns>The result of the function if the input result is successful, otherwise the forwarded failure.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<TResult> Bind<T, TResult>(this Result<T> result, Func<T, Result<TResult>> action)
    {
        if (!result.IsSuccess)
        {
            return Result.ForwardFail<TResult>(result);
        }

        return action(result.Value);
    }

    /// <summary>
    /// Binds a result containing a value to a function that transforms the value into a new result with a custom error type.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <typeparam name="TError">The type of the custom error.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="action">The function to bind.</param>
    /// <returns>The result of the function if the input result is successful, otherwise the forwarded error.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<TResult, TError> Bind<T, TResult, TError>(this Result<T, TError> result, Func<T, Result<TResult, TError>> action)
    {
        if (!result.IsSuccess)
        {
            return Result.Fail<TResult, TError>(result.Error);
        }

        return action(result.Value);
    }
    #endregion

    #region Async Result
    /// <summary>
    /// Asynchronously binds a task containing a result to an action that returns a new result.
    /// </summary>
    /// <param name="resultTask">The result task.</param>
    /// <param name="action">The action to bind.</param>
    /// <returns>A task representing the asynchronous operation, containing the result of the action if the input result is successful, otherwise the input result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result> BindAsync(this Task<Result> resultTask, Func<Result> action)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return Task.FromResult(Bind(result, action));
        }

        return Impl(resultTask, action);

        static async Task<Result> Impl(Task<Result> resultTask, Func<Result> action)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                return result;
            }

            return action();
        }
    }

    /// <summary>
    /// Asynchronously binds a task containing a result with a value to a function that transforms the value into a new result.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="action">The function to bind.</param>
    /// <returns>A task representing the asynchronous operation, containing the result of the function if the input result is successful, otherwise the forwarded failure.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result<TResult>> BindAsync<T, TResult>(this Task<Result<T>> resultTask, Func<T, Result<TResult>> action)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return Task.FromResult(Bind(result, action));
        }

        return Impl(resultTask, action);

        static async Task<Result<TResult>> Impl(Task<Result<T>> resultTask, Func<T, Result<TResult>> action)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                return Result.ForwardFail<TResult>(result);
            }

            return action(result.Value);
        }
    }

    /// <summary>
    /// Asynchronously binds a task containing a result with a value to a function that transforms the value into a new result with a custom error type.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <typeparam name="TError">The type of the custom error.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="action">The function to bind.</param>
    /// <returns>A task representing the asynchronous operation, containing the result of the function if the input result is successful, otherwise the forwarded error.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result<TResult, TError>> BindAsync<T, TResult, TError>(this Task<Result<T, TError>> resultTask, Func<T, Result<TResult, TError>> action)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return Task.FromResult(Bind(result, action));
        }

        return Impl(resultTask, action);

        static async Task<Result<TResult, TError>> Impl(Task<Result<T, TError>> resultTask, Func<T, Result<TResult, TError>> action)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                return Result.Fail<TResult, TError>(result.Error);
            }

            return action(result.Value);
        }
    }
    #endregion

    #region Async Result and Parameter
    /// <summary>
    /// Asynchronously binds a task containing a result to an async action that returns a new result.
    /// </summary>
    /// <param name="resultTask">The result task.</param>
    /// <param name="action">The async action to bind.</param>
    /// <returns>A task representing the asynchronous operation, containing the result of the action if the input result is successful, otherwise the input result.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result> BindAsync(this Task<Result> resultTask, Func<Task<Result>> action)
    {
        var result = await resultTask.ConfigureAwait(false);

        if (!result.IsSuccess)
        {
            return result;
        }

        return await action();
    }

    /// <summary>
    /// Asynchronously binds a task containing a result with a value to an async function that transforms the value into a new result.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="action">The async function to bind.</param>
    /// <returns>A task representing the asynchronous operation, containing the result of the function if the input result is successful, otherwise the forwarded failure.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<TResult>> BindAsync<T, TResult>(this Task<Result<T>> resultTask, Func<T, Task<Result<TResult>>> action)
    {
        var result = await resultTask.ConfigureAwait(false);

        if (!result.IsSuccess)
        {
            return Result.ForwardFail<TResult>(result);
        }

        return await action(result.Value).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously binds a task containing a result with a value to an async function that transforms the value into a new result with a custom error type.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <typeparam name="TError">The type of the custom error.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="action">The async function to bind.</param>
    /// <returns>A task representing the asynchronous operation, containing the result of the function if the input result is successful, otherwise the forwarded error.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<TResult, TError>> BindAsync<T, TResult, TError>(this Task<Result<T, TError>> resultTask, Func<T, Task<Result<TResult, TError>>> action)
    {
        var result = await resultTask.ConfigureAwait(false);

        if (!result.IsSuccess)
        {
            return Result.Fail<TResult, TError>(result.Error);
        }

        return await action(result.Value).ConfigureAwait(false);
    }
    #endregion

    #region Async Parameter
    /// <summary>
    /// Asynchronously binds a synchronous result to an async action that returns a new result.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="action">The async action to bind.</param>
    /// <returns>A task representing the asynchronous operation, containing the result of the action if the input result is successful, otherwise the input result.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result> BindAsync(this Result result, Func<Task<Result>> action)
    {
        if (!result.IsSuccess)
        {
            return result;
        }

        return await action().ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously binds a synchronous result with a value to an async function that transforms the value into a new result.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="action">The async function to bind.</param>
    /// <returns>A task representing the asynchronous operation, containing the result of the function if the input result is successful, otherwise the forwarded failure.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<TResult>> BindAsync<T, TResult>(this Result<T> result, Func<T, Task<Result<TResult>>> action)
    {
        if (!result.IsSuccess)
        {
            return Result.ForwardFail<TResult>(result);
        }

        return await action(result.Value).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously binds a synchronous result with a value to an async function that transforms the value into a new result with a custom error type.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <typeparam name="TError">The type of the custom error.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="action">The async function to bind.</param>
    /// <returns>A task representing the asynchronous operation, containing the result of the function if the input result is successful, otherwise the forwarded error.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<TResult, TError>> BindAsync<T, TResult, TError>(this Result<T, TError> result, Func<T, Task<Result<TResult, TError>>> action)
    {
        if (!result.IsSuccess)
        {
            return Result.Fail<TResult, TError>(result.Error);
        }

        return await action(result.Value).ConfigureAwait(false);
    }
    #endregion
}
