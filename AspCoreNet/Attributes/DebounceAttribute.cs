namespace ForgeSharp.Results.AspNetCore.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public sealed class DebounceAttribute(int milliseconds) : Attribute
{
    public int Milliseconds { get; } = milliseconds;
}
