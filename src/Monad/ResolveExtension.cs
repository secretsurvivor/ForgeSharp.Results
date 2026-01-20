using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ForgeSharp.Results.Monad;

/// <summary>
/// Provides helpers to resolve collections of asynchronous <see cref="Result"/> producers
/// (for example <see cref="Task{Result}"/> and <see cref="ValueTask{Result}"/>) into
/// synchronous or asynchronous sequences. Useful for materializing or streaming the
/// results as they complete.
/// </summary>
public static class ResolveExtension
{
#if NETSTANDARD2_1 || NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER

    /// <summary>
    /// Asynchronously resolves a sequence of <see cref="Task{Result}"/> to an <see cref="IAsyncEnumerable{Result}"/>.
    /// </summary>
    /// <param name="resultTasks">The sequence of result tasks.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous enumeration.</param>
    /// <returns>An async enumerable of resolved <see cref="Result"/> objects.</returns>
    [DebuggerStepperBoundary]
    public static async IAsyncEnumerable<Result> ResolveAsync(this IEnumerable<Task<Result>> resultTasks, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var task in resultTasks)
        {
            cancellationToken.ThrowIfCancellationRequested();

            yield return await task
#if NET6_0_OR_GREATER
                .WaitAsync(cancellationToken)
#endif
                .ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Asynchronously resolves a sequence of <see cref="ValueTask{Result}"/> to an <see cref="IAsyncEnumerable{Result}"/>.
    /// </summary>
    /// <param name="resultTasks">The sequence of result value tasks.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous enumeration.</param>
    /// <returns>An async enumerable of resolved <see cref="Result"/> objects.</returns>
    [DebuggerStepperBoundary]
    public static async IAsyncEnumerable<Result> ResolveAsync(this IEnumerable<ValueTask<Result>> resultTasks, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var task in resultTasks)
        {
            cancellationToken.ThrowIfCancellationRequested();

            yield return await task
#if NET6_0_OR_GREATER
                .AsTask()
                .WaitAsync(cancellationToken)
#endif
                .ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Asynchronously resolves a sequence of <see cref="Task{Result{T}}"/> to an <see cref="IAsyncEnumerable{Result{T}}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value in the result.</typeparam>
    /// <param name="resultTasks">The sequence of result tasks.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous enumeration.</param>
    /// <returns>An async enumerable of resolved <see cref="Result{T}"/> objects.</returns>
    [DebuggerStepperBoundary]
    public static async IAsyncEnumerable<Result<T>> ResolveAsync<T>(this IEnumerable<Task<Result<T>>> resultTasks, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var task in resultTasks)
        {
            cancellationToken.ThrowIfCancellationRequested();

            yield return await task
#if NET6_0_OR_GREATER
                .WaitAsync(cancellationToken)
#endif
                .ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Asynchronously resolves a sequence of <see cref="ValueTask{Result{T}}"/> to an <see cref="IAsyncEnumerable{Result{T}}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value in the result.</typeparam>
    /// <param name="resultTasks">The sequence of result value tasks.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous enumeration.</param>
    /// <returns>An async enumerable of resolved <see cref="Result{T}"/> objects.</returns>
    [DebuggerStepperBoundary]
    public static async IAsyncEnumerable<Result<T>> ResolveAsync<T>(this IEnumerable<ValueTask<Result<T>>> resultTasks, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var task in resultTasks)
        {
            cancellationToken.ThrowIfCancellationRequested();

            yield return await task
#if NET6_0_OR_GREATER
                .AsTask()
                .WaitAsync(cancellationToken)
#endif
                .ConfigureAwait(false);
        }
    }

#endif

    /// <summary>
    /// Asynchronously resolves a sequence of <see cref="Task{Result}"/> to an <see cref="IEnumerable{Result}"/>.
    /// </summary>
    /// <param name="resultTasks">The sequence of result tasks.</param>
    /// <returns>A task representing the asynchronous operation, with a collection of resolved <see cref="Result"/> objects.</returns>
    [DebuggerStepperBoundary]
    public static async Task<IEnumerable<Result>> ResolveAsync(this IEnumerable<Task<Result>> resultTasks)
    {
        var results = CreateList(resultTasks);

        foreach (var result in resultTasks)
        {
            results.Add(await result.ConfigureAwait(false));
        }

        return results;

        static List<Result> CreateList(IEnumerable<Task<Result>> resultTasks)
        {
            return resultTasks switch
            {
                Task<Result>[] array => new List<Result>(array.Length),
                ICollection<Task<Result>> collection => new List<Result>(collection.Count),
                _ => [],
            };
        }
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
        var results = CreateList(resultTasks);

        foreach (var result in resultTasks)
        {
            results.Add(await result.ConfigureAwait(false));
        }

        return results;

        static List<Result<T>> CreateList(IEnumerable<Task<Result<T>>> resultTasks)
        {
            return resultTasks switch
            {
                Task<Result>[] array => new List<Result<T>>(array.Length),
                ICollection<Task<Result>> collection => new List<Result<T>>(collection.Count),
                _ => [],
            };
        }
    }
}
