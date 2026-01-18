using System.Runtime.CompilerServices;

namespace ForgeSharp.Results;

/// <summary>
/// A read-only, array-backed wrapper around a sequence of <see cref="Result"/> values that provides
/// aggregated counts and iteration helpers.
/// </summary>
public readonly struct EnumerableResult
{
    internal readonly Result[] _results;

    /// <summary>
    /// Gets the underlying results as an <see cref="IReadOnlyList{Result}"/>.
    /// The returned list is the internal array and should be treated as read-only by consumers.
    /// </summary>
    public IReadOnlyList<Result> Results
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _results;
    }

    /// <summary>
    /// Gets the total number of results in the collection.
    /// </summary>
    public int Total
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _results.Length;
    }

    private readonly int _successCount;
    private readonly int _failureCount;

    /// <summary>
    /// Gets the number of successful results in the collection.
    /// </summary>
    public int SuccessCount => _successCount;

    /// <summary>
    /// Gets the number of failed results in the collection.
    /// </summary>
    public int FailureCount => _failureCount;

    /// <summary>
    /// Gets a value indicating whether all results in the collection are successful.
    /// </summary>
    public bool IsSuccess => _failureCount == 0;

    /// <summary>
    /// Gets the ratio of successful results to total results (0.0 when the collection is empty).
    /// </summary>
    public double SuccessRatio => _results.Length == 0 ? 0d : (double)_successCount / _results.Length;

    /// <summary>
    /// Gets the success percentage (0.0 to 100.0) computed from <see cref="SuccessRatio"/>.
    /// </summary>
    public double SuccessPercentage => SuccessRatio * 100d;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private EnumerableResult(Result[] results)
    {
        _results = results;
        CalculateCount(results, out _successCount, out _failureCount);
    }

    private static void CalculateCount(Result[] results, out int successCount, out int failureCount)
    {
        successCount = 0;
        failureCount = 0;

        for (int i = 0; i < results.Length; i++)
        {
            if (results[i].IsSuccess)
            {
                successCount++;
            }
            else
            {
                failureCount++;
            }
        }
    }

    /// <summary>
    /// Creates an <see cref="EnumerableResult"/> from the provided sequence of <see cref="Result"/>.
    /// </summary>
    /// <param name="results">The sequence of results to wrap. Cannot be <c>null</c>.</param>
    /// <returns>An <see cref="EnumerableResult"/> representing the provided results.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="results"/> is <c>null</c>.</exception>
    public static EnumerableResult Create(IEnumerable<Result> results)
    {
#if NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(results);
#else
            if (results is null)
            {
                throw new ArgumentNullException(nameof(results));
            }
#endif

        return results switch
        {
            Result[] array => new EnumerableResult(array),
            ICollection<Result> { Count: 0 } => new EnumerableResult([]),
            _ => new EnumerableResult([.. results])
        };
    }

    /// <summary>
    /// Creates a typed <see cref="EnumerableResult{T}"/> from the provided sequence of <see cref="Result{T}"/>.
    /// </summary>
    /// <typeparam name="T">The payload type of the generic results.</typeparam>
    /// <param name="results">The sequence of generic results to wrap. Cannot be <c>null</c>.</param>
    /// <returns>An <see cref="EnumerableResult{T}"/> representing the provided results.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="results"/> is <c>null</c>.</exception>
    public static EnumerableResult<T> Create<T>(IEnumerable<Result<T>> results)
    {
#if NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(results);
#else
            if (results is null)
            {
                throw new ArgumentNullException(nameof(results));
            }
#endif

        return results switch
        {
            Result<T>[] array => new EnumerableResult<T>(array),
            ICollection<Result<T>> { Count: 0 } => new EnumerableResult<T>([]),
            _ => new EnumerableResult<T>([.. results])
        };
    }

#if NETSTANDARD2_1 || NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
    /// <summary>
    /// Returns a <see cref="ReadOnlySpan{T}"/> over the internal results array for low-allocation iteration.
    /// </summary>
    public readonly ReadOnlySpan<Result> AsSpan()
    {
        return _results;
    }
