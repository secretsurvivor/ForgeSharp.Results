using ForgeSharp.Results.Infrastructure;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ForgeSharp.Results.Monad;

/// <summary>
/// Bind (flatMap) operators for <see cref="Result"/> and <see cref="Result{T}"/>.
/// </summary>
public static class BindExtension
{
    #region Sync
    /// <summary>
    /// Binds the result to an action that returns a new result.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="action">The action to bind.</param>
    /// <returns>The action result on success, or the original failure.</returns>
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
    /// Binds a result to a function that transforms the value into a new result.
    /// </summary>
    /// <typeparam name="T">The input value type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="action">The function to bind.</param>
    /// <returns>The action result on success, or the forwarded failure.</returns>
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
    /// Binds a result to a function that returns a new result with a custom error type.
    /// </summary>
    /// <typeparam name="T">The input value type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="action">The function to bind.</param>
    /// <returns>The action result on success, or the forwarded error.</returns>
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
    /// Binds an awaited result to a sync action. See <see cref="Bind(Result, Func{Result})"/>.
    /// </summary>
    /// <param name="resultTask">The result task.</param>
    /// <param name="action">The action to bind.</param>
    /// <returns>The action result on success, or the original failure.</returns>
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
    /// Binds an awaited result to a sync function. See <see cref="Bind{T, TResult}(Result{T}, Func{T, Result{TResult}})"/>.
    /// </summary>
    /// <typeparam name="T">The input value type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="action">The function to bind.</param>
    /// <returns>The action result on success, or the forwarded failure.</returns>
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
    /// Binds an awaited result to a sync function with a custom error type.
    /// </summary>
    /// <typeparam name="T">The input value type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="action">The function to bind.</param>
    /// <returns>The action result on success, or the forwarded error.</returns>
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
    /// Binds an awaited result to an async action.
    /// </summary>
    /// <param name="resultTask">The result task.</param>
    /// <param name="action">The async action to bind.</param>
    /// <returns>The action result on success, or the original failure.</returns>
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
    /// Binds an awaited result to an async function.
    /// </summary>
    /// <typeparam name="T">The input value type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="action">The async function to bind.</param>
    /// <returns>The action result on success, or the forwarded failure.</returns>
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
    /// Binds an awaited result to an async function with a custom error type.
    /// </summary>
    /// <typeparam name="T">The input value type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="action">The async function to bind.</param>
    /// <returns>The action result on success, or the forwarded error.</returns>
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
    /// Binds a result to an async action.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="action">The async action to bind.</param>
    /// <returns>The action result on success, or the original failure.</returns>
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
    /// Binds a result to an async function.
    /// </summary>
    /// <typeparam name="T">The input value type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="action">The async function to bind.</param>
    /// <returns>The action result on success, or the forwarded failure.</returns>
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
    /// Binds a result to an async function with a custom error type.
    /// </summary>
    /// <typeparam name="T">The input value type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="action">The async function to bind.</param>
    /// <returns>The action result on success, or the forwarded error.</returns>
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
