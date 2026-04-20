namespace ForgeSharp.Results.AspNetCore.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class TimeoutAttribute(TimeSpan timeout) : Attribute
{
    public TimeSpan Timeout { get; } = timeout;
}
