namespace ForgeSharp.Results.Monad;

/// <summary>
/// Provides LINQ-like projection helpers for <see cref="EnumerableResult"/> and <see cref="EnumerableResult{T}"/>.
/// The projection is performed lazily as the returned <see cref="IEnumerable{T}"/> is iterated.
/// </summary>
public static class SelectExtension
{
    /// <summary>
    /// Projects each <see cref="Result"/> in <paramref name="enumerableResult"/> into a new form using the supplied <paramref name="selector"/>.
    /// </summary>
    /// <typeparam name="T">The element type produced by the selector.</typeparam>
    /// <param name="enumerableResult">The source <see cref="EnumerableResult"/> whose elements will be projected.</param>
    /// <param name="selector">A transform function to apply to each <see cref="Result"/>. This function is invoked for each element during enumeration.</param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> that yields the projected values in the same order as the source results.
    /// Enumeration is lazy and will invoke <paramref name="selector"/> per element when iterated.
    /// </returns>
    /// <remarks>
    /// The method does not perform argument validation — providing a <c>null</c> <paramref name="selector"/> will cause an exception when the returned sequence is enumerated.
    /// </remarks>
    public static IEnumerable<T> Select<T>(this EnumerableResult enumerableResult, Func<Result, T> selector)
    {
        var results = enumerableResult._results;

        for (var i = 0; i < results.Length; i++)
        {
            yield return selector(results[i]);
        }
    }

    /// <summary>
    /// Projects each <see cref="Result{T}"/> in <paramref name="enumerableResult"/> into a new form using the supplied <paramref name="selector"/>.
    /// </summary>
    /// <typeparam name="T">The payload type carried by each source <see cref="Result{T}"/>.</typeparam>
    /// <typeparam name="TResult">The element type produced by the selector.</typeparam>
    /// <param name="enumerableResult">The source <see cref="EnumerableResult{T}"/> whose elements will be projected.</param>
    /// <param name="selector">A transform function to apply to each <see cref="Result{T}"/>. This function is invoked for each element during enumeration.</param>
    /// <returns>
    /// An <see cref="IEnumerable{TResult}"/> that yields the projected values in the same order as the source results.
    /// Enumeration is lazy and will invoke <paramref name="selector"/> per element when iterated.
    /// </returns>
    /// <remarks>
    /// The method does not perform argument validation — providing a <c>null</c> <paramref name="selector"/> will cause an exception when the returned sequence is enumerated.
    /// </remarks>
    public static IEnumerable<TResult> Select<T, TResult>(this EnumerableResult<T> enumerableResult, Func<Result<T>, TResult> selector)
    {
        var results = enumerableResult._results;

        for (var i = 0; i < results.Length; i++)
        {
            yield return selector(results[i]);
        }
    }
}
