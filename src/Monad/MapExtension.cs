using System.Diagnostics;

namespace ForgeSharp.Results.Monad;

public static class MapExtension
{
    /// <summary>
    /// Maps the values of a successful result sequence to a new type.
    /// </summary>
    /// <typeparam name="T">The type of the input values.</typeparam>
    /// <typeparam name="TResult">The type of the result values.</typeparam>
    /// <param name="result">The result containing a sequence of values.</param>
    /// <param name="func">The mapping function.</param>
    /// <returns>A new result with mapped values.</returns>
    [DebuggerStepperBoundary]
    public static Result<IEnumerable<TResult>> Map<T, TResult>(this Result<IEnumerable<T>> result, Func<T, TResult> func)
    {
        if (result.IsSuccess)
        {
            return Result.Ok(result.Value.Select(func));
        }

        return Result.ForwardFail<IEnumerable<TResult>>(result);
    }

    /// <summary>
    /// Maps the values of a successful awaited result sequence to a new type.
    /// </summary>
    /// <typeparam name="T">The type of the input values.</typeparam>
    /// <typeparam name="TResult">The type of the result values.</typeparam>
    /// <param name="resultTask">The result task containing a sequence of values.</param>
    /// <param name="func">The mapping function.</param>
    /// <returns>A new result with mapped values as a task.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<IEnumerable<TResult>>> MapAsync<T, TResult>(this Task<Result<IEnumerable<T>>> resultTask, Func<T, TResult> func)
    {
        var result = await resultTask;

        if (result.IsSuccess)
        {
            return Result.Ok(result.Value.Select(func));
        }

        return Result.ForwardFail<IEnumerable<TResult>>(result);
    }

#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER

    /// <summary>
    /// Maps the values of a successful awaited result sequence to a new type using a ValueTask.
    /// </summary>
    /// <typeparam name="T">The type of the input values.</typeparam>
    /// <typeparam name="TResult">The type of the result values.</typeparam>
    /// <param name="resultTask">The result value task containing a sequence of values.</param>
    /// <param name="func">The mapping function.</param>
    /// <returns>A new result with mapped values as a value task.</returns>
    [DebuggerStepperBoundary]
    public static async ValueTask<Result<IEnumerable<TResult>>> MapAsync<T, TResult>(this ValueTask<Result<IEnumerable<T>>> resultTask, Func<T, TResult> func)
    {
        var result = await resultTask;

        if (result.IsSuccess)
        {
            return Result.Ok(result.Value.Select(func));
        }

        return Result.ForwardFail<IEnumerable<TResult>>(result);
    }

#endif
}
