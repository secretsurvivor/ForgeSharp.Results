using System.Runtime.CompilerServices;

namespace ForgeSharp.Results.Infrastructure;

internal static class AsyncHelper
{
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<T> CreateValueTask<T>(T value)
    {
        return new ValueTask<T>(value);
    }
#endif
}
