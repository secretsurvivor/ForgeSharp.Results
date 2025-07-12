using System.Diagnostics;

namespace ForgeSharp.Results.Monad;

public static class ResolveExtension
{
#if NETSTANDARD2_1 || NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER

    /// <summary>
    /// Asynchronously resolves a sequence of <see cref="Task{Result}"/> to an <see cref="IAsyncEnumerable{Result}"/>.
    /// </summary>
    /// <param name="resultTasks">The sequence of result tasks.</param>
    /// <returns>An async enumerable of resolved <see cref="Result"/> objects.</returns>
    [DebuggerStepperBoundary]
    public static async IAsyncEnumerable<Result> ResolveAsync(this IEnumerable<Task<Result>> resultTasks)
    {
        foreach (var task in resultTasks)
        {
            yield return await task;
        }
    }

    /// <summary>
    /// Asynchronously resolves a sequence of <see cref="ValueTask{Result}"/> to an <see cref="IAsyncEnumerable{Result}"/>.
    /// </summary>
    /// <param name="resultTasks">The sequence of result value tasks.</param>
    /// <returns>An async enumerable of resolved <see cref="Result"/> objects.</returns>
    [DebuggerStepperBoundary]
    public static async IAsyncEnumerable<Result> ResolveAsync(this IEnumerable<ValueTask<Result>> resultTasks)
    {
        foreach (var task in resultTasks)
        {
            yield return await task;
        }
    }

    /// <summary>
    /// Asynchronously resolves a sequence of <see cref="Task{Result{T}}"/> to an <see cref="IAsyncEnumerable{Result{T}}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value in the result.</typeparam>
    /// <param name="resultTasks">The sequence of result tasks.</param>
    /// <returns>An async enumerable of resolved <see cref="Result{T}"/> objects.</returns>
    [DebuggerStepperBoundary]
    public static async IAsyncEnumerable<Result<T>> ResolveAsync<T>(this IEnumerable<Task<Result<T>>> resultTasks)
    {
        foreach (var task in resultTasks)
        {
            yield return await task;
        }
    }

    /// <summary>
    /// Asynchronously resolves a sequence of <see cref="ValueTask{Result{T}}"/> to an <see cref="IAsyncEnumerable{Result{T}}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value in the result.</typeparam>
    /// <param name="resultTasks">The sequence of result value tasks.</param>
    /// <returns>An async enumerable of resolved <see cref="Result{T}"/> objects.</returns>
    [DebuggerStepperBoundary]
    public static async IAsyncEnumerable<Result<T>> ResolveAsync<T>(this IEnumerable<ValueTask<Result<T>>> resultTasks)
    {
        foreach (var task in resultTasks)
        {
            yield return await task;
        }
    }

#else

    /// <summary>
    /// Asynchronously resolves a sequence of <see cref="Task{Result}"/> to an <see cref="IEnumerable{Result}"/>.
    /// </summary>
    /// <param name="resultTasks">The sequence of result tasks.</param>
    /// <returns>A task representing the asynchronous operation, with a collection of resolved <see cref="Result"/> objects.</returns>
    [DebuggerStepperBoundary]
    public static async Task<IEnumerable<Result>> ResolveAsync(this IEnumerable<Task<Result>> resultTasks)
    {
        var results = new List<Result>();

        foreach (var result in resultTasks)
        {
            results.Add(await result);
        }

        return results;
    }

    /// <summary>
    /// Asynchronously resolves a sequence of <see cref="Task{Result{T}}"/> to an <see cref="IEnumerable{Result{T}}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value in the result.</typeparam>
    /// <param name="resultTasks">The sequence of result tasks.</param>
    /// <returns>A task representing the asynchronous operation, with a collection of resolved <see cref="Result{T}"/> objects.</returns>
    [DebuggerStepperBoundary]
    public static async Task<IEnumerable<Result<T>>> ResolveAsync<T>(this IEnumerable<Task<Result<T>>> resultTasks)
    {
        var results = new List<Result<T>>();

        foreach (var result in resultTasks)
        {
            results.Add(await result);
        }

        return results;
    }

#endif
}
