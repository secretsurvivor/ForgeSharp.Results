using System.Runtime.CompilerServices;

namespace ForgeSharp.Results.Infrastructure;

/// <summary>
/// Internal helpers for fast-path Task/ValueTask handling. When a task has already
/// completed successfully we can avoid the async state-machine overhead entirely.
/// </summary>
internal static class AsyncHelper
{
    /// <summary>
    /// Tries to synchronously extract the result of a completed <see cref="Task{T}"/>
    /// without allocating an async state machine.
    /// </summary>
    /// <typeparam name="T">The task result type.</typeparam>
    /// <param name="task">The task to check.</param>
    /// <param name="result">The result if the task completed successfully.</param>
    /// <returns><c>true</c> if the result was available synchronously; otherwise <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetResult<T>(this Task<T> task, out T result)
    {
#if NETSTANDARD2_0
        if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
        {
            result = task.Result;
            return true;
        }
#else
        if (task.IsCompletedSuccessfully)
        {
            result = task.Result;
            return true;
        }
#endif
        result = default!;
        return false;
    }

#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
    /// <summary>
    /// Tries to synchronously extract the result of a completed <see cref="ValueTask{T}"/>
    /// without allocating an async state machine.
    /// </summary>
    /// <typeparam name="T">The task result type.</typeparam>
    /// <param name="task">The value task to check.</param>
    /// <param name="result">The result if the task completed successfully.</param>
    /// <returns><c>true</c> if the result was available synchronously; otherwise <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetResult<T>(this ValueTask<T> task, out T result)
    {
        if (task.IsCompletedSuccessfully)
        {
            result = task.Result;
            return true;
        }

        result = default!;
        return false;
    }

    /// <summary>
    /// Creates a <see cref="ValueTask{T}"/> that wraps an already-known value,
    /// avoiding heap allocation of a Task.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="value">The value to wrap.</param>
    /// <returns>A completed <see cref="ValueTask{T}"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<T> CreateValueTask<T>(T value)
    {
        return new ValueTask<T>(value);
    }
#endif
}
