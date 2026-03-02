using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ForgeSharp.Results.Monad.Options;

/// <summary>
/// Provides binding (flatMap) operators for <see cref="Options{T}"/> which allow chaining options
/// while preserving the absence-of-value semantics throughout the chain.
/// </summary>
public static class OptionsBindExtension
{
    #region Sync
    /// <summary>
    /// Chains two options together, flattening the result.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="option">The option containing a value.</param>
    /// <param name="func">The function that returns a new option based on the value.</param>
    /// <returns>The result of applying the function if the input option has a value, or <see cref="Options{T}.None()"/> otherwise.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Options<TResult> Bind<T, TResult>(this Options<T> option, Func<T, Options<TResult>> func)
    {
        if (!option.HasValue)
        {
            return Options<TResult>.None();
        }

        return func(option.Value);
    }
    #endregion

    #region Async Option
    /// <summary>
    /// Chains an awaited option with a synchronous binding function.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="optionTask">The option task containing a value.</param>
    /// <param name="func">The function that returns a new option based on the value.</param>
    /// <returns>The result of applying the function as a task, or <see cref="Options{T}.None()"/> if the input option has no value.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<Options<TResult>> BindAsync<T, TResult>(this Task<Options<T>> optionTask, Func<T, Options<TResult>> func)
    {
        var option = await optionTask.ConfigureAwait(false);
        return option.Bind(func);
    }

    /// <summary>
    /// Chains an awaited option with an asynchronous binding function.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="optionTask">The option task containing a value.</param>
    /// <param name="funcAsync">The asynchronous function that returns a new option based on the value.</param>
    /// <returns>The result of applying the function as a task, or <see cref="Options{T}.None()"/> if the input option has no value.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<Options<TResult>> BindAsync<T, TResult>(this Task<Options<T>> optionTask, Func<T, Task<Options<TResult>>> funcAsync)
    {
        var option = await optionTask.ConfigureAwait(false);

        if (!option.HasValue)
        {
            return Options<TResult>.None();
        }

        return await funcAsync(option.Value).ConfigureAwait(false);
    }
    #endregion
}
