using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ForgeSharp.Results.Monad.Options;

/// <summary>
/// Match (fold) operators for exhaustively handling presence and absence of an option value.
/// </summary>
public static class OptionsMatchExtension
{
    /// <summary>
    /// Matches an option, invoking the appropriate branch.
    /// </summary>
    /// <typeparam name="T">The input value type.</typeparam>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="option">The option.</param>
    /// <param name="onSome">The function to invoke when a value is present.</param>
    /// <param name="onNone">The function to invoke when no value is present.</param>
    /// <returns>The value produced by the matched branch.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult Match<T, TResult>(this Options<T> option, Func<T, TResult> onSome, Func<TResult> onNone)
    {
        if (option.HasValue)
        {
            return onSome(option.Value);
        }

        return onNone();
    }

    /// <summary>
    /// Awaits the option, then matches.
    /// </summary>
    /// <typeparam name="T">The input value type.</typeparam>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="optionTask">The option task.</param>
    /// <param name="onSome">The function to invoke when a value is present.</param>
    /// <param name="onNone">The function to invoke when no value is present.</param>
    /// <returns>The value produced by the matched branch.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<TResult> MatchAsync<T, TResult>(this Task<Options<T>> optionTask, Func<T, TResult> onSome, Func<TResult> onNone)
    {
        var option = await optionTask.ConfigureAwait(false);
        return option.Match(onSome, onNone);
    }

    /// <summary>
    /// Awaits the option, then matches with async branches.
    /// </summary>
    /// <typeparam name="T">The input value type.</typeparam>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="optionTask">The option task.</param>
    /// <param name="onSome">The asynchronous function to invoke when a value is present.</param>
    /// <param name="onNone">The asynchronous function to invoke when no value is present.</param>
    /// <returns>The value produced by the matched branch.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<TResult> MatchAsync<T, TResult>(this Task<Options<T>> optionTask, Func<T, Task<TResult>> onSome, Func<Task<TResult>> onNone)
    {
        var option = await optionTask.ConfigureAwait(false);

        if (option.HasValue)
        {
            return await onSome(option.Value).ConfigureAwait(false);
        }

        return await onNone().ConfigureAwait(false);
    }
}