#endif

    /// <summary>
    /// Executes the specified <paramref name="action"/> once for each element in the collection.
    /// </summary>
    /// <param name="action">The action to perform for each <see cref="Result"/>. Cannot be <c>null</c>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="action"/> is <c>null</c>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void ForEach(Action<Result> action)
    {
#if NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(action);
#else
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }
#endif

        var list = _results;

        for (int i = 0; i < list.Length; i++)
        {
            action(list[i]);
        }
    }

    /// <summary>
    /// Executes <paramref name="action"/> for each element that satisfies <paramref name="predicate"/>.
    /// </summary>
    /// <param name="predicate">A function that determines whether <paramref name="action"/> should be executed for a given item. Cannot be <c>null</c>.</param>
    /// <param name="action">The action to perform for matching items. Cannot be <c>null</c>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> or <paramref name="action"/> is <c>null</c>.</exception>
    public readonly void ForEach(Func<Result, bool> predicate, Action<Result> action)
    {
#if NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(action);
#else
            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }
#endif

        var list = _results;

        for (int i = 0; i < list.Length; i++)
        {
            var r = list[i];

            if (predicate(r))
            {
                action(r);
            }
        }
    }

    /// <summary>
    /// Iterates the collection and invokes the provided value-type visitor for each element.
    /// Using a struct-based visitor avoids delegate allocations during iteration.
    /// </summary>
    /// <typeparam name="TVisitor">A struct implementing <see cref="IResultVisitor"/>.</typeparam>
    /// <param name="visiter">The visitor instance whose <see cref="IResultVisitor.Visit(Result)"/> method will be called for each item.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void ForEach<TVisitor>(TVisitor visiter) where TVisitor : struct, IResultVisitor
    {
        var list = _results;

        for (int i = 0; i < list.Length; i++)
        {
            visiter.Visit(list[i]);
        }
    }
}

/// <summary>
/// A read-only, array-backed wrapper around a sequence of <see cref="Result{T}"/> values that provides
/// aggregated counts and iteration helpers.
/// </summary>
/// <typeparam name="T">The payload type carried by each <see cref="Result{T}"/>.</typeparam>
public readonly struct EnumerableResult<T>
{
    internal readonly Result<T>[] _results;

    /// <summary>
    /// Gets the underlying generic results as an <see cref="IReadOnlyList{Result}"/>.
    /// The returned list is the internal array and should be treated as read-only by consumers.
    /// </summary>
    public IReadOnlyList<Result<T>> Results
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _results;
    }

    /// <summary>
    /// Gets the total number of results in the collection.
    /// </summary>
    public int Total
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _results.Length;
    }

    private readonly int _successCount;
    private readonly int _failureCount;

    /// <summary>
    /// Gets the number of successful results in the collection.
    /// </summary>
    public int SuccessCount => _successCount;

    /// <summary>
    /// Gets the number of failed results in the collection.
    /// </summary>
    public int FailureCount => _failureCount;

    /// <summary>
    /// Gets a value indicating whether all results in the collection are successful.
    /// </summary>
    public bool IsSuccess => _failureCount == 0;

    /// <summary>
    /// Gets the ratio of successful results to total results (0.0 when the collection is empty).
    /// </summary>
    public double SuccessRatio => _results.Length == 0 ? 0d : (double)_successCount / _results.Length;

    /// <summary>
    /// Gets the success percentage (0.0 to 100.0) computed from <see cref="SuccessRatio"/>.
    /// </summary>
    public double SuccessPercentage => SuccessRatio * 100d;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal EnumerableResult(Result<T>[] results)
    {
        _results = results;
        CalculateCount(results, out _successCount, out _failureCount);
    }

    private static void CalculateCount(Result<T>[] results, out int successCount, out int failureCount)
    {
        successCount = 0;
        failureCount = 0;

        for (int i = 0; i < results.Length; i++)
        {
            if (results[i].IsSuccess)
            {
                successCount++;
            }
            else
            {
                failureCount++;
            }
        }
    }

    /// <summary>
    /// Creates an <see cref="EnumerableResult{T}"/> from the provided sequence of <see cref="Result{T}"/>.
    /// </summary>
    /// <param name="results">The sequence of generic results to wrap. Cannot be <c>null</c>.</param>
    /// <returns>An <see cref="EnumerableResult{T}"/> representing the provided results.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="results"/> is <c>null</c>.</exception>
    public static EnumerableResult<T> Create(IEnumerable<Result<T>> results)
    {
#if NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(results);
#else
            if (results is null)
            {
                throw new ArgumentNullException(nameof(results));
            }
#endif

        return results switch
        {
            Result<T>[] array => new EnumerableResult<T>(array),
            ICollection<Result<T>> { Count: 0 } => new EnumerableResult<T>([]),
            _ => new EnumerableResult<T>([.. results])
        };
    }

