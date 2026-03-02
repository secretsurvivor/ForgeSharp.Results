using ForgeSharp.Results.Infrastructure;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace ForgeSharp.Results.Integration;

/// <summary>
/// Provides extension methods for <see cref="HttpClient"/> to wrap HTTP operations in <see cref="Result"/> types.
/// </summary>
public static class HttpClientExtension
{
    extension(HttpClient client)
    {
        /// <summary>
        /// Sends an HTTP request asynchronously and returns the result wrapped in a <see cref="Result{T}"/>.
        /// </summary>
        /// <param name="request">The HTTP request to send.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation, with a result containing the HTTP response message or an error.</returns>
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
        /// Sends an HTTP request asynchronously with a completion option and returns the result wrapped in a <see cref="Result{T}"/>.
        /// </summary>
        /// <param name="request">The HTTP request to send.</param>
        /// <param name="completionOption">The completion option that determines when the task completes.</param>
        /// <returns>A task representing the asynchronous operation, with a result containing the HTTP response message or an error.</returns>
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
        /// Sends an HTTP request asynchronously with a completion option and cancellation token, returning the result wrapped in a <see cref="Result{T}"/>.
        /// </summary>
        /// <param name="request">The HTTP request to send.</param>
        /// <param name="completionOption">The completion option that determines when the task completes.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation, with a result containing the HTTP response message or an error.</returns>
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
        /// Sends an HTTP GET request asynchronously to the specified URI and returns the result wrapped in a <see cref="Result{T}"/>.
        /// </summary>
        /// <param name="requestUri">The URI for the HTTP request.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation, with a result containing the HTTP response message or an error.</returns>
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
        /// Sends an HTTP GET request asynchronously to the specified URI and returns the result wrapped in a <see cref="Result{T}"/>.
        /// </summary>
        /// <param name="uri">The URI for the HTTP request.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation, with a result containing the HTTP response message or an error.</returns>
        public Task<Result<HttpResponseMessage>> GetAsResultAsync(Uri? uri, CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            return SendAsResultAsync(client, request, cancellationToken);
        }

        /// <summary>
        /// Sends an HTTP GET request asynchronously to the specified URI with a completion option and returns the result wrapped in a <see cref="Result{T}"/>.
        /// </summary>
        /// <param name="requestUri">The URI for the HTTP request.</param>
        /// <param name="completionOption">The completion option that determines when the task completes.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation, with a result containing the HTTP response message or an error.</returns>
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
        /// Sends an HTTP GET request asynchronously to the specified URI with a completion option and returns the result wrapped in a <see cref="Result{T}"/>.
        /// </summary>
        /// <param name="uri">The URI for the HTTP request.</param>
        /// <param name="completionOption">The completion option that determines when the task completes.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation, with a result containing the HTTP response message or an error.</returns>
        public Task<Result<HttpResponseMessage>> GetAsResultAsync(Uri? uri, HttpCompletionOption completionOption, CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            return SendAsResultAsync(client, request, completionOption, cancellationToken);
        }

        /// <summary>
        /// Sends an HTTP POST request asynchronously to the specified URI with the given content and returns the result wrapped in a <see cref="Result{T}"/>.
        /// </summary>
        /// <param name="requestUri">The URI for the HTTP request.</param>
        /// <param name="content">The HTTP content to send with the request.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation, with a result containing the HTTP response message or an error.</returns>
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
        /// Sends an HTTP POST request asynchronously to the specified URI with the given content and returns the result wrapped in a <see cref="Result{T}"/>.
        /// </summary>
        /// <param name="uri">The URI for the HTTP request.</param>
        /// <param name="content">The HTTP content to send with the request.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation, with a result containing the HTTP response message or an error.</returns>
        public Task<Result<HttpResponseMessage>> PostAsResultAsync(Uri? uri, HttpContent content, CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = content,
            };

            return SendAsResultAsync(client, request, cancellationToken);
        }

