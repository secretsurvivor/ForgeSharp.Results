using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ForgeSharp.Results.Monad;

/// <summary>
/// Resolve operators for awaiting collections of result tasks into sequences.
/// </summary>
public static class ResolveExtension
{
#if NETSTANDARD2_1 || NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER

    /// <summary>
    /// Resolves a sequence of result tasks into an async enumerable.
    /// </summary>
    /// <param name="resultTasks">The sequence of result tasks.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous enumeration.</param>
    /// <returns>The resolved results as an async stream.</returns>
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
    /// Resolves a sequence of result tasks into an async enumerable.
    /// </summary>
    /// <param name="resultTasks">The sequence of result value tasks.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous enumeration.</param>
    /// <returns>The resolved results as an async stream.</returns>
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
    /// Resolves a sequence of result tasks into an async enumerable.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="resultTasks">The sequence of result tasks.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous enumeration.</param>
    /// <returns>The resolved results as an async stream.</returns>
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
    /// Resolves a sequence of result tasks into an async enumerable.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="resultTasks">The sequence of result value tasks.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous enumeration.</param>
    /// <returns>The resolved results as an async stream.</returns>
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
    /// Resolves a sequence of result tasks into a list.
    /// </summary>
    /// <param name="resultTasks">The sequence of result tasks.</param>
    /// <returns>The resolved results.</returns>
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
    /// Resolves a sequence of result tasks into a list.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="resultTasks">The sequence of result tasks.</param>
    /// <returns>The resolved results.</returns>
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
