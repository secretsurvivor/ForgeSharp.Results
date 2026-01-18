namespace ForgeSharp.Results.Monad;

/// <summary>
/// Provides extension methods to aggregate validation messages from collections of <see cref="Result"/> or <see cref="Result{T}"/>.
/// </summary>
public static class AggregateExtension
{
    /// <summary>
    /// Aggregates validation messages from the provided <see cref="EnumerableResult"/>.
    /// </summary>
    /// <param name="results">The collection of <see cref="Result"/> values to inspect.</param>
    /// <returns>
    /// A successful <see cref="Result"/> when there are no non-empty failure messages; otherwise a failed <see cref="Result"/>
    /// whose message is the newline-separated concatenation of all non-empty failure messages found in <paramref name="results"/>.
    /// </returns>
    /// <remarks>
    /// - If <paramref name="results"/> is empty, this method returns <c>Result.Ok()</c>.
    /// - Only non-empty <see cref="Result.Message"/> values from failed results are included.
    /// - The aggregation preserves the iteration order and runs in O(n) time where n is <see cref="EnumerableResult.Total"/>.
    /// </remarks>
    public static Result AggregateValidation(this EnumerableResult results)
    {
        if (results.Total == 0)
        {
            return Result.Ok();
        }

        var messages = new List<string>(results.FailureCount);

        results.ForEach(result =>
        {
            if (!string.IsNullOrEmpty(result.Message))
            {
                messages.Add(result.Message);
            }
        });

        if (messages.Count > 0)
        {
            return Result.Fail(string.Join(Environment.NewLine, messages));
        }

        return Result.Ok();
    }

    /// <summary>
    /// Aggregates validation messages from the provided <see cref="EnumerableResult{T}"/>.
    /// </summary>
    /// <typeparam name="T">The payload type carried by each <see cref="Result{T}"/>.</typeparam>
    /// <param name="results">The collection of <see cref="Result{T}"/> values to inspect.</param>
    /// <returns>
    /// A successful <see cref="Result"/> when there are no non-empty failure messages; otherwise a failed <see cref="Result"/>
    /// whose message is the newline-separated concatenation of all non-empty failure messages found in <paramref name="results"/>.
    /// </returns>
    /// <remarks>
    /// - If <paramref name="results"/> is empty, this method returns <c>Result.Ok()</c>.
    /// - Only non-empty <see cref="Result{T}.Message"/> values from failed results are included.
    /// - The aggregation preserves the iteration order and runs in O(n) time where n is <see cref="EnumerableResult{T}.Total"/>.
    /// </remarks>
    public static Result AggregateValidation<T>(this EnumerableResult<T> results)
    {
        if (results.Total == 0)
        {
            return Result.Ok();
        }

        var messages = new List<string>(results.FailureCount);

        results.ForEach(result =>
        {
            if (!string.IsNullOrEmpty(result.Message))
            {
                messages.Add(result.Message);
            }
        });

        if (messages.Count > 0)
        {
            return Result.Fail(string.Join(Environment.NewLine, messages));
        }

        return Result.Ok();
    }
}
