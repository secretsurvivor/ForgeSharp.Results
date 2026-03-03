using System.Diagnostics;

namespace ForgeSharp.Results.Monad;

/// <summary>
/// Retry operators for re-executing pipelines on failure.
/// </summary>
public static class RetryExtension
{
    /// <summary>
    /// Retries the pipeline up to <paramref name="maxRetries"/> times.
    /// </summary>
    /// <param name="pipeline">The pipeline to execute.</param>
    /// <param name="maxRetries">The maximum number of attempts. Must be greater than 0.</param>
    /// <returns>A pipeline that retries on failure.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="maxRetries"/> is less than 1.</exception>
    [DebuggerStepperBoundary]
    public static IPipeline Retry(this IPipeline pipeline, int maxRetries)
    {
        return Pipeline.Create(() =>
        {
            if (maxRetries < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxRetries), "Max retries must be greater than 0.");
            }
                
            int attempts = 0;
            Result result;

            do
            {
                result = pipeline.Execute();
                attempts++;
            } while (!result.IsSuccess && attempts < maxRetries);

            return result;
        });
    }

    /// <summary>
    /// Retries with a delay between attempts.
    /// </summary>
    /// <param name="pipeline">The pipeline to execute.</param>
    /// <param name="maxRetries">The maximum number of attempts. Must be greater than 0.</param>
    /// <param name="delay">The delay between attempts. Must be non-negative.</param>
    /// <returns>A pipeline that retries on failure with delay.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="maxRetries"/> is less than 1 or <paramref name="delay"/> is negative.</exception>
    [DebuggerStepperBoundary]
    public static IPipeline Retry(this IPipeline pipeline, int maxRetries, TimeSpan delay)
    {
        return Pipeline.Create(() =>
        {
            if (maxRetries < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxRetries), "Max retries must be greater than 0.");
            }

            if (delay < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(delay), "Delay must be a non-negative TimeSpan.");
            }

            int attempts = 0;
            Result result;

            do
            {
                result = pipeline.Execute();
                attempts++;

                if (!result.IsSuccess && attempts < maxRetries)
                {
                    Task.Delay(delay).Wait();
                }

            } while (!result.IsSuccess && attempts < maxRetries);

            return result;
        });
    }

    /// <summary>
    /// Async version of <see cref="Retry(IPipeline, int)"/>.
    /// </summary>
    /// <param name="pipeline">The asynchronous pipeline to execute.</param>
    /// <param name="maxRetries">The maximum number of attempts. Must be greater than 0.</param>
    /// <returns>An async pipeline that retries on failure.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="maxRetries"/> is less than 1.</exception>
    [DebuggerStepperBoundary]
    public static IAsyncPipeline RetryAsync(this IAsyncPipeline pipeline, int maxRetries)
    {
        return Pipeline.Create(async () =>
        {
            if (maxRetries < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxRetries), "Max retries must be greater than 0.");
            }

            int attempts = 0;
            Result result;

            do
            {
                result = await pipeline.ExecuteAsync().ConfigureAwait(false);
                attempts++;

            } while (!result.IsSuccess && attempts < maxRetries);

            return result;
        });
    }

    /// <summary>
    /// Async version of <see cref="Retry(IPipeline, int, TimeSpan)"/>. Retries with a delay between attempts.
    /// </summary>
    /// <param name="pipeline">The asynchronous pipeline to execute.</param>
    /// <param name="maxRetries">The maximum number of attempts. Must be greater than 0.</param>
    /// <param name="delay">The delay between attempts. Must be non-negative.</param>
    /// <returns>An async pipeline that retries on failure with delay.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="maxRetries"/> is less than 1 or <paramref name="delay"/> is negative.</exception>
    [DebuggerStepperBoundary]
    public static IAsyncPipeline RetryAsync(this IAsyncPipeline pipeline, int maxRetries, TimeSpan delay)
    {
        return Pipeline.Create(async () =>
        {
            if (maxRetries < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxRetries), "Max retries must be greater than 0.");
            }

            if (delay < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(delay), "Delay must be a non-negative TimeSpan.");
            }

            int attempts = 0;
            Result result;

            do
            {
                result = await pipeline.ExecuteAsync().ConfigureAwait(false);
                attempts++;

                if (!result.IsSuccess && attempts < maxRetries)
                {
                    await Task.Delay(delay).ConfigureAwait(false);
                }

            } while (!result.IsSuccess && attempts < maxRetries);

            return result;
        });
    }

    /// <summary>
    /// Retries the pipeline up to <paramref name="maxRetries"/> times.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="pipeline">The pipeline to execute.</param>
    /// <param name="maxRetries">The maximum number of attempts. Must be greater than 0.</param>
    /// <returns>A pipeline that retries on failure.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="maxRetries"/> is less than 1.</exception>
    [DebuggerStepperBoundary]
    public static IPipeline<T> Retry<T>(this IPipeline<T> pipeline, int maxRetries)
    {
        return Pipeline.Create<T>(() =>
        {
            if (maxRetries < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxRetries), "Max retries must be greater than 0.");
            }

            int attempts = 0;
            Result<T> result;

            do
            {
                result = pipeline.Execute();
                attempts++;

            } while (!result.IsSuccess && attempts < maxRetries);

            return result;
        });
    }

    /// <summary>
    /// Retries with a delay between attempts.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="pipeline">The pipeline to execute.</param>
    /// <param name="maxRetries">The maximum number of attempts. Must be greater than 0.</param>
    /// <param name="delay">The delay between attempts. Must be non-negative.</param>
    /// <returns>A pipeline that retries on failure with delay.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="maxRetries"/> is less than 1 or <paramref name="delay"/> is negative.</exception>
    [DebuggerStepperBoundary]
    public static IPipeline<T> Retry<T>(this IPipeline<T> pipeline, int maxRetries, TimeSpan delay)
    {
        return Pipeline.Create<T>(() =>
        {
            if (maxRetries < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxRetries), "Max retries must be greater than 0.");
            }

            if (delay < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(delay), "Delay must be a non-negative TimeSpan.");
            }

            int attempts = 0;
            Result<T> result;

            do
            {
                result = pipeline.Execute();
                attempts++;

                if (!result.IsSuccess && attempts < maxRetries)
                {
                    Task.Delay(delay).Wait();
                }

            } while (!result.IsSuccess && attempts < maxRetries);

            return result;
        });
    }

    /// <summary>
    /// Async version of <see cref="Retry{T}(IPipeline{T}, int)"/>.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="pipeline">The asynchronous pipeline to execute.</param>
    /// <param name="maxRetries">The maximum number of attempts. Must be greater than 0.</param>
    /// <returns>An async pipeline that retries on failure.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="maxRetries"/> is less than 1.</exception>
    [DebuggerStepperBoundary]
    public static IAsyncPipeline<T> RetryAsync<T>(this IAsyncPipeline<T> pipeline, int maxRetries)
    {
        return Pipeline.Create<T>(async () =>
        {
            if (maxRetries < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxRetries), "Max retries must be greater than 0.");
            }

            int attempts = 0;
            Result<T> result;

            do
            {
                result = await pipeline.ExecuteAsync().ConfigureAwait(false);
                attempts++;
            } while (!result.IsSuccess && attempts < maxRetries);

            return result;
        });
    }

    /// <summary>
    /// Async version of <see cref="Retry{T}(IPipeline{T}, int, TimeSpan)"/>. Retries with a delay between attempts.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="pipeline">The asynchronous pipeline to execute.</param>
    /// <param name="maxRetries">The maximum number of attempts. Must be greater than 0.</param>
    /// <param name="delay">The delay between attempts. Must be non-negative.</param>
    /// <returns>An async pipeline that retries on failure with delay.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="maxRetries"/> is less than 1 or <paramref name="delay"/> is negative.</exception>
    [DebuggerStepperBoundary]
    public static IAsyncPipeline<T> RetryAsync<T>(this IAsyncPipeline<T> pipeline, int maxRetries, TimeSpan delay)
    {
        return Pipeline.Create<T>(async () =>
        {
            if (maxRetries < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxRetries), "Max retries must be greater than 0.");
            }

            if (delay < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(delay), "Delay must be a non-negative TimeSpan.");
            }

            int attempts = 0;
            Result<T> result;

            do
            {
                result = await pipeline.ExecuteAsync().ConfigureAwait(false);
                attempts++;

                if (!result.IsSuccess && attempts < maxRetries)
                {
                    await Task.Delay(delay).ConfigureAwait(false);
                }

            } while (!result.IsSuccess && attempts < maxRetries);

            return result;
        });
    }
}
