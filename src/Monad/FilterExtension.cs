namespace ForgeSharp.Results.Monad;

/// <summary>
/// Filtering helpers for extracting exceptions and values from result collections.
/// </summary>
public static class FilterExtension
{
    /// <summary>
    /// Extracts all exceptions from failed <see cref="Result"/> entries in the given <see cref="EnumerableResult"/>.
    /// </summary>
    /// <param name="results">The collection of <see cref="Result"/> values to inspect.</param>
    /// <returns>
    /// A read-only list of exceptions contained in failed results, preserving the order they appeared in <paramref name="results"/>.
    /// If no failed results contain an exception, an empty list is returned.
    /// </returns>
    public static IReadOnlyList<Exception> FilterExceptions(this EnumerableResult results)
    {
        var list = new List<Exception>(results.FailureCount);

        results.ForEach(result =>
        {
            if (result.Exception is not null)
            {
                list.Add(result.Exception);
            }
        });

        return [.. list];
    }

    /// <summary>
    /// Extracts all exceptions from failed <see cref="Result{T}"/> entries in the given <see cref="EnumerableResult{T}"/>.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="results">The collection of <see cref="Result{T}"/> values to inspect.</param>
    /// <returns>
    /// A read-only list of exceptions contained in failed results, preserving the order they appeared in <paramref name="results"/>.
    /// If no failed results contain an exception, an empty list is returned.
    /// </returns>
    public static IReadOnlyList<Exception> FilterExceptions<T>(this EnumerableResult<T> results)
    {
        var list = new List<Exception>(results.FailureCount);

        results.ForEach(result =>
        {
            if (result.Exception is not null)
            {
                list.Add(result.Exception);
            }
        });

        return [.. list];
    }

    /// <summary>
    /// Extracts the successful payload values from the provided <see cref="EnumerableResult{T}"/>.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="results">The collection of <see cref="Result{T}"/> values to inspect.</param>
    /// <returns>
    /// An array containing the values from each successful result, in the same order they appeared in <paramref name="results"/>.
    /// If there are no successful results, an empty array is returned.
    /// </returns>
    public static IReadOnlyList<T> FilterValues<T>(this EnumerableResult<T> results)
    {
        var array = new T[results.SuccessCount];

        var index = 0;

        results.ForEach(result =>
        {
            if (result.IsSuccess)
            {
                array[index] = result.Value;
                index++;
            }
        });

        return array;
    }
}
