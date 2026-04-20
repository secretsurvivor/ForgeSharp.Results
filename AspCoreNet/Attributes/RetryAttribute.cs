namespace ForgeSharp.Results.AspNetCore.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class RetryAttribute(int maxRetries) : Attribute
{
    public int MaxRetries { get; } = maxRetries;
    public TimeSpan Delay { get; set; }
}
