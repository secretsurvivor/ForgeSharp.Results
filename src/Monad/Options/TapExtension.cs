using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ForgeSharp.Results.Monad.Options;

/// <summary>
/// Provides "tap" operators for <see cref="Options{T}"/> which allow running side-effecting actions
/// when an option has a value while returning the original option unchanged.
/// </summary>
public static class OptionsTapExtension
{
    #region Sync
    /// <summary>
    /// Executes an action if the option has a value.
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
    /// Executes an action if the awaited option has a value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="optionTask">The option task.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The original option as a task.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<Options<T>> TapAsync<T>(this Task<Options<T>> optionTask, Action<T> action)
    {
        var option = await optionTask.ConfigureAwait(false);
        return option.Tap(action);
    }

    /// <summary>
    /// Executes an asynchronous action if the awaited option has a value.
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
