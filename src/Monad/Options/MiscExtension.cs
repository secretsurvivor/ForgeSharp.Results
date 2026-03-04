using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ForgeSharp.Results.Monad.Options;

/// <summary>
/// Utility methods for <see cref="Options{T}"/>.
/// </summary>
public static class OptionsMiscExtension
{
    /// <summary>
    /// Filters the option based on a predicate.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="option">The option to filter.</param>
    /// <param name="predicate">The predicate to evaluate.</param>
    /// <returns>The original option if it has a value and the predicate is true, otherwise <see cref="Options{T}.None()"/>.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Options<T> Filter<T>(this Options<T> option, Func<T, bool> predicate)
    {
        if (option.HasValue && predicate(option.Value))
        {
            return option;
        }

        return Options<T>.None();
    }

    /// <summary>
    /// Returns the value, or a default if empty.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="option">The option.</param>
    /// <param name="defaultValue">The default value to return if the option has no value.</param>
    /// <returns>The value of the option, or the default value if the option has no value.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetValueOrDefault<T>(this Options<T> option, T defaultValue)
    {
        return option.HasValue ? option.Value : defaultValue;
    }

    /// <summary>
    /// Returns the value, or computes a default if empty.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="option">The option.</param>
    /// <param name="defaultValue">The function to compute the default value.</param>
    /// <returns>The value of the option, or the computed default value if the option has no value.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetValueOrElse<T>(this Options<T> option, Func<T> defaultValue)
    {
        return option.HasValue ? option.Value : defaultValue();
    }

    /// <summary>
    /// True if the option has a matching value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="option">The option.</param>
    /// <param name="predicate">The predicate to evaluate.</param>
    /// <returns>True if the option has a value and the predicate is true; otherwise, false.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Exists<T>(this Options<T> option, Func<T, bool> predicate)
    {
        return option.HasValue && predicate(option.Value);
    }

    /// <summary>
    /// True if empty or the value matches the predicate.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="option">The option.</param>
    /// <param name="predicate">The predicate to evaluate.</param>
    /// <returns>True if the option has no value or the predicate is true for the value; otherwise, false.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ForAll<T>(this Options<T> option, Func<T, bool> predicate)
    {
        return !option.HasValue || predicate(option.Value);
    }

    /// <summary>
    /// Converts the option to a nullable value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="option">The option.</param>
    /// <returns>The value wrapped in a nullable, or null if the option has no value.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? ToNullable<T>(this Options<T> option) where T : struct
    {
        return option.HasValue ? option.Value : null;
    }

    /// <summary>
    /// Yields the value if present, otherwise empty.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="option">The option.</param>
    /// <returns>An enumerable containing the value if present, or an empty enumerable otherwise.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> ToEnumerable<T>(this Options<T> option)
    {
        if (option.HasValue)
        {
            yield return option.Value;
        }
    }

    /// <summary>
    /// Folds the option into a single value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="option">The option.</param>
    /// <param name="defaultValue">The value to return if the option has no value.</param>
    /// <param name="folder">The function to apply if the option has a value.</param>
    /// <returns>The result of applying the folder function if the option has a value, or the default value otherwise.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult Fold<T, TResult>(this Options<T> option, TResult defaultValue, Func<T, TResult> folder)
    {
        return option.HasValue ? folder(option.Value) : defaultValue;
    }

    /// <summary>
    /// Converts the option to a <see cref="Result{T}"/>, using the specified message on failure.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="option">The option.</param>
    /// <param name="failureMessage">The validation failure message to use when the option has no value.</param>
    /// <returns>A success result with the value if the option has a value, otherwise a failure result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> ToResult<T>(this Options<T> option, string failureMessage)
    {
        if (option.HasValue)
        {
            return Result.Ok(option.Value);
        }

        return Result.Fail<T>(failureMessage);
    }

    /// <summary>
    /// Converts the option to a <see cref="Result{T}"/>, using a lazily evaluated message on failure.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="option">The option.</param>
    /// <param name="failureMessageFactory">The function to produce the validation failure message when the option has no value.</param>
    /// <returns>A success result with the value if the option has a value, otherwise a failure result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> ToResult<T>(this Options<T> option, Func<string> failureMessageFactory)
    {
        if (option.HasValue)
        {
            return Result.Ok(option.Value);
        }

        return Result.Fail<T>(failureMessageFactory());
    }

    /// <summary>
    /// Converts the option to a <see cref="Result{T, TError}"/>, using the specified error on failure.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="option">The option.</param>
    /// <param name="error">The error to use when the option has no value.</param>
    /// <returns>A success result with the value if the option has a value, otherwise a failure result with the error.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T, TError> ToResult<T, TError>(this Options<T> option, TError error)
    {
        if (option.HasValue)
        {
            return Result.Ok<T, TError>(option.Value);
        }

        return Result.Fail<T, TError>(error);
    }

    /// <summary>
    /// Converts the option to a <see cref="Result{T, TError}"/>, using a lazily evaluated error on failure.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="option">The option.</param>
    /// <param name="errorFactory">The function to produce the error when the option has no value.</param>
    /// <returns>A success result with the value if the option has a value, otherwise a failure result with the error.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T, TError> ToResult<T, TError>(this Options<T> option, Func<TError> errorFactory)
    {
        if (option.HasValue)
        {
            return Result.Ok<T, TError>(option.Value);
        }

        return Result.Fail<T, TError>(errorFactory());
    }
}
