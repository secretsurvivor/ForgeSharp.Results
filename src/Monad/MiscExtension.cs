using ForgeSharp.Results.Infrastructure;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace ForgeSharp.Results.Monad;

/// <summary>
/// Utility methods for extracting, converting, and inspecting results.
/// </summary>
public static class MiscExtension
{
    /// <summary>
    /// Gets the value if the result is successful, otherwise returns the default value.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
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
    /// Gets the value if the result is successful, otherwise returns the default value.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="defaultValue">The default value to return if not successful.</param>
    /// <returns>The value or the default value.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetOrDefault<T, TError>(this Result<T, TError> result, T defaultValue = default!)
    {
        if (!result.IsSuccess)
        {
            return defaultValue;
        }

        return result.Value;
    }

    /// <summary>
    /// Awaits the result, then returns the value or default.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="defaultValue">The default value to return if not successful.</param>
    /// <returns>The value or default.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<T> GetOrDefaultAsync<T>(this Task<Result<T>> resultTask, T defaultValue = default!)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return Task.FromResult(GetOrDefault(result, defaultValue));
        }

        return Impl(resultTask, defaultValue);

        static async Task<T> Impl(Task<Result<T>> resultTask, T defaultValue)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (result.IsSuccess)
            {
                return result.Value;
            }

            return defaultValue;
        }
    }

    /// <summary>
    /// Awaits the result, then returns the value or default.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="defaultValue">The default value to return if not successful.</param>
    /// <returns>The value or default.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<T> GetOrDefaultAsync<T, TError>(this Task<Result<T, TError>> resultTask, T defaultValue = default!)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return Task.FromResult(GetOrDefault(result, defaultValue));
        }

        return Impl(resultTask, defaultValue);

        static async Task<T> Impl(Task<Result<T, TError>> resultTask, T defaultValue)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (result.IsSuccess)
            {
                return result.Value;
            }

            return defaultValue;
        }
    }

    /// <summary>
    /// Returns the value on success, or re-throws/throws on failure.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="result">The result.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentException">Thrown if the result is not successful and not an exception.</exception>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetOrThrow<T>(this IResult<T> result)
    {
        if (result.IsSuccess)
        {
            return result.Value;
        }

        if (result.IsException)
        {
            ExceptionDispatchInfo.Capture(result.Exception).Throw();
        }

        throw new ArgumentException(result.Message);
    }

    /// <summary>
    /// Async version of <see cref="GetOrThrow{T}(IResult{T})"/>.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentException">Thrown if the result is not successful and not an exception.</exception>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<T> GetOrThrowAsync<T>(this Task<Result<T>> resultTask)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return Task.FromResult(GetOrThrow(result));
        }

        return Impl(resultTask);

        static async Task<T> Impl(Task<Result<T>> resultTask)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (result.IsSuccess)
            {
                return result.Value;
            }

            if (result.IsException)
            {
                ExceptionDispatchInfo.Capture(result.Exception).Throw();
            }

            throw new ArgumentException(result.Message);
        }
    }

    /// <summary>
    /// Checks whether the awaited result succeeded.
    /// </summary>
    /// <param name="resultTask">The result task.</param>
    /// <returns>True if successful; otherwise, false.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<bool> IsSuccessAsync(this Task<Result> resultTask)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return Task.FromResult(result.IsSuccess);
        }

        return Impl(resultTask);

        static async Task<bool> Impl(Task<Result> resultTask)
        {
            return (await resultTask.ConfigureAwait(false)).IsSuccess;
        }
    }

    /// <summary>
    /// Checks whether the awaited result succeeded.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <returns>True if successful; otherwise, false.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<bool> IsSuccessAsync<T>(this Task<Result<T>> resultTask)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return Task.FromResult(result.IsSuccess);
        }

        return Impl(resultTask);

        static async Task<bool> Impl(Task<Result<T>> resultTask)
        {
            return (await resultTask.ConfigureAwait(false)).IsSuccess;
        }
    }

    /// <summary>
    /// Checks whether the awaited result succeeded.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <returns>True if successful; otherwise, false.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<bool> IsSuccessAsync<T, TError>(this Task<Result<T, TError>> resultTask)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return Task.FromResult(result.IsSuccess);
        }

        return Impl(resultTask);

        static async Task<bool> Impl(Task<Result<T, TError>> resultTask)
        {
            return (await resultTask.ConfigureAwait(false)).IsSuccess;
        }
    }

    /// <summary>
    /// Fails the result if the value is null.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="result">The result.</param>
    /// <returns>A successful result if the value is not null; otherwise, a failed result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    /// Async version of <see cref="EnsureNotNull{T}(Result{T})"/>.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <returns>A successful result if the value is not null; otherwise, a failed result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result<T>> EnsureNotNullAsync<T>(this Task<Result<T?>> resultTask)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return Task.FromResult(EnsureNotNull(result));
        }

        return Impl(resultTask);

        static async Task<Result<T>> Impl(Task<Result<T?>> resultTask)
        {
            var result = await resultTask.ConfigureAwait(false);

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
    }

    /// <summary>
    /// Drops the value, keeping only the success/failure state.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="result">The result to convert.</param>
    /// <returns>The non-generic result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result AsResult<T>(this Result<T> result)
    {
        return (Result) result;
    }

    /// <summary>
    /// Async version of <see cref="AsResult{T}(Result{T})"/>.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <returns>The non-generic result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result> AsResultAsync<T>(this Task<Result<T>> resultTask)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return Task.FromResult((Result) result);
        }

        return Impl(resultTask);

        static async Task<Result> Impl(Task<Result<T>> resultTask)
        {
            return (Result) await resultTask.ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Recovers from failure using the provided function.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
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
    /// Async version of <see cref="Restore{T}(Result{T}, Func{IResult, Result{T}})"/>.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="restoreFunc">The function to restore the result.</param>
    /// <returns>The original result if successful; otherwise, the restored result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result<T>> RestoreAsync<T>(this Task<Result<T>> resultTask, Func<IResult, Result<T>> restoreFunc)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return Task.FromResult(Restore(result, restoreFunc));
        }

        return Impl(resultTask, restoreFunc);

        static async Task<Result<T>> Impl(Task<Result<T>> resultTask, Func<IResult, Result<T>> restoreFunc)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (result.IsSuccess)
            {
                return result;
            }

            return restoreFunc(result);
        }
    }

    /// <summary>
    /// Async version of Restore with an async restore function.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="resultTask">The result task.</param>
    /// <param name="restoreFunc">The asynchronous function to restore the result.</param>
    /// <returns>The original result if successful; otherwise, the restored result.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<T>> RestoreAsync<T>(this Task<Result<T>> resultTask, Func<IResult, Task<Result<T>>> restoreFunc)
    {
        var result = await resultTask.ConfigureAwait(false);

        if (result.IsSuccess)
        {
            return result;
        }

        return await restoreFunc(result).ConfigureAwait(false);
    }

