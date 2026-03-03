using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace ForgeSharp.Results;

/// <summary>
/// Common interface for all result types.
/// </summary>
public interface IResult
{
    /// <summary>
    /// True if the operation succeeded.
    /// </summary>
    bool IsSuccess { get; }

    /// <summary>
    /// True if this is a validation fault.
    /// </summary>
    bool IsValidationFault { get; }

    /// <summary>
    /// True if this represents an exception.
    /// </summary>
    bool IsException { get; }

    /// <summary>
    /// The validation message.
    /// </summary>
    string Message { get; }

    /// <summary>
    /// The captured exception.
    /// </summary>
    Exception Exception { get; }
}

/// <summary>
/// Extends <see cref="IResult"/> with a typed value.
/// </summary>
/// <typeparam name="T">The value type.</typeparam>
public interface IResult<out T> : IResult
{
    /// <summary>
    /// The result value.
    /// </summary>
    T Value { get; }
}

/// <summary>
/// Non-generic result representing success or failure.
/// </summary>
[StructLayout(LayoutKind.Sequential), Serializable, ReadOnly(true)]
public readonly struct Result : IResult, ISerializable
{
    private readonly string? _message;
    private readonly Exception? _exception;

    /// <inheritdoc/>
    public bool IsSuccess => _message is null && _exception is null;

    /// <inheritdoc/>
    public bool IsValidationFault => _message is not null;

    /// <inheritdoc/>
    public bool IsException => _exception is not null;

    /// <inheritdoc/>
    public string Message => _message!;

    /// <inheritdoc/>
    public Exception Exception => _exception!;

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> struct with a validation fault message.
    /// </summary>
    /// <param name="message">The validation fault message.</param>
    [DebuggerStepperBoundary]
    internal Result(string message)
    {
        _message = message;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> struct with an exception.
    /// </summary>
    /// <param name="exception">The exception.</param>
    [DebuggerStepperBoundary]
    internal Result(Exception exception)
    {
        _exception = exception;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> struct from another <see cref="IResult"/>.
    /// </summary>
    /// <param name="result">The result to copy.</param>
    [DebuggerStepperBoundary]
    internal Result(IResult result)
    {
        _message = result.Message;
        _exception = result.Exception;
    }

    /// <summary>
    /// Creates a success result.
    /// </summary>
    /// <returns>A success result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result Ok() => new Result();

    /// <summary>
    /// Creates a failed result from an exception.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <returns>A failure result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result Fail(Exception exception) => new Result(exception);

    /// <summary>
    /// Creates a failed result with a validation message.
    /// </summary>
    /// <param name="message">The failure message.</param>
    /// <returns>A failure result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result Fail(string message) => new Result(message);

    /// <summary>
    /// Creates a success result with a value.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>A success result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> Ok<T>(T value) => Result<T>.Ok(value);

    /// <summary>
    /// Creates a typed failure with a validation message.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="message">The failure message.</param>
    /// <returns>A failure result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> Fail<T>(string message) => Result<T>.Fail(message);

    /// <summary>
    /// Creates a typed failure from an exception.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="exception">The exception.</param>
    /// <returns>A failure result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> Fail<T>(Exception exception) => Result<T>.Fail(exception);

    /// <summary>
    /// Creates a success result with a value.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>A success result.</returns>
    public static Result<TValue, TError> Ok<TValue, TError>(TValue value) => Result<TValue, TError>.Ok(value);

    /// <summary>
    /// Creates a failed result with an error.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="error">The error.</param>
    /// <returns>A failure result.</returns>
    public static Result<TValue, TError> Fail<TValue, TError>(TError error) => Result<TValue, TError>.Fail(error);

    /// <summary>
    /// Propagates a failure into a new <see cref="Result"/>.
    /// </summary>
    /// <param name="result">The failed result to propagate.</param>
    /// <returns>A new result carrying the same failure.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result ForwardFail(IResult result) => new Result(result);

    /// <summary>
    /// Propagates a failure into a new <see cref="Result{T}"/>.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="result">The failed result to propagate.</param>
    /// <returns>A new result carrying the same failure.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> ForwardFail<T>(IResult result) => Result<T>.ForwardFail(result);

    /// <summary>
    /// Runs <paramref name="action"/> and returns any thrown exception as a failure.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>Success if no exception was thrown; otherwise a failure containing the exception.</returns>
    [DebuggerStepperBoundary]
    public static Result Capture(Action action)
    {
        try
        {
            action();
            return Ok();
        }
        catch (Exception e)
        {
            return Fail(e);
        }
    }

    /// <summary>
    /// Runs <paramref name="func"/> and returns any thrown exception as a failure.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="func">The function to execute.</param>
    /// <returns>A success result with the return value, or a failure containing the exception.</returns>
    [DebuggerStepperBoundary]
    public static Result<T> Capture<T>(Func<T> func)
    {
        try
        {
            return Ok(func());
        }
        catch (Exception e)
        {
            return Fail<T>(e);
        }
    }

    /// <summary>
    /// Runs <paramref name="func"/> and maps any thrown exception to <typeparamref name="TError"/> via <paramref name="mapException"/>.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="func">The function to execute.</param>
    /// <param name="mapException">Maps a caught exception to the error type.</param>
    /// <returns>A success result with the return value, or a failure with the mapped error.</returns>
    public static Result<T, TError> Capture<T, TError>(Func<T> func, Func<Exception, TError> mapException)
    {
        try
        {
            return Ok<T, TError>(func());
        }
        catch (Exception e)
        {
            return Fail<T, TError>(mapException(e));
        }
    }

    /// <summary>
    /// Async version of <see cref="Capture(Action)"/>.
    /// </summary>
    /// <param name="action">The async action to execute.</param>
    /// <returns>Success if no exception was thrown; otherwise a failure containing the exception.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result> CaptureAsync(Func<Task> action)
    {
        try
        {
            await action().ConfigureAwait(false);
            return Ok();
        }
        catch (Exception e)
        {
            return Fail(e);
        }
    }

    /// <summary>
    /// Async version of <see cref="Capture{T}(Func{T})"/>.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="func">The async function to execute.</param>
    /// <returns>A success result with the return value, or a failure containing the exception.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<T>> CaptureAsync<T>(Func<Task<T>> func)
    {
        try
        {
            return Ok(await func().ConfigureAwait(false));
        }
        catch (Exception e)
        {
            return Fail<T>(e);
        }
    }

    /// <summary>
    /// Async version of <see cref="Capture{T, TError}(Func{T}, Func{Exception, TError})"/>.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="func">The async function to execute.</param>
    /// <param name="mapException">Maps a caught exception to the error type.</param>
    /// <returns>A success result with the return value, or a failure with the mapped error.</returns>
    public static async Task<Result<T, TError>> CaptureAsync<T, TError>(Func<Task<T>> func, Func<Exception, TError> mapException)
    {
        try
        {
            return Ok<T, TError>(await func().ConfigureAwait(false));
        }
        catch (Exception e)
        {
            return Fail<T, TError>(mapException(e));
        }
    }

    /// <summary>
    /// Returns a string representation of the result.
    /// </summary>
    /// <returns>A string describing the result.</returns>
    public override string ToString()
    {
        if (IsSuccess)
        {
            return "Success";
        }
        else if (IsValidationFault)
        {
            return $"Validation Failed: {Message}";
        }
        else if (IsException)
        {
            return $"Exception: {Exception.GetType().Name} - {Exception.Message}";
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Populates a <see cref="SerializationInfo"/> with the data needed to serialize the <see cref="Result"/>.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> to populate with data.</param>
    /// <param name="context">The destination (see <see cref="StreamingContext"/>) for this serialization.</param>
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue(nameof(IsSuccess), IsSuccess);
        info.AddValue(nameof(Message), Message);
        info.AddValue(nameof(Exception), Exception, typeof(Exception));
    }

    /// <summary>
    /// Represents the state of a <see cref="Result"/>.
    /// </summary>
    public enum State
    {
        /// <summary>Success.</summary>
        Success,

        /// <summary>Failed due to an exception.</summary>
        Exception,

        /// <summary>Failed due to a validation fault.</summary>
        ValidationFault,
    }
}

/// <summary>
/// Generic result carrying a value of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The value type.</typeparam>
[StructLayout(LayoutKind.Sequential), Serializable]
public readonly struct Result<T> : IResult<T>, ISerializable
{
    private readonly T? _value;
    private readonly string? _message;
    private readonly Exception? _exception;

    /// <inheritdoc/>
    public bool IsSuccess => _message is null && _exception is null;

    /// <inheritdoc/>
    public bool IsValidationFault => _message is not null;

    /// <inheritdoc/>
    public bool IsException => _exception is not null;

    /// <inheritdoc/>
    public string Message => _message!;

    /// <inheritdoc/>
    public Exception Exception => _exception!;

    /// <inheritdoc/>
    public T Value => _value!;

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{T}"/> struct with a value.
    /// </summary>
    /// <param name="value">The value.</param>
    [DebuggerStepperBoundary]
    internal Result(T value)
    {
        _value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{T}"/> struct with a validation fault message.
    /// </summary>
    /// <param name="message">The validation fault message.</param>
    [DebuggerStepperBoundary]
    internal Result(string message)
    {
        _message = message;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{T}"/> struct with an exception.
    /// </summary>
    /// <param name="exception">The exception.</param>
    [DebuggerStepperBoundary]
    internal Result(Exception exception)
    {
        _exception = exception;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{T}"/> struct from another <see cref="IResult"/>.
    /// </summary>
    /// <param name="result">The result to copy.</param>
    [DebuggerStepperBoundary]
    internal Result(IResult result)
    {
        _message = result.Message;
        _exception = result.Exception;
    }

    /// <summary>
    /// Creates a success result with a value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A success result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> Ok(T value) => new Result<T>(value);

    /// <summary>
    /// Creates a failure with a validation message.
    /// </summary>
    /// <param name="message">The failure message.</param>
    /// <returns>A failure result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> Fail(string message) => new Result<T>(message);

    /// <summary>
    /// Creates a failure from an exception.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <returns>A failure result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> Fail(Exception exception) => new Result<T>(exception);

    /// <summary>
    /// Propagates a failure into a new <see cref="Result{T}"/>.
    /// </summary>
    /// <param name="result">The failed result to propagate.</param>
    /// <returns>A new result carrying the same failure.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> ForwardFail(IResult result) => new Result<T>(result);

    /// <summary>
    /// Converts to a non-generic <see cref="Result"/>, preserving the failure state.
    /// </summary>
    /// <param name="result">The result to convert.</param>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator Result(Result<T> result) => Result.ForwardFail(result);

    /// <summary>
    /// Returns a string representation of the result.
    /// </summary>
    /// <returns>A string describing the result.</returns>
    public override string ToString()
    {
        if (IsSuccess)
        {
            return $"Success: {Value!.GetType().Name}";
        }
        else if (IsValidationFault)
        {
            return $"Validation Failed: {Message}";
        }
        else if (IsException)
        {
            return $"Exception: {Exception.GetType().Name} - {Exception.Message}";
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Populates a <see cref="SerializationInfo"/> with the data needed to serialize the <see cref="Result{T}"/>.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> to populate with data.</param>
    /// <param name="context">The destination (see <see cref="StreamingContext"/>) for this serialization.</param>
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue(nameof(IsSuccess), IsSuccess);
        info.AddValue(nameof(Value), Value, typeof(T));
        info.AddValue(nameof(Message), Message);
        info.AddValue(nameof(Exception), Exception, typeof(Exception));
    }
}

/// <summary>
/// Discriminated result carrying either a <typeparamref name="TValue"/> on success or a <typeparamref name="TError"/> on failure.
/// </summary>
/// <typeparam name="TValue">The value type.</typeparam>
/// <typeparam name="TError">The error type.</typeparam>
[StructLayout(LayoutKind.Sequential), Serializable]
public readonly struct Result<TValue, TError>
{
    private readonly TValue? _value;
    private readonly TError? _error;

    /// <summary>
    /// True if the operation succeeded.
    /// </summary>
    public bool IsSuccess { [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)] get; }

    /// <summary>
    /// The success value. Only valid when <see cref="IsSuccess"/> is true.
    /// </summary>
    public TValue Value => _value!;

    /// <summary>
    /// The error. Only valid when <see cref="IsSuccess"/> is false.
    /// </summary>
    public TError Error => _error!;

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TValue, TError}"/> struct with a successful value.
    /// </summary>
    /// <param name="value">The successful value.</param>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Result(TValue value)
    {
        IsSuccess = true;
        _value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TValue, TError}"/> struct with an error.
    /// </summary>
    /// <param name="error">The error.</param>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Result(TError error)
    {
        IsSuccess = false;
        _error = error;
    }

    /// <summary>
    /// Creates a success result with a value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A success result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<TValue, TError> Ok(TValue value) => new Result<TValue, TError>(value);

    /// <summary>
    /// Creates a failed result with an error.
    /// </summary>
    /// <param name="error">The error.</param>
    /// <returns>A failure result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<TValue, TError> Fail(TError error) => new Result<TValue, TError>(error);
}