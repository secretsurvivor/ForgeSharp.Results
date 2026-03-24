using ForgeSharp.Results.Infrastructure;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ForgeSharp.Results.Monad.Options;

/// <summary>
/// Tap operators for running side-effects on <see cref="Options{T}"/>.
/// </summary>
public static class OptionsTapExtension
{
    #region Sync
    /// <summary>
    /// Runs a side-effect if the option has a value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="option">The option.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The original option.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Options<T> Tap<T>(this Options<T> option, Action<T> action)
    {
        if (option.HasValue)
        {
            action(option.Value);
        }

        return option;
    }
    #endregion

    #region Async Option
    /// <summary>
    /// Awaits the option, then taps.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="optionTask">The option task.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The original option as a task.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Options<T>> TapAsync<T>(this Task<Options<T>> optionTask, Action<T> action)
    {
        if (optionTask.TryGetResult(out var option))
        {
            return Task.FromResult(Tap(option, action));
        }

        return Impl(optionTask, action);

        static async Task<Options<T>> Impl(Task<Options<T>> optionTask, Action<T> action)
        {
            var option = await optionTask.ConfigureAwait(false);

            if (option.HasValue)
            {
                action(option.Value);
            }

            return option;
        }
    }

    /// <summary>
    /// Awaits the option, then runs an async side-effect.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="optionTask">The option task.</param>
    /// <param name="actionAsync">The asynchronous action to execute.</param>
    /// <returns>The original option as a task.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<Options<T>> TapAsync<T>(this Task<Options<T>> optionTask, Func<T, Task> actionAsync)
    {
        var option = await optionTask.ConfigureAwait(false);

        if (option.HasValue)
        {
            await actionAsync(option.Value).ConfigureAwait(false);
        }

        return option;
    }
    #endregion
}
