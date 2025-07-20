using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace ForgeSharp.Results;

/// <summary>
/// Represents the result of an operation.
/// </summary>
public interface IResult
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    bool IsSuccess { get; }

    /// <summary>
    /// Gets the status of the result.
    /// </summary>
    Result.State Status { get; }

    /// <summary>
    /// Gets the message associated with the result.
    /// </summary>
    string Message { get; }

    /// <summary>
    /// Gets the exception associated with the result, if any.
    /// </summary>
    Exception Exception { get; }
}

/// <summary>
/// Represents the result of an operation with a value.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
public interface IResult<out T> : IResult
{
    /// <summary>
    /// Gets the value associated with the result.
    /// </summary>
    T Value { get; }
}

/// <summary>
/// Represents a non-generic result of an operation.
/// </summary>
[StructLayout(LayoutKind.Sequential), Serializable]
public readonly struct Result : IResult, ISerializable
{
    /// <inheritdoc/>
    public bool IsSuccess { [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)] get; }

    /// <inheritdoc/>
    public State Status { [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)] get; }

    /// <inheritdoc/>
    public string Message { [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)] get; }

    /// <inheritdoc/>
    public Exception Exception { [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)] get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> struct with the specified state.
    /// </summary>
    /// <param name="state">The result state.</param>
    [DebuggerStepperBoundary]
    internal Result(State state)
    {
        IsSuccess = state == State.Success;
        Status = state;
        Message = string.Empty;
        Exception = default!;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> struct with a validation fault message.
    /// </summary>
    /// <param name="message">The validation fault message.</param>
    [DebuggerStepperBoundary]
    internal Result(string message)
    {
        IsSuccess = false;
        Status = State.ValidationFault;
        Message = message;
        Exception = default!;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> struct with an exception.
    /// </summary>
    /// <param name="exception">The exception.</param>
    [DebuggerStepperBoundary]
    internal Result(Exception exception)
    {
        IsSuccess = false;
        Status = State.Exception;
        Message = string.Empty;
        Exception = exception;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> struct from another <see cref="IResult"/>.
    /// </summary>
    /// <param name="result">The result to copy.</param>
    [DebuggerStepperBoundary]
    internal Result(IResult result)
    {
        IsSuccess = result.IsSuccess;
        Status = result.Status;
        Message = result.Message;
        Exception = result.Exception;
    }

    /// <summary>
    /// Creates a successful <see cref="Result"/>.
    /// </summary>
    /// <returns>A successful result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result Ok() => new Result(State.Success);

    /// <summary>
    /// Creates a failed <see cref="Result"/> with an exception.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <returns>A failed result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result Fail(Exception exception) => new Result(exception);

    /// <summary>
    /// Creates a failed <see cref="Result"/> with a message.
    /// </summary>
    /// <param name="message">The failure message.</param>
    /// <returns>A failed result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result Fail(string message) => new Result(message);

    /// <summary>
    /// Creates a successful <see cref="Result{T}"/> with a value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>A successful result with a value.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> Ok<T>(T value) => Result<T>.Ok(value);

    /// <summary>
    /// Creates a failed <see cref="Result{T}"/> with a message.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="message">The failure message.</param>
    /// <returns>A failed result with a message.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> Fail<T>(string message) => Result<T>.Fail(message);

    /// <summary>
    /// Creates a failed <see cref="Result{T}"/> with an exception.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="exception">The exception.</param>
    /// <returns>A failed result with an exception.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> Fail<T>(Exception exception) => Result<T>.Fail(exception);

    /// <summary>
    /// Forwards a failed <see cref="IResult"/> as a <see cref="Result"/>.
    /// </summary>
    /// <param name="result">The result to forward.</param>
    /// <returns>A forwarded failed result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result ForwardFail(IResult result) => new Result(result);

    /// <summary>
    /// Forwards a failed <see cref="IResult"/> as a <see cref="Result{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="result">The result to forward.</param>
    /// <returns>A forwarded failed result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> ForwardFail<T>(IResult result) => Result<T>.ForwardFail(result);

    /// <summary>
    /// Executes an action and captures any exception as a failed result.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>A successful or failed result.</returns>
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
    /// Executes a function and captures any exception as a failed result.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="func">The function to execute.</param>
    /// <returns>A successful or failed result with a value.</returns>
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
    /// Executes an asynchronous action and captures any exception as a failed result.
    /// </summary>
    /// <param name="action">The asynchronous action to execute.</param>
    /// <returns>A task representing the asynchronous operation, with a successful or failed result.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result> CaptureAsync(Func<Task> action)
    {
        try
        {
            await action();
            return Ok();
        }
        catch (Exception e)
        {
            return Fail(e);
        }
    }

    /// <summary>
    /// Executes an asynchronous function and captures any exception as a failed result.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="func">The asynchronous function to execute.</param>
    /// <returns>A task representing the asynchronous operation, with a successful or failed result with a value.</returns>
    [DebuggerStepperBoundary]
    public static async Task<Result<T>> CaptureAsync<T>(Func<Task<T>> func)
    {
        try
        {
            return Ok(await func());
        }
        catch (Exception e)
        {
            return Fail<T>(e);
        }
    }

    /// <summary>
    /// Returns a string representation of the result.
    /// </summary>
    /// <returns>A string describing the result.</returns>
    public override string ToString()
    {
        return Status switch
        {
            State.Success => $"Success",
            State.ValidationFault => $"Validation Failed: {Message}",
            State.Exception => $"Exception: {Exception.GetType().Name} - {Exception.Message}",
            _ => throw new NotImplementedException()
        };
    }

    /// <summary>
    /// Populates a <see cref="SerializationInfo"/> with the data needed to serialize the <see cref="Result"/>.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> to populate with data.</param>
    /// <param name="context">The destination (see <see cref="StreamingContext"/>) for this serialization.</param>
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue(nameof(IsSuccess), IsSuccess);
        info.AddValue(nameof(Status), Status);

        if (Status == State.ValidationFault)
        {
            info.AddValue(nameof(Message), Message);
        }
        else if (Status == State.Exception)
        {
            info.AddValue(nameof(Exception), Exception, typeof(Exception));
        }
    }

    /// <summary>
    /// Represents the state of a <see cref="Result"/>.
    /// </summary>
    public enum State
    {
        /// <summary>
        /// The operation was successful.
        /// </summary>
        Success,

        /// <summary>
        /// The operation failed due to an exception.
        /// </summary>
        Exception,

        /// <summary>
        /// The operation failed due to a validation fault.
        /// </summary>
        ValidationFault,
    }
}

/// <summary>
/// Represents a result of an operation with a value.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
[StructLayout(LayoutKind.Sequential), Serializable]
public readonly struct Result<T> : IResult<T>, ISerializable
{
    /// <inheritdoc/>
    public bool IsSuccess { [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)] get; }

    /// <inheritdoc/>
    public Result.State Status { [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)] get; }

    /// <inheritdoc/>
    public string Message { [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)] get; }

    /// <inheritdoc/>
    public Exception Exception { [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)] get; }

    /// <inheritdoc/>
    public T Value { [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)] get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{T}"/> struct with a value.
    /// </summary>
    /// <param name="value">The value.</param>
    [DebuggerStepperBoundary]
    internal Result(T value)
    {
        IsSuccess = true;
        Status = Result.State.Success;
        Message = string.Empty;
        Exception = default!;
        Value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{T}"/> struct with a validation fault message.
    /// </summary>
    /// <param name="message">The validation fault message.</param>
    [DebuggerStepperBoundary]
    internal Result(string message)
    {
        IsSuccess = false;
        Status = Result.State.ValidationFault;
        Message = message;
        Exception = default!;
        Value = default!;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{T}"/> struct with an exception.
    /// </summary>
    /// <param name="exception">The exception.</param>
    [DebuggerStepperBoundary]
    internal Result(Exception exception)
    {
        IsSuccess = false;
        Status = Result.State.Exception;
        Message = string.Empty;
        Exception = exception;
        Value = default!;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{T}"/> struct from another <see cref="IResult"/>.
    /// </summary>
    /// <param name="result">The result to copy.</param>
    [DebuggerStepperBoundary]
    internal Result(IResult result)
    {
        IsSuccess = result.IsSuccess;
        Status = result.Status;
        Message = result.Message;
        Exception = result.Exception;
        Value = default!;
    }

    /// <summary>
    /// Creates a successful <see cref="Result{T}"/> with a value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A successful result with a value.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> Ok(T value) => new Result<T>(value);

    /// <summary>
    /// Creates a failed <see cref="Result{T}"/> with a message.
    /// </summary>
    /// <param name="message">The failure message.</param>
    /// <returns>A failed result with a message.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> Fail(string message) => new Result<T>(message);

    /// <summary>
    /// Creates a failed <see cref="Result{T}"/> with an exception.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <returns>A failed result with an exception.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> Fail(Exception exception) => new Result<T>(exception);

    /// <summary>
    /// Forwards a failed <see cref="IResult"/> as a <see cref="Result{T}"/>.
    /// </summary>
    /// <param name="result">The result to forward.</param>
    /// <returns>A forwarded failed result.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> ForwardFail(IResult result) => new Result<T>(result);

    /// <summary>
    /// Explicitly converts a <see cref="Result{T}"/> to a non-generic <see cref="Result"/>.
    /// </summary>
    /// <param name="result">The generic result to convert.</param>
    /// <returns>A non-generic <see cref="Result"/> representing the same state as the input.</returns>
    [DebuggerStepperBoundary, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator Result(Result<T> result) => Result.ForwardFail(result);

    /// <summary>
    /// Returns a string representation of the result.
    /// </summary>
    /// <returns>A string describing the result.</returns>
    public override string ToString()
    {
        return Status switch
        {
            Result.State.Success => $"Success: {Value!.GetType().Name}",
            Result.State.ValidationFault => $"Validation Failed: {Message}",
            Result.State.Exception => $"Exception: {Exception.GetType().Name} - {Exception.Message}",
            _ => throw new NotImplementedException(),
        };
    }

    /// <summary>
    /// Populates a <see cref="SerializationInfo"/> with the data needed to serialize the <see cref="Result{T}"/>.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> to populate with data.</param>
    /// <param name="context">The destination (see <see cref="StreamingContext"/>) for this serialization.</param>
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue(nameof(IsSuccess), IsSuccess);
        info.AddValue(nameof(Status), Status);

        if (Status == Result.State.ValidationFault)
        {
            info.AddValue(nameof(Message), Message);
        }
        else if (Status == Result.State.Exception)
        {
            info.AddValue(nameof(Exception), Exception, typeof(Exception));
        }
        else if (IsSuccess)
        {
            info.AddValue(nameof(Value), Value, typeof(T));
        }
    }
}