#if NETSTANDARD2_1 || NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
    /// <summary>
    /// Returns a <see cref="ReadOnlySpan{T}"/> over the internal generic results array for low-allocation iteration.
    /// </summary>
    public readonly ReadOnlySpan<Result<T>> AsSpan()
    {
        return _results;
    }
#endif

    /// <summary>
    /// Executes the specified <paramref name="action"/> once for each element in the generic collection.
    /// </summary>
    /// <param name="action">The action to perform for each <see cref="Result{T}"/>. Cannot be <c>null</c>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="action"/> is <c>null</c>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void ForEach(Action<Result<T>> action)
    {
#if NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(action);
#else
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }
#endif

        var list = _results;

        for (int i = 0; i < list.Length; i++)
        {
            action(list[i]);
        }
    }

    /// <summary>
    /// Executes <paramref name="action"/> for each generic element that satisfies <paramref name="predicate"/>.
    /// </summary>
    /// <param name="predicate">A function that determines whether <paramref name="action"/> should be executed for a given item. Cannot be <c>null</c>.</param>
    /// <param name="action">The action to perform for matching items. Cannot be <c>null</c>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> or <paramref name="action"/> is <c>null</c>.</exception>
    public readonly void ForEach(Func<Result<T>, bool> predicate, Action<Result<T>> action)
    {
#if NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(action);
#else
            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }
#endif

        var list = _results;

        for (int i = 0; i < list.Length; i++)
        {
            var r = list[i];

            if (predicate(r))
            {
                action(r);
            }
        }
    }

    /// <summary>
    /// Iterates the generic collection and invokes the provided value-type visitor for each element.
    /// Using a struct-based visitor avoids delegate allocations during iteration.
    /// </summary>
    /// <typeparam name="TVisitor">A struct implementing <see cref="IResultVisitor{T}"/>.</typeparam>
    /// <param name="visiter">The visitor instance whose <see cref="IResultVisitor{T}.Visit(Result{T})"/> method will be called for each item.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void ForEach<TVisitor>(TVisitor visiter) where TVisitor : struct, IResultVisitor<T>
    {
        var list = _results;

        for (int i = 0; i < list.Length; i++)
        {
            visiter.Visit(list[i]);
        }
    }
}

/// <summary>
/// Visitor interface used with <see cref="EnumerableResult.ForEach{TVisitor}"/> to process <see cref="Result"/> items
/// without allocations (visitor should be a value type).
/// </summary>
public interface IResultVisitor
{
    /// <summary>
    /// Called for each <see cref="Result"/> element in the collection when using a struct visitor.
    /// </summary>
    /// <param name="result">The current <see cref="Result"/> being visited.</param>
    void Visit(Result result);
}

/// <summary>
/// Visitor interface used with <see cref="EnumerableResult{T}.ForEach{TVisitor}"/> to process <see cref="Result{T}"/> items
/// without allocations (visitor should be a value type).
/// </summary>
/// <typeparam name="T">The payload type carried by each <see cref="Result{T}"/>.</typeparam>
public interface IResultVisitor<T>
{
    /// <summary>
    /// Called for each <see cref="Result{T}"/> element in the collection when using a struct visitor.
    /// </summary>
    /// <param name="result">The current <see cref="Result{T}"/> being visited.</param>
    void Visit(Result<T> result);
}
