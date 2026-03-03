using ForgeSharp.Results.Infrastructure;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace ForgeSharp.Results.Integration;

/// <summary>
/// Result-returning wrappers for <see cref="HttpClient"/> operations.
/// </summary>
public static class HttpClientExtension
{
    extension(HttpClient client)
    {
        /// <summary>
        /// Sends a request as a Result.
        /// </summary>
        /// <param name="request">The HTTP request to send.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>The response, or a failure.</returns>
        public async Task<Result<HttpResponseMessage>> SendAsResultAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            try
            {
                return Result.Ok(await client.SendAsync(request, cancellationToken).ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                return Result.Fail<HttpResponseMessage>(ex);
            }
        }

        /// <summary>
        /// Sends a request with a completion option, as a Result.
        /// </summary>
        /// <param name="request">The HTTP request to send.</param>
        /// <param name="completionOption">The completion option that determines when the task completes.</param>
        /// <returns>The response, or a failure.</returns>
        public async Task<Result<HttpResponseMessage>> SendAsResultAsync(HttpRequestMessage request, HttpCompletionOption completionOption)
        {
            try
            {
                return Result.Ok(await client.SendAsync(request, completionOption).ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                return Result.Fail<HttpResponseMessage>(ex);
            }
        }

        /// <summary>
        /// Sends a request with a completion option, as a Result.
        /// </summary>
        /// <param name="request">The HTTP request to send.</param>
        /// <param name="completionOption">The completion option that determines when the task completes.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>The response, or a failure.</returns>
        public async Task<Result<HttpResponseMessage>> SendAsResultAsync(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken = default)
        {
            try
            {
                return Result.Ok(await client.SendAsync(request, completionOption, cancellationToken).ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                return Result.Fail<HttpResponseMessage>(ex);
            }
        }

        /// <summary>
        /// Sends a GET request as a Result.
        /// </summary>
        /// <param name="requestUri">The URI for the HTTP request.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>The response, or a failure.</returns>
        public Task<Result<HttpResponseMessage>> GetAsResultAsync(
#if NET6_0_OR_GREATER
            [StringSyntax(StringSyntaxAttribute.Uri)]
#endif
        string? requestUri, CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

            return SendAsResultAsync(client, request, cancellationToken);
        }

        /// <summary>
        /// Sends a GET request as a Result.
        /// </summary>
        /// <param name="uri">The URI for the HTTP request.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>The response, or a failure.</returns>
        public Task<Result<HttpResponseMessage>> GetAsResultAsync(Uri? uri, CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            return SendAsResultAsync(client, request, cancellationToken);
        }

        /// <summary>
        /// Sends a GET request with a completion option, as a Result.
        /// </summary>
        /// <param name="requestUri">The URI for the HTTP request.</param>
        /// <param name="completionOption">The completion option that determines when the task completes.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>The response, or a failure.</returns>
        public Task<Result<HttpResponseMessage>> GetAsResultAsync(
#if NET6_0_OR_GREATER
            [StringSyntax(StringSyntaxAttribute.Uri)]
#endif
        string? requestUri, HttpCompletionOption completionOption, CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

            return SendAsResultAsync(client, request, completionOption, cancellationToken);
        }

        /// <summary>
        /// Sends a GET request with a completion option, as a Result.
        /// </summary>
        /// <param name="uri">The URI for the HTTP request.</param>
        /// <param name="completionOption">The completion option that determines when the task completes.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>The response, or a failure.</returns>
        public Task<Result<HttpResponseMessage>> GetAsResultAsync(Uri? uri, HttpCompletionOption completionOption, CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            return SendAsResultAsync(client, request, completionOption, cancellationToken);
        }

        /// <summary>
        /// Sends a POST request as a Result.
        /// </summary>
        /// <param name="requestUri">The URI for the HTTP request.</param>
        /// <param name="content">The HTTP content to send with the request.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>The response, or a failure.</returns>
        public Task<Result<HttpResponseMessage>> PostAsResultAsync(
#if NET6_0_OR_GREATER
            [StringSyntax(StringSyntaxAttribute.Uri)]
#endif
        string? requestUri, HttpContent content, CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = content,
            };

            return SendAsResultAsync(client, request, cancellationToken);
        }

        /// <summary>
        /// Sends a POST request as a Result.
        /// </summary>
        /// <param name="uri">The URI for the HTTP request.</param>
        /// <param name="content">The HTTP content to send with the request.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>The response, or a failure.</returns>
        public Task<Result<HttpResponseMessage>> PostAsResultAsync(Uri? uri, HttpContent content, CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = content,
            };

            return SendAsResultAsync(client, request, cancellationToken);
        }

