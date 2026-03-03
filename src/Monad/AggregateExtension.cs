namespace ForgeSharp.Results.Monad;

/// <summary>
/// Aggregation helpers for combining validation messages from result collections.
/// </summary>
public static class AggregateExtension
{
    /// <summary>
    /// Combines validation messages from all failed results into a single result.
    /// </summary>
    /// <param name="results">The collection of <see cref="Result"/> values to inspect.</param>
    /// <returns>
    /// A successful <see cref="Result"/> when there are no non-empty failure messages; otherwise a failed <see cref="Result"/>
    /// whose message is the newline-separated concatenation of all non-empty failure messages found in <paramref name="results"/>.
    /// </returns>
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
    /// Combines validation messages from all failed results into a single result.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="results">The collection of <see cref="Result{T}"/> values to inspect.</param>
    /// <returns>
    /// A successful <see cref="Result"/> when there are no non-empty failure messages; otherwise a failed <see cref="Result"/>
    /// whose message is the newline-separated concatenation of all non-empty failure messages found in <paramref name="results"/>.
    /// </returns>
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
