using ForgeSharp.Results.Infrastructure;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ForgeSharp.Results.Monad;

/// <summary>
/// Flatten operators for unwrapping nested results and collapsing result sequences.
/// </summary>
public static class FlattenExtension
{
    /// <summary>
    /// Unwraps a nested <see cref="Result{Result}"/> into a single <see cref="Result"/>.
    /// </summary>
    /// <param name="result">The nested result to flatten.</param>
    /// <returns>The inner result on success; otherwise the outer failure.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result Flatten(this Result<Result> result)
    {
        if (result.IsSuccess)
        {
            return result.Value;
        }

        return Result.ForwardFail(result);
    }

    /// <summary>
    /// Unwraps a nested <see cref="Result{Result{T}}"/> into a single <see cref="Result{T}"/>.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="result">The nested result.</param>
    /// <returns>The inner result on success; otherwise the outer failure.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> Flatten<T>(this Result<Result<T>> result)
    {
        if (result.IsSuccess)
        {
            return result.Value;
        }

        return Result.ForwardFail<T>(result);
    }

    /// <summary>
    /// Async version of <see cref="Flatten{T}(Result{Result{T}})"/>.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="resultTask">The task containing the nested result.</param>
    /// <returns>The inner result on success; otherwise the outer failure.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result<T>> FlattenAsync<T>(this Task<Result<Result<T>>> resultTask)
    {
        if (resultTask.TryGetResult(out var result))
        {
            Task.FromResult(Flatten(result));
        }

        return Impl(resultTask);

        static async Task<Result<T>> Impl(Task<Result<Result<T>>> resultTask)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (result.IsSuccess)
            {
                return result.Value;
            }

            return Result.ForwardFail<T>(result);
        }
    }

    /// <summary>
    /// Async version of <see cref="Flatten(Result{Result})"/>.
    /// </summary>
    /// <param name="resultTask">The task containing the nested result.</param>
    /// <returns>The inner result on success; otherwise the outer failure.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result> FlattenAsync(this Task<Result<Result>> resultTask)
    {
        if (resultTask.TryGetResult(out var result))
        {
            Task.FromResult(Flatten(result));
        }

        return Impl(resultTask);

        static async Task<Result> Impl(Task<Result<Result>> resultTask)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (result.IsSuccess)
            {
                return result.Value;
            }

            return (Result)result;
        }
    }

    /// <summary>
    /// Collapses a sequence of <see cref="Result"/> into a single result.
    /// </summary>
    /// <param name="results">The sequence of results.</param>
    /// <returns>The first failure, or success if all passed.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result Flatten(this IEnumerable<Result> results)
    {
        if (results.Any(x => !x.IsSuccess))
        {
            return results.First(x => !x.IsSuccess);
        }

        return Result.Ok();
    }

    /// <summary>
    /// Async version of <see cref="Flatten(IEnumerable{Result})"/>.
    /// </summary>
    /// <typeparam name="T">Unused – required by the type system.</typeparam>
    /// <param name="resultTask">The task containing the sequence of results.</param>
    /// <returns>The first failure, or success if all passed.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result> FlattenAsync<T>(this Task<IEnumerable<Result>> resultTask)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return Task.FromResult(Flatten(result));
        }

        return Impl(resultTask);

        static async Task<Result> Impl(Task<IEnumerable<Result>> resultTask)
        {
            var results = await resultTask.ConfigureAwait(false);

            if (results.Any(x => !x.IsSuccess))
            {
                return results.First(x => !x.IsSuccess);
            }

            return Result.Ok();
        }
    }

#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER

    /// <summary>
    /// ValueTask overload of <see cref="FlattenAsync{T}(Task{Result{Result{T}}})"/>.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="resultTask">The value task containing the nested result.</param>
    /// <returns>The inner result on success; otherwise the outer failure.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<Result<T>> FlattenAsync<T>(this ValueTask<Result<Result<T>>> resultTask)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return AsyncHelper.CreateValueTask(Flatten(result));
        }

        return Impl(resultTask);

        static async ValueTask<Result<T>> Impl(ValueTask<Result<Result<T>>> resultTask)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (result.IsSuccess)
            {
                return result.Value;
            }

            return Result.ForwardFail<T>(result);
        }
    }

    /// <summary>
    /// ValueTask overload of <see cref="FlattenAsync(Task{Result{Result}})"/>.
    /// </summary>
    /// <param name="resultTask">The value task containing the nested result.</param>
    /// <returns>The inner result on success; otherwise the outer failure.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<Result> FlattenAsync(this ValueTask<Result<Result>> resultTask)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return AsyncHelper.CreateValueTask(Flatten(result));
        }

        return Impl(resultTask);

        static async ValueTask<Result> Impl(ValueTask<Result<Result>> resultTask)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (result.IsSuccess)
            {
                return result.Value;
            }

            return (Result)result;
        }
    }

    /// <summary>
    /// ValueTask overload of <see cref="FlattenAsync{T}(Task{IEnumerable{Result}})"/>.
    /// </summary>
    /// <typeparam name="T">Unused – required by the type system.</typeparam>
    /// <param name="resultTask">The value task containing the sequence of results.</param>
    /// <returns>The first failure, or success if all passed.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<Result> FlattenAsync<T>(this ValueTask<IEnumerable<Result>> resultTask)
    {
        if (resultTask.TryGetResult(out var result))
        {
            return AsyncHelper.CreateValueTask(Flatten(result));
        }

        return Impl(resultTask);

        static async ValueTask<Result> Impl(ValueTask<IEnumerable<Result>> resultTask)
        {
            var results = await resultTask.ConfigureAwait(false);

            if (results.Any(x => !x.IsSuccess))
            {
                return results.First(x => !x.IsSuccess);
            }

            return Result.Ok();
        }
    }

    /// <summary>
    /// Collapses an async sequence of <see cref="Result"/> into a single result.
    /// </summary>
    /// <param name="results">The asynchronous sequence of results.</param>
    /// <returns>The first failure, or success if all passed.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result> FlattenAsync(this IAsyncEnumerable<Result> results)
    {
        await foreach (var result in results.ConfigureAwait(false))
        {
            if (!result.IsSuccess)
            {
                return result;
            }
        }

        return Result.Ok();
    }

#endif
}
