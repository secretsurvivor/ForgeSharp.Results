using System.ComponentModel;
using System.Diagnostics;

namespace ForgeSharp.Results.Monad;

/// <summary>
/// LINQ query syntax support for <see cref="IPipeline{T}"/> and <see cref="IAsyncPipeline{T}"/>.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class LinqExtension
{
    /// <summary>
    /// LINQ <c>select</c> support for sync pipelines.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never), DebuggerStepperBoundary]
    public static IPipeline<TResult> Select<T, TResult>(this IPipeline<T> pipeline, Func<T, TResult> selector)
    {
        return Pipeline.Create(() => {
            var result = pipeline.Execute();

            return result.IsSuccess ? Result.Ok(selector(result.Value)) : result.As<TResult>();
        });
    }

    /// <summary>
    /// LINQ <c>where</c> support for sync pipelines.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never), DebuggerStepperBoundary]
    public static IPipeline<T> Where<T>(this IPipeline<T> pipeline, Func<T, bool> predicate)
    {
        return Pipeline.Create(() => {
            var result = pipeline.Execute();

            if (!result.IsSuccess)
            {
                return result;
            }

            return predicate(result.Value) ? result : Result.Fail<T>("Predicate not satisfied.");
        });
    }

    /// <summary>
    /// LINQ <c>from...select</c> (SelectMany) support for sync pipelines.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never), DebuggerStepperBoundary]
    public static IPipeline<TResult> SelectMany<T, TIntermediate, TResult>(
        this IPipeline<T> pipeline,
        Func<T, IPipeline<TIntermediate>> selector,
        Func<T, TIntermediate, TResult> resultSelector)
    {
        return Pipeline.Create(() => {
            var result = pipeline.Execute();

            if (!result.IsSuccess)
            {
                return result.As<TResult>();
            }

            var intermediate = selector(result.Value).Execute();

            if (!intermediate.IsSuccess)
            {
                return intermediate.As<TResult>();
            }

            return Result.Ok(resultSelector(result.Value, intermediate.Value));
        });
    }

    /// <summary>
    /// LINQ <c>select</c> support for async pipelines.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never), DebuggerStepperBoundary]
    public static IAsyncPipeline<TResult> Select<T, TResult>(this IAsyncPipeline<T> pipeline, Func<T, TResult> selector)
    {
        return Pipeline.Create<TResult>(async () => {
            var result = await pipeline.ExecuteAsync().ConfigureAwait(false);

            return result.IsSuccess ? Result.Ok(selector(result.Value)) : result.As<TResult>();
        });
    }

    /// <summary>
    /// LINQ <c>where</c> support for async pipelines.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never), DebuggerStepperBoundary]
    public static IAsyncPipeline<T> Where<T>(this IAsyncPipeline<T> pipeline, Func<T, bool> predicate)
    {
        return Pipeline.Create<T>(async () => {
            var result = await pipeline.ExecuteAsync().ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                return result;
            }

            return predicate(result.Value) ? result : Result.Fail<T>("Predicate not satisfied.");
        });
    }

    /// <summary>
    /// LINQ <c>from...select</c> (SelectMany) support for async pipelines.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never), DebuggerStepperBoundary]
    public static IAsyncPipeline<TResult> SelectMany<T, TIntermediate, TResult>(
        this IAsyncPipeline<T> pipeline,
        Func<T, IAsyncPipeline<TIntermediate>> selector,
        Func<T, TIntermediate, TResult> resultSelector)
    {
        return Pipeline.Create<TResult>(async () => {
            var result = await pipeline.ExecuteAsync().ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                return result.As<TResult>();
            }

            var intermediate = await selector(result.Value).ExecuteAsync().ConfigureAwait(false);

            if (!intermediate.IsSuccess)
            {
                return intermediate.As<TResult>();
            }

            return Result.Ok(resultSelector(result.Value, intermediate.Value));
        });
    }

    /// <summary>
    /// Mixed sync→async <c>SelectMany</c>.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never), DebuggerStepperBoundary]
    public static IAsyncPipeline<TResult> SelectMany<T, TIntermediate, TResult>(
        this IPipeline<T> pipeline,
        Func<T, IAsyncPipeline<TIntermediate>> selector,
        Func<T, TIntermediate, TResult> resultSelector)
    {
        return Pipeline.Create<TResult>(async () => {
            var result = pipeline.Execute();

            if (!result.IsSuccess)
            {
                return result.As<TResult>();
            }

            var intermediate = await selector(result.Value).ExecuteAsync().ConfigureAwait(false);

            if (!intermediate.IsSuccess)
            {
                return intermediate.As<TResult>();
            }

            return Result.Ok(resultSelector(result.Value, intermediate.Value));
        });
    }

    /// <summary>
    /// Mixed async→sync <c>SelectMany</c>.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never), DebuggerStepperBoundary]
    public static IAsyncPipeline<TResult> SelectMany<T, TIntermediate, TResult>(
        this IAsyncPipeline<T> pipeline,
        Func<T, IPipeline<TIntermediate>> selector,
        Func<T, TIntermediate, TResult> resultSelector)
    {
        return Pipeline.Create<TResult>(async () => {
            var result = await pipeline.ExecuteAsync().ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                return result.As<TResult>();
            }

            var intermediate = selector(result.Value).Execute();

            if (!intermediate.IsSuccess)
            {
                return intermediate.As<TResult>();
            }

            return Result.Ok(resultSelector(result.Value, intermediate.Value));
        });
    }

    /// <summary>
    /// Async <c>SelectMany</c> with async selector.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never), DebuggerStepperBoundary]
    public static IAsyncPipeline<TResult> SelectMany<T, TIntermediate, TResult>(
        this IAsyncPipeline<T> pipeline,
        Func<T, Task<IAsyncPipeline<TIntermediate>>> selector,
        Func<T, TIntermediate, TResult> resultSelector)
    {
        return Pipeline.Create<TResult>(async () => {
            var result = await pipeline.ExecuteAsync().ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                return result.As<TResult>();
            }

            var intermediatePipeline = await selector(result.Value).ConfigureAwait(false);
            var intermediate = await intermediatePipeline.ExecuteAsync().ConfigureAwait(false);

            if (!intermediate.IsSuccess)
            {
                return intermediate.As<TResult>();
            }

            return Result.Ok(resultSelector(result.Value, intermediate.Value));
        });
    }

    /// <summary>
    /// Async <c>SelectMany</c> with async selector returning sync pipeline.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never), DebuggerStepperBoundary]
    public static IAsyncPipeline<TResult> SelectMany<T, TIntermediate, TResult>(
        this IAsyncPipeline<T> pipeline,
        Func<T, Task<IPipeline<TIntermediate>>> selector,
        Func<T, TIntermediate, TResult> resultSelector)
    {
        return Pipeline.Create<TResult>(async () => {
            var result = await pipeline.ExecuteAsync().ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                return result.As<TResult>();
            }

            var intermediatePipeline = await selector(result.Value).ConfigureAwait(false);
            var intermediate = intermediatePipeline.Execute();

            if (!intermediate.IsSuccess)
            {
                return intermediate.As<TResult>();
            }

            return Result.Ok(resultSelector(result.Value, intermediate.Value));
        });
    }

    /// <summary>
    /// Sync pipeline with async selector returning async pipeline.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never), DebuggerStepperBoundary]
    public static IAsyncPipeline<TResult> SelectMany<T, TIntermediate, TResult>(
        this IPipeline<T> pipeline,
        Func<T, Task<IAsyncPipeline<TIntermediate>>> selector,
        Func<T, TIntermediate, TResult> resultSelector)
    {
        return Pipeline.Create<TResult>(async () => {
            var result = pipeline.Execute();

            if (!result.IsSuccess)
            {
                return result.As<TResult>();
            }

            var intermediatePipeline = await selector(result.Value).ConfigureAwait(false);
            var intermediate = await intermediatePipeline.ExecuteAsync().ConfigureAwait(false);

            if (!intermediate.IsSuccess)
            {
                return intermediate.As<TResult>();
            }

            return Result.Ok(resultSelector(result.Value, intermediate.Value));
        });
    }

    /// <summary>
    /// Sync pipeline with async selector returning sync pipeline.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never), DebuggerStepperBoundary]
    public static IAsyncPipeline<TResult> SelectMany<T, TIntermediate, TResult>(
        this IPipeline<T> pipeline,
        Func<T, Task<IPipeline<TIntermediate>>> selector,
        Func<T, TIntermediate, TResult> resultSelector)
    {
        return Pipeline.Create<TResult>(async () => {
            var result = pipeline.Execute();

            if (!result.IsSuccess)
            {
                return result.As<TResult>();
            }

            var intermediatePipeline = await selector(result.Value).ConfigureAwait(false);
            var intermediate = intermediatePipeline.Execute();

            if (!intermediate.IsSuccess)
            {
                intermediate.As<TResult>();
            }

            return Result.Ok(resultSelector(result.Value, intermediate.Value));
        });
    }
}