        /// <summary>
        /// Sends a PUT request as a Result.
        /// </summary>
        /// <param name="requestUri">The URI for the HTTP request.</param>
        /// <param name="content">The HTTP content to send with the request.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>The response, or a failure.</returns>
        public Task<Result<HttpResponseMessage>> PutAsResultAsync(
#if NET6_0_OR_GREATER
            [StringSyntax(StringSyntaxAttribute.Uri)]
#endif
        string? requestUri, HttpContent content, CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, requestUri)
            {
                Content = content,
            };

            return SendAsResultAsync(client, request, cancellationToken);
        }

        /// <summary>
        /// Sends a PUT request as a Result.
        /// </summary>
        /// <param name="uri">The URI for the HTTP request.</param>
        /// <param name="content">The HTTP content to send with the request.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>The response, or a failure.</returns>
        public Task<Result<HttpResponseMessage>> PutAsResultAsync(Uri? uri, HttpContent content, CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, uri)
            {
                Content = content,
            };

            return SendAsResultAsync(client, request, cancellationToken);
        }

        /// <summary>
        /// Sends a DELETE request as a Result.
        /// </summary>
        /// <param name="requestUri">The URI for the HTTP request.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>The response, or a failure.</returns>
        public Task<Result<HttpResponseMessage>> DeleteAsResultAsync(
#if NET6_0_OR_GREATER
            [StringSyntax(StringSyntaxAttribute.Uri)]
#endif
        string? requestUri, CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, requestUri);

            return SendAsResultAsync(client, request, cancellationToken);
        }

        /// <summary>
        /// Sends a DELETE request as a Result.
        /// </summary>
        /// <param name="uri">The URI for the HTTP request.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>The response, or a failure.</returns>
        public Task<Result<HttpResponseMessage>> DeleteAsResultAsync(Uri? uri, CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, uri);

            return SendAsResultAsync(client, request, cancellationToken);
        }
    }

    /// <summary>
    /// Fails the result if the status code is not 2xx, disposing the response.
    /// </summary>
    /// <param name="result">The result containing an HTTP response message.</param>
    /// <returns>The response on 2xx, or a failure with the status code.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<HttpResponseMessage> ThenEnsureSuccessStatusCode(this Result<HttpResponseMessage> result)
    {
        if (!result.IsSuccess)
        {
            return Result.ForwardFail<HttpResponseMessage>(result);
        }

        if (!result.Value.IsSuccessStatusCode)
        {
            try
            {
                return Result.Fail<HttpResponseMessage>($"HttpRequest failed with {result.Value.StatusCode} status code and reason phrase {result.Value.ReasonPhrase}");
            }
            finally
            {
                result.Value.Dispose();
            }
        }

        return result;
    }

    /// <summary>
    /// Async version of <see cref="ThenEnsureSuccessStatusCode(Result{HttpResponseMessage})"/>.
    /// </summary>
    /// <param name="taskResponse">A task representing the result containing an HTTP response message.</param>
    /// <returns>The response on 2xx, or a failure with the status code.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result<HttpResponseMessage>> ThenEnsureSuccessStatusCodeAsync(this Task<Result<HttpResponseMessage>> taskResponse)
    {
        if (taskResponse.TryGetResult(out var result))
        {
            return Task.FromResult(ThenEnsureSuccessStatusCode(result));
        }

        return Impl(taskResponse);

        static async Task<Result<HttpResponseMessage>> Impl(Task<Result<HttpResponseMessage>> taskResponse)
        {
            var responseResult = await taskResponse.ConfigureAwait(false);

            if (!responseResult.IsSuccess)
            {
                return Result.ForwardFail<HttpResponseMessage>(responseResult);
            }

            if (!responseResult.Value.IsSuccessStatusCode)
            {
                try
                {
                    return Result.Fail<HttpResponseMessage>($"HttpRequest failed with {responseResult.Value.StatusCode} status code and reason phrase {responseResult.Value.ReasonPhrase}");
                }
                finally
                {
                    responseResult.Value.Dispose();
                }
            }

            return responseResult;
        }
    }

    /// <summary>
    /// Routes the response to <paramref name="successFunc"/> or <paramref name="failureFunc"/> based on the status code.
    /// </summary>
    /// <typeparam name="T">The type of the value produced by the handler functions.</typeparam>
    /// <param name="result">The result containing an HTTP response message.</param>
    /// <param name="successFunc">The function to invoke if the response has a successful status code.</param>
    /// <param name="failureFunc">The function to invoke if the response has a failed status code.</param>
    /// <param name="dispose">Whether to dispose the HTTP response message after handling. Default is true.</param>
    /// <returns>The handler's return value, or the forwarded failure.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> ThenEnsureSuccessStatusCode<T>(this Result<HttpResponseMessage> result, Func<HttpResponseMessage, T> successFunc, Func<HttpResponseMessage, T> failureFunc, bool dispose = true)
    {
        if (!result.IsSuccess)
        {
            return Result.ForwardFail<T>(result);
        }

        try
        {
            if (!result.Value.IsSuccessStatusCode)
            {
                return Result.Ok(failureFunc(result.Value));
            }

            return Result.Ok(successFunc(result.Value));
        }
        finally
        {
            if (dispose)
            {
                result.Value.Dispose();
            }
        }
    }

    /// <summary>
    /// Async version of the corresponding <see cref="ThenEnsureSuccessStatusCode{T}(Result{HttpResponseMessage}, Func{HttpResponseMessage, T}, Func{HttpResponseMessage, T}, bool)"/> overload.
    /// </summary>
    /// <typeparam name="T">The type of the value produced by the handler functions.</typeparam>
    /// <param name="taskResponse">A task representing the result containing an HTTP response message.</param>
    /// <param name="successFunc">The function to invoke if the response has a successful status code.</param>
    /// <param name="failureFunc">The function to invoke if the response has a failed status code.</param>
    /// <param name="dispose">Whether to dispose the HTTP response message after handling. Default is true.</param>
    /// <returns>The handler's return value, or the forwarded failure.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result<T>> ThenEnsureSuccessStatusCode<T>(this Task<Result<HttpResponseMessage>> taskResponse, Func<HttpResponseMessage, T> successFunc, Func<HttpResponseMessage, T> failureFunc, bool dispose = true)
    {
        if (taskResponse.TryGetResult(out var result))
        {
            return Task.FromResult(ThenEnsureSuccessStatusCode(result, successFunc, failureFunc, dispose));
        }

        return Impl(taskResponse, successFunc, failureFunc, dispose);

        static async Task<Result<T>> Impl(Task<Result<HttpResponseMessage>> taskResponse, Func<HttpResponseMessage, T> successFunc, Func<HttpResponseMessage, T> failureFunc, bool dispose)
        {
            var responseResult = await taskResponse.ConfigureAwait(false);

            if (!responseResult.IsSuccess)
            {
                return Result.ForwardFail<T>(responseResult);
            }

            try
            {
                if (!responseResult.Value.IsSuccessStatusCode)
                {
                    return Result.Ok(failureFunc(responseResult.Value));
                }

                return Result.Ok(successFunc(responseResult.Value));
            }
            finally
            {
                if (dispose)
                {
                    responseResult.Value.Dispose();
                }
            }
        }
    }
}
