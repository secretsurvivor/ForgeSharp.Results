namespace ForgeSharp.Results;

/// <summary>
/// Common functional type for representing nothing
/// </summary>
public readonly struct Unit
{
    /// <summary>
    /// The single value of the <see cref="Unit"/> type.
    /// </summary>
    public static readonly Unit Value = new();
}
