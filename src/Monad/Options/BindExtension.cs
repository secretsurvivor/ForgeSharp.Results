using ForgeSharp.Results.Infrastructure;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ForgeSharp.Results.Monad.Options;

/// <summary>
/// Bind (flatMap) operators for <see cref="Options{T}"/>.
/// </summary>
public static class OptionsBindExtension
{
    #region Sync
    /// <summary>
    /// Binds the option to a function that returns another option.
    /// </summary>
    /// <typeparam name="T">The input type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
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
    /// Awaits the option, then binds.
    /// </summary>
    /// <typeparam name="T">The input type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="optionTask">The option task containing a value.</param>
    /// <param name="func">The function that returns a new option based on the value.</param>
    /// <returns>The result of applying the function as a task, or <see cref="Options{T}.None()"/> if the input option has no value.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Options<TResult>> BindAsync<T, TResult>(this Task<Options<T>> optionTask, Func<T, Options<TResult>> func)
    {
        if (optionTask.TryGetResult(out var option))
        {
            return Task.FromResult(Bind(option, func));
        }

        return Impl(optionTask, func);

        static async Task<Options<TResult>> Impl(Task<Options<T>> optionTask, Func<T, Options<TResult>> func)
        {
            var option = await optionTask.ConfigureAwait(false);

            if (!option.HasValue)
            {
                return Options<TResult>.None();
            }

            return func(option.Value);
        }
    }

    /// <summary>
    /// Awaits the option, then binds with an async function.
    /// </summary>
    /// <typeparam name="T">The input type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
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
