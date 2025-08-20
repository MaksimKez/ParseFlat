using System.Net.Http.Json;
using Polly;

namespace Infrastructure.UserServiceClient;


public abstract class BaseHttpService(HttpClient httpClient, ResiliencePipeline resiliencePipeline)
{
    protected async Task<TResult> ExecuteGetAsync<TResult, TResponse>(
        string endpoint, 
        Func<TResponse?, TResult> onSuccess,
        Func<string, int, TResult> onError,
        CancellationToken cancellationToken = default)
        where TResponse : class
    {
        return await resiliencePipeline.ExecuteAsync(async token =>
        {
            var response = await httpClient.GetAsync(endpoint, token);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await SafeReadResponseBodyAsync(response);
                return onError(errorBody, (int)response.StatusCode);
            }

            var data = await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: token);
            
            if (data == null)
            {
                return onError("Response body is null", (int)response.StatusCode);
            }

            return onSuccess(data);
        }, cancellationToken);
    }

    protected async Task<TResult> ExecutePostAsync<TResult, TRequest, TResponse>(
        string endpoint,
        TRequest request,
        Func<TResponse?, TResult> onSuccess,
        Func<string, int, TResult> onError,
        CancellationToken cancellationToken = default)
        where TResponse : class
    {
        return await resiliencePipeline.ExecuteAsync(async token =>
        {
            var response = await httpClient.PostAsJsonAsync(endpoint, request, token);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await SafeReadResponseBodyAsync(response);
                return onError(errorBody, (int)response.StatusCode);
            }

            var data = await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: token);
            
            if (data == null)
            {
                return onError("Response body is null", (int)response.StatusCode);
            }

            return onSuccess(data);
        }, cancellationToken);
    }

    private static async Task<string> SafeReadResponseBodyAsync(HttpResponseMessage response)
    {
        try
        {
            var body = await response.Content.ReadAsStringAsync();
            return string.IsNullOrWhiteSpace(body) 
                ? $"HTTP {(int)response.StatusCode} {response.ReasonPhrase}"
                : $"HTTP {(int)response.StatusCode} {response.ReasonPhrase}: {body}";
        }
        catch
        {
            return $"HTTP {(int)response.StatusCode} {response.ReasonPhrase}";
        }
    }
}