using System.ComponentModel;
using System.Diagnostics;

namespace ForgeSharp.Results.Monad;

/// <summary>
/// Provides LINQ extension methods for composing and transforming <see cref="IPipeline{T}"/> and <see cref="IAsyncPipeline{T}"/> instances.
/// These methods are intended to be used via LINQ query expressions to enable monadic and query-based workflows over pipelines.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class LinqExtention
{
    /// <summary>
    /// Projects each element of a pipeline into a new form. Intended for use with LINQ <c>select</c> clauses over <see cref="IPipeline{T}"/>.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never), DebuggerStepperBoundary]
    public static IPipeline<TResult> Select<T, TResult>(this IPipeline<T> pipeline, Func<T, TResult> selector)
    {
        return Pipeline.Create(() =>
        {
            var result = pipeline.Execute();

            return result.IsSuccess ? Result.Ok(selector(result.Value)) : Result.ForwardFail<TResult>(result);
        });
    }

    /// <summary>
    /// Filters the elements of a pipeline based on a predicate. Intended for use with LINQ <c>where</c> clauses over <see cref="IPipeline{T}"/>.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never), DebuggerStepperBoundary]
    public static IPipeline<T> Where<T>(this IPipeline<T> pipeline, Func<T, bool> predicate)
    {
        return Pipeline.Create(() =>
        {
            var result = pipeline.Execute();

            if (!result.IsSuccess)
            {
                return result;
            }

            return predicate(result.Value) ? result : Result.Fail<T>("Predicate not satisfied.");
        });
    }

    /// <summary>
    /// Projects each element of a pipeline to another pipeline and flattens the resulting pipelines into one. Intended for use with LINQ <c>from</c> and <c>select</c> clauses over <see cref="IPipeline{T}"/>.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never), DebuggerStepperBoundary]
    public static IPipeline<TResult> SelectMany<T, TIntermediate, TResult>(
        this IPipeline<T> pipeline,
        Func<T, IPipeline<TIntermediate>> selector,
        Func<T, TIntermediate, TResult> resultSelector)
    {
        return Pipeline.Create(() =>
        {
            var result = pipeline.Execute();

            if (!result.IsSuccess)
            {
                return Result.ForwardFail<TResult>(result);
            }

            var intermediate = selector(result.Value).Execute();

            if (!intermediate.IsSuccess)
            {
                return Result.ForwardFail<TResult>(intermediate);
            }

            return Result.Ok(resultSelector(result.Value, intermediate.Value));
        });
    }

    /// <summary>
    /// Projects each element of an asynchronous pipeline into a new form. Intended for use with LINQ <c>select</c> clauses over <see cref="IAsyncPipeline{T}"/>.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never), DebuggerStepperBoundary]
    public static IAsyncPipeline<TResult> Select<T, TResult>(this IAsyncPipeline<T> pipeline, Func<T, TResult> selector)
    {
        return Pipeline.Create<TResult>(async () =>
        {
            var result = await pipeline.ExecuteAsync().ConfigureAwait(false);

            return result.IsSuccess ? Result.Ok(selector(result.Value)) : Result.ForwardFail<TResult>(result);
        });
    }

    /// <summary>
    /// Filters the elements of an asynchronous pipeline based on a predicate. Intended for use with LINQ <c>where</c> clauses over <see cref="IAsyncPipeline{T}"/>.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never), DebuggerStepperBoundary]
    public static IAsyncPipeline<T> Where<T>(this IAsyncPipeline<T> pipeline, Func<T, bool> predicate)
    {
        return Pipeline.Create<T>(async () =>
        {
            var result = await pipeline.ExecuteAsync().ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                return result;
            }

            return predicate(result.Value) ? result : Result.Fail<T>("Predicate not satisfied.");
        });
    }

    /// <summary>
    /// Projects each element of an asynchronous pipeline to another asynchronous pipeline and flattens the resulting pipelines into one. Intended for use with LINQ <c>from</c> and <c>select</c> clauses over <see cref="IAsyncPipeline{T}"/>.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never), DebuggerStepperBoundary]
    public static IAsyncPipeline<TResult> SelectMany<T, TIntermediate, TResult>(
        this IAsyncPipeline<T> pipeline,
        Func<T, IAsyncPipeline<TIntermediate>> selector,
        Func<T, TIntermediate, TResult> resultSelector)
    {
        return Pipeline.Create<TResult>(async () =>
        {
            var result = await pipeline.ExecuteAsync().ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                return Result.ForwardFail<TResult>(result);
            }

            var intermediate = await selector(result.Value).ExecuteAsync().ConfigureAwait(false);

            if (!intermediate.IsSuccess)
            {
                return Result.ForwardFail<TResult>(intermediate);
            }

            return Result.Ok(resultSelector(result.Value, intermediate.Value));
        });
    }

    /// <summary>
    /// Supports mixing synchronous and asynchronous pipelines in LINQ query expressions. Projects each element of a pipeline to an asynchronous pipeline and flattens the result.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never), DebuggerStepperBoundary]
    public static IAsyncPipeline<TResult> SelectMany<T, TIntermediate, TResult>(
        this IPipeline<T> pipeline,
        Func<T, IAsyncPipeline<TIntermediate>> selector,
        Func<T, TIntermediate, TResult> resultSelector)
    {
        return Pipeline.Create<TResult>(async () =>
        {
            var result = pipeline.Execute();

            if (!result.IsSuccess)
            {
                return Result.ForwardFail<TResult>(result);
            }

            var intermediate = await selector(result.Value).ExecuteAsync().ConfigureAwait(false);

            if (!intermediate.IsSuccess)
            {
                return Result.ForwardFail<TResult>(intermediate);
            }

            return Result.Ok(resultSelector(result.Value, intermediate.Value));
        });
    }

    /// <summary>
    /// Supports mixing asynchronous and synchronous pipelines in LINQ query expressions. Projects each element of an asynchronous pipeline to a synchronous pipeline and flattens the result.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never), DebuggerStepperBoundary]
    public static IAsyncPipeline<TResult> SelectMany<T, TIntermediate, TResult>(
        this IAsyncPipeline<T> pipeline,
        Func<T, IPipeline<TIntermediate>> selector,
        Func<T, TIntermediate, TResult> resultSelector)
    {
        return Pipeline.Create<TResult>(async () =>
        {
            var result = await pipeline.ExecuteAsync().ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                return Result.ForwardFail<TResult>(result);
            }

            var intermediate = selector(result.Value).Execute();

            if (!intermediate.IsSuccess)
            {
                return Result.ForwardFail<TResult>(intermediate);
            }

            return Result.Ok(resultSelector(result.Value, intermediate.Value));
        });
    }

    /// <summary>
    /// Supports advanced LINQ query expressions by allowing asynchronous selection of asynchronous pipelines.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never), DebuggerStepperBoundary]
    public static IAsyncPipeline<TResult> SelectMany<T, TIntermediate, TResult>(
        this IAsyncPipeline<T> pipeline,
        Func<T, Task<IAsyncPipeline<TIntermediate>>> selector,
        Func<T, TIntermediate, TResult> resultSelector)
    {
        return Pipeline.Create<TResult>(async () =>
        {
            var result = await pipeline.ExecuteAsync().ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                return Result.ForwardFail<TResult>(result);
            }

            var intermediatePipeline = await selector(result.Value).ConfigureAwait(false);
            var intermediate = await intermediatePipeline.ExecuteAsync().ConfigureAwait(false);

            if (!intermediate.IsSuccess)
            {
                return Result.ForwardFail<TResult>(intermediate);
            }

            return Result.Ok(resultSelector(result.Value, intermediate.Value));
        });
    }

    /// <summary>
    /// Supports advanced LINQ query expressions by allowing asynchronous selection of synchronous pipelines.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never), DebuggerStepperBoundary]
    public static IAsyncPipeline<TResult> SelectMany<T, TIntermediate, TResult>(
        this IAsyncPipeline<T> pipeline,
        Func<T, Task<IPipeline<TIntermediate>>> selector,
        Func<T, TIntermediate, TResult> resultSelector)
    {
        return Pipeline.Create<TResult>(async () =>
        {
            var result = await pipeline.ExecuteAsync().ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                return Result.ForwardFail<TResult>(result);
            }

            var intermediatePipeline = await selector(result.Value).ConfigureAwait(false);
            var intermediate = intermediatePipeline.Execute();

            if (!intermediate.IsSuccess)
            {
                return Result.ForwardFail<TResult>(intermediate);
            }

            return Result.Ok(resultSelector(result.Value, intermediate.Value));
        });
    }

    /// <summary>
    /// Supports advanced LINQ query expressions by allowing synchronous selection of asynchronous pipelines.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never), DebuggerStepperBoundary]
    public static IAsyncPipeline<TResult> SelectMany<T, TIntermediate, TResult>(
        this IPipeline<T> pipeline,
        Func<T, Task<IAsyncPipeline<TIntermediate>>> selector,
        Func<T, TIntermediate, TResult> resultSelector)
    {
        return Pipeline.Create<TResult>(async () =>
        {
            var result = pipeline.Execute();

            if (!result.IsSuccess)
            {
                return Result.ForwardFail<TResult>(result);
            }

            var intermediatePipeline = await selector(result.Value).ConfigureAwait(false);
            var intermediate = await intermediatePipeline.ExecuteAsync().ConfigureAwait(false);

            if (!intermediate.IsSuccess)
            {
                return Result.ForwardFail<TResult>(intermediate);
            }

            return Result.Ok(resultSelector(result.Value, intermediate.Value));
        });
    }

    /// <summary>
    /// Supports advanced LINQ query expressions by allowing synchronous selection of synchronous pipelines asynchronously.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never), DebuggerStepperBoundary]
    public static IAsyncPipeline<TResult> SelectMany<T, TIntermediate, TResult>(
        this IPipeline<T> pipeline,
        Func<T, Task<IPipeline<TIntermediate>>> selector,
        Func<T, TIntermediate, TResult> resultSelector)
    {
        return Pipeline.Create<TResult>(async () =>
        {
            var result = pipeline.Execute();

            if (!result.IsSuccess)
            {
                return Result.ForwardFail<TResult>(result);
            }

            var intermediatePipeline = await selector(result.Value).ConfigureAwait(false);
            var intermediate = intermediatePipeline.Execute();

            if (!intermediate.IsSuccess)
            {
                return Result.ForwardFail<TResult>(intermediate);
            }

            return Result.Ok(resultSelector(result.Value, intermediate.Value));
        });
    }
}