        /// <summary>
        /// Sends an HTTP PUT request asynchronously to the specified URI with the given content and returns the result wrapped in a <see cref="Result{T}"/>.
        /// </summary>
        /// <param name="requestUri">The URI for the HTTP request.</param>
        /// <param name="content">The HTTP content to send with the request.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation, with a result containing the HTTP response message or an error.</returns>
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
        /// Sends an HTTP PUT request asynchronously to the specified URI with the given content and returns the result wrapped in a <see cref="Result{T}"/>.
        /// </summary>
        /// <param name="uri">The URI for the HTTP request.</param>
        /// <param name="content">The HTTP content to send with the request.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation, with a result containing the HTTP response message or an error.</returns>
        public Task<Result<HttpResponseMessage>> PutAsResultAsync(Uri? uri, HttpContent content, CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, uri)
            {
                Content = content,
            };

            return SendAsResultAsync(client, request, cancellationToken);
        }

        /// <summary>
        /// Sends an HTTP DELETE request asynchronously to the specified URI and returns the result wrapped in a <see cref="Result{T}"/>.
        /// </summary>
        /// <param name="requestUri">The URI for the HTTP request.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation, with a result containing the HTTP response message or an error.</returns>
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
        /// Sends an HTTP DELETE request asynchronously to the specified URI and returns the result wrapped in a <see cref="Result{T}"/>.
        /// </summary>
        /// <param name="uri">The URI for the HTTP request.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation, with a result containing the HTTP response message or an error.</returns>
        public Task<Result<HttpResponseMessage>> DeleteAsResultAsync(Uri? uri, CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, uri);

            return SendAsResultAsync(client, request, cancellationToken);
        }
    }

    /// <summary>
    /// Ensures that an HTTP response result has a successful status code, disposing the response on failure.
    /// </summary>
    /// <param name="result">The result containing an HTTP response message.</param>
    /// <returns>A result containing the response message if successful, otherwise a failed result with an error message describing the failure.</returns>
    /// <remarks>
    /// If the response status code indicates failure (not in the 2xx range), the response message is disposed
    /// and a failed result is returned containing the status code and reason phrase.
    /// </remarks>
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
    /// Ensures that an asynchronous HTTP response result has a successful status code, disposing the response on failure.
    /// </summary>
    /// <param name="taskResponse">A task representing the result containing an HTTP response message.</param>
    /// <returns>A task representing the asynchronous operation, with a result containing the response message if successful,
    /// otherwise a failed result with an error message describing the failure.</returns>
    /// <remarks>
    /// If the response status code indicates failure (not in the 2xx range), the response message is disposed
    /// and a failed result is returned containing the status code and reason phrase.
    /// </remarks>
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
    /// Ensures that an HTTP response result has a successful status code and transforms it using the appropriate handler function.
    /// </summary>
    /// <typeparam name="T">The type of the value produced by the handler functions.</typeparam>
    /// <param name="result">The result containing an HTTP response message.</param>
    /// <param name="successFunc">The function to invoke if the response has a successful status code.</param>
    /// <param name="failureFunc">The function to invoke if the response has a failed status code.</param>
    /// <param name="dispose">Whether to dispose the HTTP response message after handling. Default is true.</param>
    /// <returns>A result containing the value produced by the appropriate handler function, or a failure if the input result failed.</returns>
    /// <remarks>
    /// The specified handler function (either <paramref name="successFunc"/> or <paramref name="failureFunc"/>) is always invoked
    /// with the HTTP response message. The response is disposed after handling (unless <paramref name="dispose"/> is false)
    /// or if the input result is already failed.
    /// </remarks>
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
    /// Ensures that an asynchronous HTTP response result has a successful status code and transforms it using the appropriate handler function.
    /// </summary>
    /// <typeparam name="T">The type of the value produced by the handler functions.</typeparam>
    /// <param name="taskResponse">A task representing the result containing an HTTP response message.</param>
    /// <param name="successFunc">The function to invoke if the response has a successful status code.</param>
    /// <param name="failureFunc">The function to invoke if the response has a failed status code.</param>
    /// <param name="dispose">Whether to dispose the HTTP response message after handling. Default is true.</param>
    /// <returns>A task representing the asynchronous operation, with a result containing the value produced by the appropriate handler function,
    /// or a failure if the input result failed.</returns>
    /// <remarks>
    /// The specified handler function (either <paramref name="successFunc"/> or <paramref name="failureFunc"/>) is always invoked
    /// with the HTTP response message. The response is disposed after handling (unless <paramref name="dispose"/> is false)
    /// or if the input result is already failed.
    /// </remarks>
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
