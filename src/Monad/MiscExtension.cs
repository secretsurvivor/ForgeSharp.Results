using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace ForgeSharp.Results.Monad;

/// <summary>
/// Provides extension methods for working with <see cref="Result"/> and <see cref="Result{T}"/>.
/// </summary>
public static class MiscExtension
{
    /// <summary>
    /// Gets the value if the result is successful, otherwise returns the default value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="defaultValue">The default value to return if not successful.</param>
    /// <returns>The value or the default value.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetOrDefault<T>(this IResult<T> result, T defaultValue = default!)
    {
        if (result.IsSuccess)
        {
            return result.Value;
        }

        return defaultValue;
    }

    /// <summary>
    /// Gets the value if the awaited result is successful, otherwise returns the default value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="defaultValue">The default value to return if not successful.</param>
    /// <returns>The value or the default value as a task.</returns>
    [DebuggerStepperBoundary]
    public static async Task<T> GetOrDefaultAsync<T>(this Task<Result<T>> resultTask, T defaultValue = default!)
    {
        var result = await resultTask;

        if (result.IsSuccess)
        {
            return result.Value;
        }

        return defaultValue;
    }

    /// <summary>
    /// Gets the value if the result is successful, otherwise throws an exception.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="result">The result.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentException">Thrown if the result is not successful and not an exception.</exception>
    [DebuggerStepperBoundary]
    public static T GetOrThrow<T>(this IResult<T> result)
    {
        if (result.IsSuccess)
        {
            return result.Value;
        }

        if (result.Status == Result.State.Exception)
        {
            ExceptionDispatchInfo.Capture(result.Exception).Throw();
        }

        throw new ArgumentException(result.Message);
    }

    /// <summary>
    /// Gets the value if the awaited result is successful, otherwise throws an exception.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <returns>The value as a task.</returns>
    /// <exception cref="ArgumentException">Thrown if the result is not successful and not an exception.</exception>
    [DebuggerStepperBoundary]
    public static async Task<T> GetOrThrowAsync<T>(this Task<Result<T>> resultTask)
    {
        var result = await resultTask;

        if (result.IsSuccess)
        {
            return result.Value;
        }

        if (result.Status == Result.State.Exception)
        {
            ExceptionDispatchInfo.Capture(result.Exception).Throw();
        }

        throw new ArgumentException(result.Message);
    }

    /// <summary>
    /// Determines whether the awaited result is successful.
    /// </summary>
    /// <param name="resultTask">The result task.</param>
    /// <returns>True if successful; otherwise, false.</returns>
    [DebuggerStepperBoundary]
    public static async Task<bool> IsSuccessAsync(this Task<Result> resultTask)
    {
        return (await resultTask).IsSuccess;
    }

    /// <summary>
    /// Determines whether the awaited result is successful.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <returns>True if successful; otherwise, false.</returns>
    [DebuggerStepperBoundary]
    public static async Task<bool> IsSuccessAsync<T>(this Task<Result<T>> resultTask)
    {
        return (await resultTask).IsSuccess;
    }

    /// <summary>
    /// Ensures that the value of a successful result is not null.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="result">The result.</param>
    /// <returns>A successful result if the value is not null; otherwise, a failed result.</returns>
    [DebuggerStepperBoundary]
    public static Result<T> EnsureNotNull<T>(this Result<T?> result)
    {
        if (result.IsSuccess)
        {
            if (result.Value is not null)
            {
                return Result.Ok(result.Value);
            }

            return Result.Fail<T>($"Result<{typeof(T).Name}> is null");
        }

        return Result.ForwardFail<T>(result);
    }

    /// <summary>
    /// Ensures that the value of a successful awaited result is not null.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <returns>A successful result if the value is not null; otherwise, a failed result as a task.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<T>> EnsureNotNullAsync<T>(this Task<Result<T?>> resultTask)
    {
        var result = await resultTask;

        if (result.IsSuccess)
        {
            if (result.Value is not null)
            {
                return Result.Ok(result.Value);
            }

            return Result.Fail<T>($"Result<{typeof(T).Name}> is null");
        }

        return Result.ForwardFail<T>(result);
    }

    /// <summary>
    /// Converts a <see cref="Result{T}"/> to a non-generic <see cref="Result"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="result">The result to convert.</param>
    /// <returns>The non-generic result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result AsResult<T>(this Result<T> result)
    {
        return (Result)result;
    }

    /// <summary>
    /// Converts a <see cref="Result{T}"/> to a non-generic <see cref="Result"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <returns>The non-generic result as a task.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<Result> AsResultAsync<T>(this Task<Result<T>> resultTask)
    {
        return (Result)await resultTask;
    }

