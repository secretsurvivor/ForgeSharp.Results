namespace ForgeSharp.Results;

/// <summary>
/// Optional value wrapper.
/// </summary>
/// <typeparam name="T">The value type.</typeparam>
public readonly struct Options<T>
{
    /// <summary>
    /// True if a value is present.
    /// </summary>
    public bool HasValue { get; }

    /// <summary>
    /// The value, if present.
    /// </summary>
    public T Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Options{T}"/> struct.
    /// </summary>
    /// <param name="hasValue">A value indicating whether the option has a value.</param>
    /// <param name="value">The value of the option.</param>
    internal Options(bool hasValue, T value)
    {
        HasValue = hasValue;
        Value = value;
    }

    /// <summary>
    /// Creates an option with a value.
    /// </summary>
    /// <param name="value">The value to wrap in the option.</param>
    /// <returns>An option containing the specified value.</returns>
    public static Options<T> Some(T value)
    {
        return new Options<T>(true, value);
    }

    /// <summary>
    /// Creates an empty option.
    /// </summary>
    /// <returns>An empty option.</returns>
    public static Options<T> None()
    {
        return new Options<T>(false, default!);
    }
}
