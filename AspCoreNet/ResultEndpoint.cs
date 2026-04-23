using ForgeSharp.Results.AspNetCore.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace ForgeSharp.Results.AspNetCore;

public sealed class ResultEndpoint : IResult
{
    private readonly IResult? _override;
    private readonly Result _result;

    public static implicit operator ResultEndpoint(Result result) => new ResultEndpoint(result);
    public static implicit operator Result(ResultEndpoint resultEndpoint) => resultEndpoint._result;

    public ResultEndpoint(Result result) => _result = result;
    internal ResultEndpoint(IResult @override) => _override = @override;

    public Task ExecuteAsync(HttpContext httpContext)
    {
        if (_override is not null)
        {
            return _override.ExecuteAsync(httpContext);
        }

        var result = _result;

        if (result.IsSuccess)
        {
            httpContext.Response.StatusCode = StatusCodes.Status204NoContent;
            return Task.CompletedTask;
        }

        var factory = httpContext.RequestServices.GetRequiredService<IResultErrorMapperFactory>();
        var problemDetailsService = httpContext.RequestServices.GetService<IProblemDetailsService>();

        if (problemDetailsService is null)
        {
            throw new InvalidOperationException("ForgeSharp.Results.AspNetCore requires ProblemDetailsService. Use the AddProblemDetails on IServiceCollection to add it to the Service Provider.");
        }

        if (result.IsValidationFault && factory.TryGetMapper<string>(out var validationMapper))
        {
            var problem = validationMapper.ConvertError(result.Message);
            httpContext.Response.StatusCode = problem.Status ?? StatusCodes.Status400BadRequest;

            return problemDetailsService.WriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problem
            }).AsTask();
        }

        if (result.IsException && factory.TryGetMapper<Exception>(out var exceptionMapper))
        {
            var problem = exceptionMapper.ConvertError(result.Exception);
            httpContext.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;

            return problemDetailsService.WriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problem,
                Exception = result._exception,
            }).AsTask();
        }

        httpContext.Response.StatusCode = result.IsException
            ? StatusCodes.Status500InternalServerError
            : StatusCodes.Status400BadRequest;

        return problemDetailsService.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails =
            {
                Status = httpContext.Response.StatusCode,
                Detail = result.IsValidationFault
                    ? result.Message
                    : result.Exception?.Message,
            },
            Exception = result._exception,
        }).AsTask();
    }
}

public sealed class ResultEndpoint<T> : IResult
{
    private readonly IResult? _override;
    private readonly Result<T> _result;

    public static implicit operator ResultEndpoint<T>(Result<T> result) => new ResultEndpoint<T>(result);
    public static implicit operator Result<T>(ResultEndpoint<T> resultEndpoint) => resultEndpoint._result;

    public ResultEndpoint(Result<T> result) => _result = result;
    internal ResultEndpoint(IResult result) => _override = result;

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        if (_override is not null)
        {
            await _override.ExecuteAsync(httpContext);
            return;
        }

        var result = _result;

        if (result.IsSuccess)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = StatusCodes.Status200OK;

            await JsonSerializer.SerializeAsync(
                httpContext.Response.Body,
                result.Value,
                JsonSerializerOptions.Web);

            return;
        }

        var factory = httpContext.RequestServices.GetRequiredService<IResultErrorMapperFactory>();
        var problemDetailsService = httpContext.RequestServices.GetService<IProblemDetailsService>();

        if (problemDetailsService is null)
        {
            throw new InvalidOperationException("ForgeSharp.Results.AspNetCore requires ProblemDetailsService. Use the AddProblemDetails on IServiceCollection to add it to the Service Provider.");
        }

        if (result.IsValidationFault && factory.TryGetMapper<string>(out var validationMapper))
        {
            var problem = validationMapper.ConvertError(result.Message);
            httpContext.Response.StatusCode = problem.Status ?? StatusCodes.Status400BadRequest;

            await problemDetailsService.WriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problem
            });

            return;
        }

        if (result.IsException && factory.TryGetMapper<Exception>(out var exceptionMapper))
        {
            var problem = exceptionMapper.ConvertError(result.Exception);
            httpContext.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;

            await problemDetailsService.WriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problem,
                Exception = result._exception,
            });

            return;
        }

        httpContext.Response.StatusCode = result.IsException
            ? StatusCodes.Status500InternalServerError
            : StatusCodes.Status400BadRequest;

        await problemDetailsService.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails =
            {
                Status = httpContext.Response.StatusCode,
                Detail = result.IsValidationFault
                    ? result.Message
                    : result.Exception?.Message,
            },
            Exception = result._exception,
        });
    }
}

public sealed class ResultEndpoint<T, TError> : IResult
{
    private readonly IResult? _override;
    private readonly Result<T, TError> _result;

    public static implicit operator ResultEndpoint<T, TError>(Result<T, TError> result) => new ResultEndpoint<T, TError>(result);
    public static implicit operator Result<T, TError>(ResultEndpoint<T, TError> resultEndpoint) => resultEndpoint._result;

    public ResultEndpoint(Result<T, TError> result) => _result = result;
    internal ResultEndpoint(IResult result) => _override = result;

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        if (_override is not null)
        {
            await _override.ExecuteAsync(httpContext);
            return;
        }

        var result = _result;

        if (result.IsSuccess)
        {
            if (result.Value is Unit)
            {
                httpContext.Response.StatusCode = StatusCodes.Status204NoContent;
                return;
            }

            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = StatusCodes.Status200OK;

            await JsonSerializer.SerializeAsync(
                httpContext.Response.Body,
                result.Value,
                JsonSerializerOptions.Web);

            return;
        }

        var factory = httpContext.RequestServices.GetRequiredService<IResultErrorMapperFactory>();
        var problemDetailsService = httpContext.RequestServices.GetService<IProblemDetailsService>();

        if (problemDetailsService is null)
        {
            throw new InvalidOperationException("ForgeSharp.Results.AspNetCore requires ProblemDetailsService. Use the AddProblemDetails on IServiceCollection to add it to the Service Provider.");
        }

        if (factory.TryGetMapper<TError>(out var error))
        {
            var problem = error.ConvertError(result.Error);
            httpContext.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;

            await problemDetailsService.WriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problem,
            });

            return;
        }

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        await problemDetailsService.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails =
            {
                Status = httpContext.Response.StatusCode,
                Detail = result.Error?.ToString() ?? "An unknown error occurred",
            },
        });
    }
}