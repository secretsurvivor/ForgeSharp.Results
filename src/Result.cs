using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace ForgeSharp.Results;

/// <summary>
/// Non-generic result representing success or failure.
/// </summary>
[StructLayout(LayoutKind.Sequential), Serializable, ReadOnly(true)]
public readonly struct Result : ISerializable, IEquatable<Result>
{
    internal readonly string? _message;
    internal readonly Exception? _exception;

    internal Result(string? message, Exception? exception)
    {
        _message = message;
        _exception = exception;
    }

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
    internal Result(string message) : this(message, null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> struct with an exception.
    /// </summary>
    /// <param name="exception">The exception.</param>
    [DebuggerStepperBoundary]
    internal Result(Exception exception) : this(null, exception) { }

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
    /// Converts this result into a <see cref="Result{T}"/>, carrying over the failure state.
    /// </summary>
    /// <typeparam name="T">The target value type.</typeparam>
    /// <returns>A new result with the same success/failure state.</returns>
    public Result<T> As<T>() => new Result<T>(_message, _exception);

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

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Result other)
    {
        if (IsSuccess)
        {
            return other.IsSuccess;
        }

        if (IsValidationFault)
        {
            return other.IsValidationFault && _message == other._message;
        }

        if (IsException)
        {
            return other.IsException && ReferenceEquals(_exception, other._exception);
        }

        return false;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Result other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        if (IsSuccess)
        {
            return 0;
        }

        if (IsValidationFault)
        {
            return _message!.GetHashCode();
        }

        return _exception!.GetHashCode();
    }

    /// <summary>
    /// Determines whether two <see cref="Result"/> instances are equal.
    /// </summary>
    public static bool operator ==(Result left, Result right) => left.Equals(right);

    /// <summary>
    /// Determines whether two <see cref="Result"/> instances are not equal.
    /// </summary>
    public static bool operator !=(Result left, Result right) => !left.Equals(right);

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
public readonly struct Result<T> : ISerializable, IEquatable<Result<T>>
{
    internal readonly T? _value;
    internal readonly string? _message;
    internal readonly Exception? _exception;

    internal Result(T? value)
    {
        _value = value;
    }

    internal Result(string? message, Exception? exception)
    {
        _message = message;
        _exception = exception;
    }

    /// <inheritdoc/>
    public bool IsSuccess => _value is not null && _message is null && _exception is null;

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
    /// Initializes a new instance of the <see cref="Result{T}"/> struct with a validation fault message.
    /// </summary>
    /// <param name="message">The validation fault message.</param>
    [DebuggerStepperBoundary]
    internal Result(string message) : this(message, null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{T}"/> struct with an exception.
    /// </summary>
    /// <param name="exception">The exception.</param>
    [DebuggerStepperBoundary]
    internal Result(Exception exception) : this(null, exception) { }

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
    /// Implicitly converts a value to a success <see cref="Result{T}"/>.
    /// </summary>
    /// <param name="value">The value.</param>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Result<T>(T value) => new Result<T>(value);

    /// <summary>
    /// Explicitly converts a <see cref="Result{T}"/> to a non-generic <see cref="Result"/>, dropping the value.
    /// </summary>
    /// <param name="value">The typed result to convert.</param>
    public static explicit operator Result(Result<T> value) => new Result(value._message, value._exception);

    /// <summary>
    /// Converts this result into a <see cref="Result{T}"/> of a different type, carrying over the failure state.
    /// </summary>
    /// <typeparam name="U">The target value type.</typeparam>
    /// <returns>A new result with the same success/failure state.</returns>
    public Result<U> As<U>() => new Result<U>(_message, _exception);

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

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Result<T> other)
    {
        if (IsSuccess)
        {
            return other.IsSuccess && EqualityComparer<T>.Default.Equals(_value!, other._value!);
        }

        if (IsValidationFault)
        {
            return other.IsValidationFault && _message == other._message;
        }

        if (IsException)
        {
            return other.IsException && ReferenceEquals(_exception, other._exception);
        }

        return false;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Result<T> other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        if (IsSuccess)
        {
            return _value is not null ? EqualityComparer<T>.Default.GetHashCode(_value) : 0;
        }

        if (IsValidationFault)
        {
            return _message!.GetHashCode();
        }

        return _exception!.GetHashCode();
    }

    /// <summary>
    /// Determines whether two <see cref="Result{T}"/> instances are equal.
    /// </summary>
    public static bool operator ==(Result<T> left, Result<T> right) => left.Equals(right);

    /// <summary>
    /// Determines whether two <see cref="Result{T}"/> instances are not equal.
    /// </summary>
    public static bool operator !=(Result<T> left, Result<T> right) => !left.Equals(right);
}

/// <summary>
/// Discriminated result carrying either a <typeparamref name="TValue"/> on success or a <typeparamref name="TError"/> on failure.
/// </summary>
/// <typeparam name="TValue">The value type.</typeparam>
/// <typeparam name="TError">The error type.</typeparam>
[StructLayout(LayoutKind.Sequential), Serializable]
public readonly struct Result<TValue, TError> : IEquatable<Result<TValue, TError>>
{
    internal readonly TValue? _value;
    internal readonly TError? _error;

    /// <summary>
    /// True if the operation succeeded.
    /// </summary>
    public bool IsSuccess => _value is not null && _error is null;

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
        _value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TValue, TError}"/> struct with an error.
    /// </summary>
    /// <param name="error">The error.</param>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Result(TError error)
    {
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

    /// <summary>
    /// Implicitly converts a value to a success <see cref="Result{TValue, TError}"/>.
    /// </summary>
    /// <param name="value">The value.</param>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Result<TValue, TError>(TValue value) => new Result<TValue, TError>(value);

    /// <summary>
    /// Implicitly converts an error to a failure <see cref="Result{TValue, TError}"/>.
    /// </summary>
    /// <param name="error">The error.</param>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Result<TValue, TError>(TError error) => new Result<TValue, TError>(error);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Result<TValue, TError> other)
    {
        if (IsSuccess != other.IsSuccess)
        {
            return false;
        }

        if (IsSuccess)
        {
            return EqualityComparer<TValue>.Default.Equals(_value!, other._value!);
        }

        return EqualityComparer<TError>.Default.Equals(_error!, other._error!);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Result<TValue, TError> other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        if (IsSuccess)
        {
            return _value is not null ? EqualityComparer<TValue>.Default.GetHashCode(_value) : 0;
        }

        return _error is not null ? EqualityComparer<TError>.Default.GetHashCode(_error) : 0;
    }

    /// <summary>
    /// Determines whether two <see cref="Result{TValue, TError}"/> instances are equal.
    /// </summary>
    public static bool operator ==(Result<TValue, TError> left, Result<TValue, TError> right) => left.Equals(right);

    /// <summary>
    /// Determines whether two <see cref="Result{TValue, TError}"/> instances are not equal.
    /// </summary>
    public static bool operator !=(Result<TValue, TError> left, Result<TValue, TError> right) => !left.Equals(right);
}