#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER

    /// <summary>
    /// ValueTask overload of GetOrDefaultAsync.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="resultTask">The result value task.</param>
    /// <param name="defaultValue">The default value to return if not successful.</param>
    /// <returns>The value or default.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<T> GetOrDefaultAsync<T>(this ValueTask<Result<T>> resultTask, T defaultValue = default!)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return AsyncHelper.CreateValueTask(GetOrDefault(result, defaultValue));
        }

        return Impl(resultTask, defaultValue);

        static async ValueTask<T> Impl(ValueTask<Result<T>> resultTask, T defaultValue)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (result.IsSuccess)
            {
                return result.Value;
            }

            return defaultValue;
        }
    }

    /// <summary>
    /// ValueTask overload of GetOrThrowAsync.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="resultTask">The result value task.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentException">Thrown if the result is not successful and not an exception.</exception>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<T> GetOrThrowAsync<T>(this ValueTask<Result<T>> resultTask)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return AsyncHelper.CreateValueTask(GetOrThrow(result));
        }

        return Impl(resultTask);

        static async ValueTask<T> Impl(ValueTask<Result<T>> resultTask)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (result.IsSuccess)
            {
                return result.Value;
            }

            if (result.IsException)
            {
                ExceptionDispatchInfo.Capture(result.Exception).Throw();
            }

            throw new ArgumentException(result.Message);
        }
    }

    /// <summary>
    /// ValueTask overload of IsSuccessAsync.
    /// </summary>
    /// <param name="resultTask">The result value task.</param>
    /// <returns>True if successful; otherwise, false.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<bool> IsSuccessAsync(this ValueTask<Result> resultTask)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return AsyncHelper.CreateValueTask(result.IsSuccess);
        }

        return Impl(resultTask);

        static async ValueTask<bool> Impl(ValueTask<Result> resultTask)
        {
            return (await resultTask.ConfigureAwait(false)).IsSuccess;
        }
    }

    /// <summary>
    /// ValueTask overload of IsSuccessAsync.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="resultTask">The result value task.</param>
    /// <returns>True if successful; otherwise, false.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<bool> IsSuccess<T>(this ValueTask<Result<T>> resultTask)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return AsyncHelper.CreateValueTask(result.IsSuccess);
        }

        return Impl(resultTask);

        static async ValueTask<bool> Impl(ValueTask<Result<T>> resultTask)
        {
            return (await resultTask.ConfigureAwait(false)).IsSuccess;
        }
    }

    /// <summary>
    /// ValueTask overload of EnsureNotNullAsync.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="resultTask">The result value task.</param>
    /// <returns>A successful result if the value is not null; otherwise, a failed result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<Result<T>> EnsureNotNullAsync<T>(this ValueTask<Result<T?>> resultTask)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return AsyncHelper.CreateValueTask(EnsureNotNull(result));
        }

        return Impl(resultTask);

        static async ValueTask<Result<T>> Impl(ValueTask<Result<T?>> resultTask)
        {
            var result = await resultTask.ConfigureAwait(false);

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
    }

    /// <summary>
    /// ValueTask overload of AsResultAsync.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="resultTask">The result value task.</param>
    /// <returns>The non-generic result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<Result> AsResultAsync<T>(this ValueTask<Result<T>> resultTask)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return AsyncHelper.CreateValueTask((Result) result);
        }

        return Impl(resultTask);

        static async ValueTask<Result> Impl(ValueTask<Result<T>> resultTask)
        {
            return (Result) await resultTask.ConfigureAwait(false);
        }
    }