    /// <summary>
    /// Restores a failed result using the provided restore function.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="restoreFunc">The function to restore the result.</param>
    /// <returns>The original result if successful; otherwise, the restored result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> Restore<T>(this Result<T> result, Func<IResult, Result<T>> restoreFunc)
    {
        if (result.IsSuccess)
        {
            return result;
        }

        return restoreFunc(result);
    }

    /// <summary>
    /// Restores a failed awaited result using the provided restore function.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="restoreFunc">The function to restore the result.</param>
    /// <returns>The original result if successful; otherwise, the restored result as a task.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<T>> RestoreAsync<T>(this Task<Result<T>> resultTask, Func<IResult, Result<T>> restoreFunc)
    {
        var result = await resultTask;

        if (result.IsSuccess)
        {
            return result;
        }

        return restoreFunc(result);
    }

    /// <summary>
    /// Restores a failed awaited result using the provided asynchronous restore function.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="restoreFunc">The asynchronous function to restore the result.</param>
    /// <returns>The original result if successful; otherwise, the restored result as a task.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<T>> RestoreAsync<T>(this Task<Result<T>> resultTask, Func<IResult, Task<Result<T>>> restoreFunc)
    {
        var result = await resultTask;

        if (result.IsSuccess)
        {
            return result;
        }

        return await restoreFunc(result);
    }

#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER

    /// <summary>
    /// Gets the value if the awaited result is successful, otherwise returns the default value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="resultTask">The result value task.</param>
    /// <param name="defaultValue">The default value to return if not successful.</param>
    /// <returns>The value or the default value as a value task.</returns>
    [DebuggerStepperBoundary]
    public static async ValueTask<T> GetOrDefaultAsync<T>(this ValueTask<Result<T>> resultTask, T defaultValue = default!)
    {
        var result = await resultTask;

        if (result.IsSuccess)
        {
            return result.Value;
        }

        return defaultValue;
    }

    /// <summary>
    /// Gets the value if the awaited result is successful, otherwise throws an exception.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="resultTask">The result value task.</param>
    /// <returns>The value as a value task.</returns>
    /// <exception cref="ArgumentException">Thrown if the result is not successful and not an exception.</exception>
    [DebuggerStepperBoundary]
    public static async ValueTask<T> GetOrThrowAsync<T>(this ValueTask<Result<T>> resultTask)
    {
        var result = await resultTask;

        if (result.IsSuccess)
        {
            return result.Value;
        }

        if (result.Status == Result.State.Exception)
        {
            ExceptionDispatchInfo.Capture(result.Exception).Throw();
        }

        throw new ArgumentException(result.Message);
    }

    /// <summary>
    /// Determines whether the awaited result is successful.
    /// </summary>
    /// <param name="resultTask">The result value task.</param>
    /// <returns>True if successful; otherwise, false as a value task.</returns>
    [DebuggerStepperBoundary]
    public static async ValueTask<bool> IsSuccessAsync(this ValueTask<Result> resultTask)
    {
        return (await resultTask).IsSuccess;
    }

    /// <summary>
    /// Determines whether the awaited result is successful.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="resultTask">The result value task.</param>
    /// <returns>True if successful; otherwise, false as a value task.</returns>
    [DebuggerStepperBoundary]
    public static async ValueTask<bool> IsSuccess<T>(this ValueTask<Result<T>> resultTask)
    {
        return (await resultTask).IsSuccess;
    }

    /// <summary>
    /// Ensures that the value of a successful awaited result is not null.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="resultTask">The result value task.</param>
    /// <returns>A successful result if the value is not null; otherwise, a failed result as a value task.</returns>
    [DebuggerStepperBoundary]
    public static async ValueTask<Result<T>> EnsureNotNullAsync<T>(this ValueTask<Result<T?>> resultTask)
    {
        var result = await resultTask;

        if (result.IsSuccess)
        {
            if (result.Value is not null)
            {
                return Result.Ok(result.Value);
            }

            return Result.Fail<T>($"Result<{typeof(T).Name}> is null");
        }

        return Result.ForwardFail<T>(result);
    }

    /// <summary>
    /// Converts a <see cref="Result{T}"/> to a non-generic <see cref="Result"/> as a value task.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="resultTask">The result value task.</param>
    /// <returns>The non-generic result as a value task.</returns>
    [DebuggerStepperBoundary]
    public static async ValueTask<Result> AsResultAsync<T>(this ValueTask<Result<T>> resultTask)
    {
        return (Result)await resultTask;
    }

#endif
}
