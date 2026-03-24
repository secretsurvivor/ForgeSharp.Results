using ForgeSharp.Results.Infrastructure;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ForgeSharp.Results.Monad;

/// <summary>
/// MapError operators for transforming the error side of a discriminated result, forwarding success unchanged.
/// </summary>
public static class MapErrorExtension
{
    #region Sync
    /// <summary>
    /// Transforms the error on failure, forwarding the value on success.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <typeparam name="TError">The original error type.</typeparam>
    /// <typeparam name="TNewError">The new error type.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="mapError">The function to transform the error.</param>
    /// <returns>The result with a mapped error type.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<TValue, TNewError> MapError<TValue, TError, TNewError>(this Result<TValue, TError> result, Func<TError, TNewError> mapError)
    {
        if (!result.IsSuccess)
        {
            return Result.Fail<TValue, TNewError>(mapError(result.Error));
        }

        return Result.Ok<TValue, TNewError>(result.Value);
    }
    #endregion

    #region Task Parameter
    /// <summary>
    /// Async version of the corresponding MapError overload.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <typeparam name="TError">The original error type.</typeparam>
    /// <typeparam name="TNewError">The new error type.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="mapError">The asynchronous function to transform the error.</param>
    /// <returns>The result with a mapped error type.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<TValue, TNewError>> MapErrorAsync<TValue, TError, TNewError>(this Result<TValue, TError> result, Func<TError, Task<TNewError>> mapError)
    {
        if (!result.IsSuccess)
        {
            return Result.Fail<TValue, TNewError>(await mapError(result.Error).ConfigureAwait(false));
        }

        return Result.Ok<TValue, TNewError>(result.Value);
    }
    #endregion

    #region Task Result
    /// <summary>
    /// Awaits the result, then maps the error on failure.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <typeparam name="TError">The original error type.</typeparam>
    /// <typeparam name="TNewError">The new error type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="mapError">The function to transform the error.</param>
    /// <returns>The result with a mapped error type.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result<TValue, TNewError>> MapErrorAsync<TValue, TError, TNewError>(this Task<Result<TValue, TError>> resultTask, Func<TError, TNewError> mapError)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return Task.FromResult(MapError(result, mapError));
        }

        return Impl(resultTask, mapError);

        static async Task<Result<TValue, TNewError>> Impl(Task<Result<TValue, TError>> resultTask, Func<TError, TNewError> mapError)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                return Result.Fail<TValue, TNewError>(mapError(result.Error));
            }

            return Result.Ok<TValue, TNewError>(result.Value);
        }
    }
    #endregion

    #region Task Result and Parameter
    /// <summary>
    /// Awaits the result, then runs the async function on the error.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <typeparam name="TError">The original error type.</typeparam>
    /// <typeparam name="TNewError">The new error type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="mapError">The asynchronous function to transform the error.</param>
    /// <returns>The result with a mapped error type.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<TValue, TNewError>> MapErrorAsync<TValue, TError, TNewError>(this Task<Result<TValue, TError>> resultTask, Func<TError, Task<TNewError>> mapError)
    {
        var result = await resultTask.ConfigureAwait(false);

        if (!result.IsSuccess)
        {
            return Result.Fail<TValue, TNewError>(await mapError(result.Error).ConfigureAwait(false));
        }

        return Result.Ok<TValue, TNewError>(result.Value);
    }
    #endregion

#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER

    /// <summary>
    /// ValueTask overload of the corresponding MapError.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <typeparam name="TError">The original error type.</typeparam>
    /// <typeparam name="TNewError">The new error type.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="mapError">The asynchronous function to transform the error.</param>
    /// <returns>The result with a mapped error type.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<TValue, TNewError>> MapErrorAsync<TValue, TError, TNewError>(this Result<TValue, TError> result, Func<TError, ValueTask<TNewError>> mapError)
    {
        if (!result.IsSuccess)
        {
            return Result.Fail<TValue, TNewError>(await mapError(result.Error).ConfigureAwait(false));
        }

        return Result.Ok<TValue, TNewError>(result.Value);
    }

    /// <summary>
    /// ValueTask overload: awaits the result, then maps the error on failure.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <typeparam name="TError">The original error type.</typeparam>
    /// <typeparam name="TNewError">The new error type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="mapError">The function to transform the error.</param>
    /// <returns>The result with a mapped error type.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<Result<TValue, TNewError>> MapErrorAsync<TValue, TError, TNewError>(this ValueTask<Result<TValue, TError>> resultTask, Func<TError, TNewError> mapError)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return AsyncHelper.CreateValueTask(MapError(result, mapError));
        }

        return Impl(resultTask, mapError);

        static async ValueTask<Result<TValue, TNewError>> Impl(ValueTask<Result<TValue, TError>> resultTask, Func<TError, TNewError> mapError)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                return Result.Fail<TValue, TNewError>(mapError(result.Error));
            }

            return Result.Ok<TValue, TNewError>(result.Value);
        }
    }

    /// <summary>
    /// ValueTask overload: awaits the result, then runs the async ValueTask function on the error.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <typeparam name="TError">The original error type.</typeparam>
    /// <typeparam name="TNewError">The new error type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="mapError">The asynchronous function to transform the error.</param>
    /// <returns>The result with a mapped error type.</returns>
    [DebuggerStepperBoundary]
    public static async ValueTask<Result<TValue, TNewError>> MapErrorAsync<TValue, TError, TNewError>(this ValueTask<Result<TValue, TError>> resultTask, Func<TError, ValueTask<TNewError>> mapError)
    {
        var result = await resultTask.ConfigureAwait(false);

        if (!result.IsSuccess)
        {
            return Result.Fail<TValue, TNewError>(await mapError(result.Error).ConfigureAwait(false));
        }

        return Result.Ok<TValue, TNewError>(result.Value);
    }

    /// <summary>
    /// ValueTask overload: awaits the result, then runs the async Task function on the error.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <typeparam name="TError">The original error type.</typeparam>
    /// <typeparam name="TNewError">The new error type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="mapError">The asynchronous function to transform the error.</param>
    /// <returns>The result with a mapped error type.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<TValue, TNewError>> MapErrorAsync<TValue, TError, TNewError>(this ValueTask<Result<TValue, TError>> resultTask, Func<TError, Task<TNewError>> mapError)
    {
        var result = await resultTask.ConfigureAwait(false);

        if (!result.IsSuccess)
        {
            return Result.Fail<TValue, TNewError>(await mapError(result.Error).ConfigureAwait(false));
        }

        return Result.Ok<TValue, TNewError>(result.Value);
    }

#endif
}