#endif

    /// <summary>
    /// Wraps Dictionary.TryGetValue as a Result.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="dict">The dictionary to search.</param>
    /// <param name="key">The key to locate.</param>
    /// <returns>The value if found; otherwise, a failed result.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepperBoundary]
    public static Result<T> TryGetValueResult<TKey, T>(this IDictionary<TKey, T> dict, TKey key)
    {
        return dict.TryGetValue(key, out var value) ? Result.Ok(value) : Result.Fail<T>($"Key '{key}' not found.");
    }

    /// <summary>
    /// Returns the first matching element as a Result, failing if none found.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="source">The sequence to search.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="errorMessage">The error message to use if no matching element is found.</param>
    /// <returns>The first matching element, or a failed result.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepperBoundary]
    public static Result<T> FirstOrResult<T>(this IEnumerable<T> source, Func<T, bool> predicate, string errorMessage = "No matching element found.")
    {
        var item = source.FirstOrDefault(predicate);

        if (item != null)
        {
            return Result.Ok(item);
        }
        else
        {
            return Result.Fail<T>(errorMessage);
        }
    }

    /// <summary>
    /// Tries to get the value without throwing.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="result">The result to read the value from.</param>
    /// <param name="value">The value if successful; otherwise, <c>default(T)</c>.</param>
    /// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepperBoundary]
    public static bool TryGetValue<T>(this IResult<T> result, out T value)
    {
        if (result.IsSuccess)
        {
            value = result.Value;
            return true;
        }

        value = default!;
        return false;
    }

    /// <summary>
    /// Extracts values from results, substituting defaults for failures.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="results">The enumerable result collection to extract values from.</param>
    /// <param name="defaultValue">The value to use for failed elements.</param>
    /// <returns>A list of values with defaults substituted for failures.</returns>
    public static IReadOnlyList<T> ExtractValuesOrDefault<T>(this EnumerableResult<T> results, T defaultValue = default!)
    {
        var values = new T[results.Total];
        int index = 0;

        results.ForEach(result => {
            values[index] = result.IsSuccess ? result.Value : defaultValue;

            index++;
        });

        return values;
    }

    /// <summary>
    /// Converts to an <see cref="EnumerableResult"/>.
    /// </summary>
    /// <param name="results">The sequence of results to wrap.</param>
    /// <returns>An <see cref="EnumerableResult"/> containing the provided results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="results"/> is <c>null</c>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EnumerableResult ToEnumerableResult(this IEnumerable<Result> results)
    {
        return EnumerableResult.Create(results);
    }

    /// <summary>
    /// Converts to an <see cref="EnumerableResult{T}"/>.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="results">The sequence of results to wrap.</param>
    /// <returns>An <see cref="EnumerableResult{T}"/> containing the provided results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="results"/> is <c>null</c>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EnumerableResult<T> ToEnumerableResult<T>(this IEnumerable<Result<T>> results)
    {
        return EnumerableResult.Create(results);
    }
}
