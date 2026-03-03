using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ForgeSharp.Results.Monad.Options;

/// <summary>
/// Map operators for <see cref="Options{T}"/>.
/// </summary>
public static class OptionsMapExtension
{
    /// <summary>
    /// Transforms the value if present.
    /// </summary>
    /// <typeparam name="T">The input type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="option">The option containing a value.</param>
    /// <param name="func">The mapping function to apply if the option has a value.</param>
    /// <returns>A new option with the mapped value, or <see cref="Options{T}.None()"/> if the input option has no value.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Options<TResult> Map<T, TResult>(this Options<T> option, Func<T, TResult> func)
    {
        if (option.HasValue)
        {
            return Options<TResult>.Some(func(option.Value));
        }

        return Options<TResult>.None();
    }

    /// <summary>
    /// Awaits the option, then maps.
    /// </summary>
    /// <typeparam name="T">The input type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="optionTask">The option task containing a value.</param>
    /// <param name="func">The transformation function to apply if the option has a value.</param>
    /// <returns>A new option with the transformed value as a task, or <see cref="Options{T}.None()"/> if the input option has no value.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<Options<TResult>> MapAsync<T, TResult>(this Task<Options<T>> optionTask, Func<T, TResult> func)
    {
        var option = await optionTask.ConfigureAwait(false);
        return option.Map(func);
    }

    /// <summary>
    /// Awaits the option, then maps with an async function.
    /// </summary>
    /// <typeparam name="T">The input type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="optionTask">The option task containing a value.</param>
    /// <param name="funcAsync">The asynchronous transformation function to apply if the option has a value.</param>
    /// <returns>A new option with the transformed value as a task, or <see cref="Options{T}.None()"/> if the input option has no value.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<Options<TResult>> MapAsync<T, TResult>(this Task<Options<T>> optionTask, Func<T, Task<TResult>> funcAsync)
    {
        var option = await optionTask.ConfigureAwait(false);

        if (option.HasValue)
        {
            var result = await funcAsync(option.Value).ConfigureAwait(false);
            return Options<TResult>.Some(result);
        }

        return Options<TResult>.None();
    }
}
