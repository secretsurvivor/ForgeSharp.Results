using System.Diagnostics;

namespace ForgeSharp.Results.Monad;

/// <summary>
/// Retry operators for re-executing pipelines on failure.
/// </summary>
public static class RetryExtension
{
    private sealed class RetryImpl(IPipeline pipeline, int maxRetries) : IPipeline
    {
        public Result Execute()
        {
            int attempts = 0;
            Result result;

            do
            {
                result = pipeline.Execute();
                attempts++;
            } while (!result.IsSuccess && attempts <= maxRetries);

            return result;
        }
    }

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
        if (maxRetries < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maxRetries), "Max retries must be greater than 0.");
        }

        return new RetryImpl(pipeline, maxRetries);
    }

    private sealed class RetryWithPredicateImpl(IPipeline pipeline, int maxRetries, Func<Result, bool> predicate) : IPipeline
    {
        public Result Execute()
        {
            int attempts = 0;
            Result result;

            do
            {
                result = pipeline.Execute();
                attempts++;
            } while (!result.IsSuccess && attempts <= maxRetries && predicate(result));

            return result;
        }
    }

    /// <summary>
    /// Retries the pipeline up to <paramref name="maxRetries"/> times, only when the predicate returns true.
    /// </summary>
    /// <param name="pipeline">The pipeline to execute.</param>
    /// <param name="maxRetries">The maximum number of attempts. Must be greater than 0.</param>
    /// <param name="predicate">A function that determines whether to retry based on the failed result.</param>
    /// <returns>A pipeline that conditionally retries on failure.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="maxRetries"/> is less than 1.</exception>
    [DebuggerStepperBoundary]
    public static IPipeline Retry(this IPipeline pipeline, int maxRetries, Func<Result, bool> predicate)
    {
        if (maxRetries < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maxRetries), "Max retries must be greater than 0.");
        }

        return new RetryWithPredicateImpl(pipeline, maxRetries, predicate);
    }

    private sealed class RetryDelayImpl(IPipeline pipeline, int maxRetries, TimeSpan delay) : IPipeline
    {
        public Result Execute()
        {
            int attempts = 0;
            Result result;

            do
            {
                result = pipeline.Execute();
                attempts++;

                if (!result.IsSuccess && attempts <= maxRetries)
                {
                    Thread.Sleep(delay);
                }
            } while (!result.IsSuccess && attempts <= maxRetries);

            return result;
        }
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
        if (maxRetries < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maxRetries), "Max retries must be greater than 0.");
        }

        if (delay < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(delay), "Delay must be a non-negative TimeSpan.");
        }

        return new RetryDelayImpl(pipeline, maxRetries, delay);
    }

    private sealed class RetryImpl<T>(IPipeline<T> pipeline, int maxRetries) : IPipeline<T>
    {
        public Result<T> Execute()
        {
            int attempts = 0;
            Result<T> result;

            do
            {
                result = pipeline.Execute();
                attempts++;

            } while (!result.IsSuccess && attempts <= maxRetries);

            return result;
        }
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
        if (maxRetries < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maxRetries), "Max retries must be greater than 0.");
        }

        return new RetryImpl<T>(pipeline, maxRetries);
    }

    private sealed class RetryWithPredicateImpl<T>(IPipeline<T> pipeline, int maxRetries, Func<Result, bool> predicate) : IPipeline<T>
    {
        public Result<T> Execute()
        {
            int attempts = 0;
            Result<T> result;

            do
            {
                result = pipeline.Execute();
                attempts++;

            } while (!result.IsSuccess && attempts <= maxRetries && predicate((Result) result));

            return result;
        }
    }

    /// <summary>
    /// Retries the pipeline up to <paramref name="maxRetries"/> times, only when the predicate returns true.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="pipeline">The pipeline to execute.</param>
    /// <param name="maxRetries">The maximum number of attempts. Must be greater than 0.</param>
    /// <param name="predicate">A function that determines whether to retry based on the failed result.</param>
    /// <returns>A pipeline that conditionally retries on failure.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="maxRetries"/> is less than 1.</exception>
    [DebuggerStepperBoundary]
    public static IPipeline<T> Retry<T>(this IPipeline<T> pipeline, int maxRetries, Func<Result, bool> predicate)
    {
        if (maxRetries < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maxRetries), "Max retries must be greater than 0.");
        }

        return new RetryWithPredicateImpl<T>(pipeline, maxRetries, predicate);
    }

    private sealed class RetryDelayImpl<T>(IPipeline<T> pipeline, int maxRetries, TimeSpan delay) : IPipeline<T>
    {
        public Result<T> Execute()
        {
            int attempts = 0;
            Result<T> result;

            do
            {
                result = pipeline.Execute();
                attempts++;

                if (!result.IsSuccess && attempts <= maxRetries)
                {
                    Thread.Sleep(delay);
                }
            } while (!result.IsSuccess && attempts <= maxRetries);

            return result;
        }
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
        if (maxRetries < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maxRetries), "Max retries must be greater than 0.");
        }

        if (delay < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(delay), "Delay must be a non-negative TimeSpan.");
        }

        return new RetryDelayImpl<T>(pipeline, maxRetries, delay);
    }

    private sealed class RetryImpl<T, TError>(IPipeline<T, TError> pipeline, int maxRetries) : IPipeline<T, TError>
    {
        public Result<T, TError> Execute()
        {
            int attempts = 0;
            Result<T, TError> result;

            do
            {
                result = pipeline.Execute();
                attempts++;

            } while (!result.IsSuccess && attempts <= maxRetries);

            return result;
        }
    }

    /// <summary>
    /// Retries the pipeline up to <paramref name="maxRetries"/> times.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="pipeline">The pipeline to execute.</param>
    /// <param name="maxRetries">The maximum number of attempts. Must be greater than 0.</param>
    /// <returns>A pipeline that retries on failure.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="maxRetries"/> is less than 1.</exception>
    [DebuggerStepperBoundary]
    public static IPipeline<T, TError> Retry<T, TError>(this IPipeline<T, TError> pipeline, int maxRetries)
    {
        if (maxRetries < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maxRetries), "Max retries must be greater than 0.");
        }

        return new RetryImpl<T, TError>(pipeline, maxRetries);
    }

    private sealed class RetryWithPredicateImpl<T, TError>(IPipeline<T, TError> pipeline, int maxRetries, Func<TError, bool> predicate) : IPipeline<T, TError>
    {
        public Result<T, TError> Execute()
        {
            int attempts = 0;
            Result<T, TError> result;

            do
            {
                result = pipeline.Execute();
                attempts++;

            } while (!result.IsSuccess && attempts <= maxRetries && predicate(result.Error));

            return result;
        }
    }

    /// <summary>
    /// Retries the pipeline up to <paramref name="maxRetries"/> times, only when the error predicate returns true.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="pipeline">The pipeline to execute.</param>
    /// <param name="maxRetries">The maximum number of attempts. Must be greater than 0.</param>
    /// <param name="predicate">A function that determines whether to retry based on the error.</param>
    /// <returns>A pipeline that conditionally retries on failure.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="maxRetries"/> is less than 1.</exception>
    [DebuggerStepperBoundary]
    public static IPipeline<T, TError> Retry<T, TError>(this IPipeline<T, TError> pipeline, int maxRetries, Func<TError, bool> predicate)
    {
        if (maxRetries < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maxRetries), "Max retries must be greater than 0.");
        }

        return new RetryWithPredicateImpl<T, TError>(pipeline, maxRetries, predicate);
    }

    private sealed class RetryDelayImpl<T, TError>(IPipeline<T, TError> pipeline, int maxRetries, TimeSpan delay) : IPipeline<T, TError>
    {
        public Result<T, TError> Execute()
        {
            int attempts = 0;
            Result<T, TError> result;

            do
            {
                result = pipeline.Execute();
                attempts++;

                if (!result.IsSuccess && attempts <= maxRetries)
                {
                    Thread.Sleep(delay);
                }
            } while (!result.IsSuccess && attempts <= maxRetries);

            return result;
        }
    }

    /// <summary>
    /// Retries with a delay between attempts.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="pipeline">The pipeline to execute.</param>
    /// <param name="maxRetries">The maximum number of attempts. Must be greater than 0.</param>
    /// <param name="delay">The delay between attempts. Must be non-negative.</param>
    /// <returns>A pipeline that retries on failure with delay.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="maxRetries"/> is less than 1 or <paramref name="delay"/> is negative.</exception>
    [DebuggerStepperBoundary]
    public static IPipeline<T, TError> Retry<T, TError>(this IPipeline<T, TError> pipeline, int maxRetries, TimeSpan delay)
    {
        if (maxRetries < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maxRetries), "Max retries must be greater than 0.");
        }

        if (delay < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(delay), "Delay must be a non-negative TimeSpan.");
        }

        return new RetryDelayImpl<T, TError>(pipeline, maxRetries, delay);
    }

    private sealed class AsyncRetryImpl(IAsyncPipeline pipeline, int maxRetries) : IAsyncPipeline
    {
        public async Task<Result> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            int attempts = 0;
            Result result;

            do
            {
                result = await pipeline.ExecuteAsync(cancellationToken).ConfigureAwait(false);
                attempts++;

            } while (!cancellationToken.IsCancellationRequested && !result.IsSuccess && attempts <= maxRetries);

            return result;
        }
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
        if (maxRetries < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maxRetries), "Max retries must be greater than 0.");
        }

        return new AsyncRetryImpl(pipeline, maxRetries);
    }

    private sealed class AsyncRetryWithPredicateImpl(IAsyncPipeline pipeline, int maxRetries, Func<Result, bool> predicate) : IAsyncPipeline
    {
        public async Task<Result> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            int attempts = 0;
            Result result;

            do
            {
                result = await pipeline.ExecuteAsync(cancellationToken).ConfigureAwait(false);
                attempts++;

            } while (!cancellationToken.IsCancellationRequested && !result.IsSuccess && attempts <= maxRetries && predicate(result));

            return result;
        }
    }

    /// <summary>
    /// Async retry that only retries when the predicate returns true.
    /// </summary>
    /// <param name="pipeline">The asynchronous pipeline to execute.</param>
    /// <param name="maxRetries">The maximum number of attempts. Must be greater than 0.</param>
    /// <param name="predicate">A function that determines whether to retry based on the failed result.</param>
    /// <returns>An async pipeline that conditionally retries on failure.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="maxRetries"/> is less than 1.</exception>
    [DebuggerStepperBoundary]
    public static IAsyncPipeline RetryAsync(this IAsyncPipeline pipeline, int maxRetries, Func<Result, bool> predicate)
    {
        if (maxRetries < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maxRetries), "Max retries must be greater than 0.");
        }

        return new AsyncRetryWithPredicateImpl(pipeline, maxRetries, predicate);
    }

    private sealed class AsyncRetryDelayImpl(IAsyncPipeline pipeline, int maxRetries, TimeSpan delay) : IAsyncPipeline
    {
        public async Task<Result> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            int attempts = 0;
            Result result;

            do
            {
                result = await pipeline.ExecuteAsync().ConfigureAwait(false);
                attempts++;

                if (!result.IsSuccess && attempts <= maxRetries)
                {
                    await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                }
            } while (!cancellationToken.IsCancellationRequested && !result.IsSuccess && attempts <= maxRetries);

            return result;
        }
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
        if (maxRetries < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maxRetries), "Max retries must be greater than 0.");
        }

        if (delay < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(delay), "Delay must be a non-negative TimeSpan.");
        }

        return new AsyncRetryDelayImpl(pipeline, maxRetries, delay);
    }

    private sealed class AsyncRetryImpl<T>(IAsyncPipeline<T> pipeline, int maxRetries) : IAsyncPipeline<T>
    {
        public async Task<Result<T>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            int attempts = 0;
            Result<T> result;

            do
            {
                result = await pipeline.ExecuteAsync(cancellationToken).ConfigureAwait(false);
                attempts++;
            } while (!cancellationToken.IsCancellationRequested && !result.IsSuccess && attempts <= maxRetries);

            return result;
        }
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
        if (maxRetries < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maxRetries), "Max retries must be greater than 0.");
        }

        return new AsyncRetryImpl<T>(pipeline, maxRetries);
    }

    private sealed class AsyncRetryWithPredicateImpl<T>(IAsyncPipeline<T> pipeline, int maxRetries, Func<Result, bool> predicate) : IAsyncPipeline<T>
    {
        public async Task<Result<T>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            int attempts = 0;
            Result<T> result;

            do
            {
                result = await pipeline.ExecuteAsync(cancellationToken).ConfigureAwait(false);
                attempts++;
            } while (!cancellationToken.IsCancellationRequested && !result.IsSuccess && attempts <= maxRetries && predicate((Result) result));

            return result;
        }
    }

    /// <summary>
    /// Async retry that only retries when the predicate returns true.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="pipeline">The asynchronous pipeline to execute.</param>
    /// <param name="maxRetries">The maximum number of attempts. Must be greater than 0.</param>
    /// <param name="predicate">A function that determines whether to retry based on the failed result.</param>
    /// <returns>An async pipeline that conditionally retries on failure.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="maxRetries"/> is less than 1.</exception>
    [DebuggerStepperBoundary]
    public static IAsyncPipeline<T> RetryAsync<T>(this IAsyncPipeline<T> pipeline, int maxRetries, Func<Result, bool> predicate)
    {
        if (maxRetries < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maxRetries), "Max retries must be greater than 0.");
        }

        return new AsyncRetryWithPredicateImpl<T>(pipeline, maxRetries, predicate);
    }

    private sealed class AsyncRetryDelayImpl<T>(IAsyncPipeline<T> pipeline, int maxRetries, TimeSpan delay) : IAsyncPipeline<T>
    {
        public async Task<Result<T>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            int attempts = 0;
            Result<T> result;

            do
            {
                result = await pipeline.ExecuteAsync(cancellationToken).ConfigureAwait(false);
                attempts++;

                if (!result.IsSuccess && attempts <= maxRetries)
                {
                    await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                }
            } while (!cancellationToken.IsCancellationRequested && !result.IsSuccess && attempts <= maxRetries);

            return result;
        }
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
        if (maxRetries < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maxRetries), "Max retries must be greater than 0.");
        }

        if (delay < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(delay), "Delay must be a non-negative TimeSpan.");
        }

        return new AsyncRetryDelayImpl<T>(pipeline, maxRetries, delay);
    }

    private sealed class AsyncRetryImpl<T, TError>(IAsyncPipeline<T, TError> pipeline, int maxRetries) : IAsyncPipeline<T, TError>
    {
        public async Task<Result<T, TError>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            int attempts = 0;
            Result<T, TError> result;

            do
            {
                result = await pipeline.ExecuteAsync(cancellationToken).ConfigureAwait(false);
                attempts++;
            } while (!cancellationToken.IsCancellationRequested && !result.IsSuccess && attempts <= maxRetries);

            return result;
        }
    }

    /// <summary>
    /// Async version of <see cref="Retry{T, TError}(IPipeline{T, TError}, int)"/>.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="pipeline">The asynchronous pipeline to execute.</param>
    /// <param name="maxRetries">The maximum number of attempts. Must be greater than 0.</param>
    /// <returns>An async pipeline that retries on failure.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="maxRetries"/> is less than 1.</exception>
    [DebuggerStepperBoundary]
    public static IAsyncPipeline<T, TError> RetryAsync<T, TError>(this IAsyncPipeline<T, TError> pipeline, int maxRetries)
    {
        if (maxRetries < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maxRetries), "Max retries must be greater than 0.");
        }

        return new AsyncRetryImpl<T, TError>(pipeline, maxRetries);
    }

    private sealed class AsyncRetryWithPredicateImpl<T, TError>(IAsyncPipeline<T, TError> pipeline, int maxRetries, Func<TError, bool> predicate) : IAsyncPipeline<T, TError>
    {
        public async Task<Result<T, TError>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            int attempts = 0;
            Result<T, TError> result;

            do
            {
                result = await pipeline.ExecuteAsync(cancellationToken).ConfigureAwait(false);
                attempts++;
            } while (!cancellationToken.IsCancellationRequested && !result.IsSuccess && attempts <= maxRetries && predicate(result.Error));

            return result;
        }
    }

    /// <summary>
    /// Async retry that only retries when the error predicate returns true.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="pipeline">The asynchronous pipeline to execute.</param>
    /// <param name="maxRetries">The maximum number of attempts. Must be greater than 0.</param>
    /// <param name="predicate">A function that determines whether to retry based on the error.</param>
    /// <returns>An async pipeline that conditionally retries on failure.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="maxRetries"/> is less than 1.</exception>
    [DebuggerStepperBoundary]
    public static IAsyncPipeline<T, TError> RetryAsync<T, TError>(this IAsyncPipeline<T, TError> pipeline, int maxRetries, Func<TError, bool> predicate)
    {
        if (maxRetries < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maxRetries), "Max retries must be greater than 0.");
        }

        return new AsyncRetryWithPredicateImpl<T, TError>(pipeline, maxRetries, predicate);
    }

    private sealed class AsyncRetryDelayImpl<T, TError>(IAsyncPipeline<T, TError> pipeline, int maxRetries, TimeSpan delay) : IAsyncPipeline<T, TError>
    {
        public async Task<Result<T, TError>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            int attempts = 0;
            Result<T, TError> result;

            do
            {
                result = await pipeline.ExecuteAsync(cancellationToken).ConfigureAwait(false);
                attempts++;

                if (!result.IsSuccess && attempts <= maxRetries)
                {
                    await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                }
            } while (!cancellationToken.IsCancellationRequested && !result.IsSuccess && attempts <= maxRetries);

            return result;
        }
    }

    /// <summary>
    /// Async version of retry with a delay between attempts.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="pipeline">The asynchronous pipeline to execute.</param>
    /// <param name="maxRetries">The maximum number of attempts. Must be greater than 0.</param>
    /// <param name="delay">The delay between attempts. Must be non-negative.</param>
    /// <returns>An async pipeline that retries on failure with delay.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="maxRetries"/> is less than 1 or <paramref name="delay"/> is negative.</exception>
    [DebuggerStepperBoundary]
    public static IAsyncPipeline<T, TError> RetryAsync<T, TError>(this IAsyncPipeline<T, TError> pipeline, int maxRetries, TimeSpan delay)
    {
        if (maxRetries < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maxRetries), "Max retries must be greater than 0.");
        }

        if (delay < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(delay), "Delay must be a non-negative TimeSpan.");
        }

        return new AsyncRetryDelayImpl<T, TError>(pipeline, maxRetries, delay);
    }
}
