using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ForgeSharp.Results.AspNetCore.Core;

public record JobState : IResult
{
    public required string JobToken { get; init; }
    public required State CurrentState { get; init; }
    public string? Message { get; init; }

    [MemberNotNullWhen(true, nameof(Result))]
    public bool IsFinished => CurrentState is State.Completed or State.Failed;
    public IResult? Result { get; init; }

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = StatusCodes.Status200OK;
        string? result = null;

        if (IsFinished)
        {
            result = UriHelper.BuildAbsolute(httpContext.Request.Scheme, httpContext.Request.Host, pathBase: httpContext.Request.PathBase, path: $"/__longrunning/{JobToken}/result");
        }

        var envelope = new
        {
            status = CurrentState switch
            {
                State.Running => "running",
                State.Canceled => "canceled",
                State.Completed => "completed",
                State.Failed => "failed",
                _ => "unknown",
            },
            Message,
            result
        };

        await JsonSerializer.SerializeAsync(
            httpContext.Response.Body,
            envelope,
            envelope.GetType(),
            JsonSerializerOptions.Web);
    }

    public enum State
    {
        Waiting,
        Running,
        Canceled,
        Completed,
        Failed,
    }
}
