namespace ForgeSharp.Results.AspNetCore.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public sealed class LongRunningAttribute : Attribute
{
    /// <summary>
    /// Uses the <c>X-Priority</c> header to determine the request's priority
    /// </summary>
    /// <remarks>
    /// Only has an effect if LongRunning service has been configured to be throttled
    /// </remarks>
    public bool UsePriorityHeader { get; set; }

    /// <summary>
    /// Ignores any background throttling and always runs instantly
    /// </summary>
    /// <remarks>
    /// Only has an effect if LongRunning service has been configured to be throttled
    /// </remarks>
    public bool AlwaysRun { get; set; }
}
