using System.Net.Http.Headers;
using System.Text.Json;

namespace StaffAtt.Web.Helpers;

/// <summary>
/// Provides methods for performing HTTP operations against a specified API endpoint.
/// </summary>
/// <remarks>This class utilizes an <see cref="IHttpClientFactory"/> to create HTTP clients and an <see
/// cref="IHttpContextAccessor"/> to access the current HTTP context for retrieving authentication tokens. It supports
/// CRUD operations and handles JSON serialization and deserialization of request and response bodies.</remarks>
public class ApiClient : IApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ApiClient(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    private HttpClient CreateClient()
    {
        var client = _httpClientFactory.CreateClient("api");

        var token = _httpContextAccessor.HttpContext?.Session.GetString("JwtToken");
        if (!string.IsNullOrEmpty(token))
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return client;
    }

    // Common helper for reading response    
    private static async Task<Result<T>> ReadResponseAsync<T>(HttpResponseMessage response, string verb, string endpoint)
    {
        var status = (int)response.StatusCode;

        // Case 1: Error status code
        if (!response.IsSuccessStatusCode)
        {
            var errorText = await response.Content.ReadAsStringAsync();
            return Result<T>.Failure($"{verb} {endpoint} failed ({status}): {response.ReasonPhrase}. {errorText}");
        }

        // Case 2: No content (e.g. 204 or empty body)
        if (response.StatusCode == System.Net.HttpStatusCode.NoContent ||
            response.Content.Headers.ContentLength == 0)
        {
            // For POST we may not expect body, so success anyway
            if (typeof(T) == typeof(bool))
                return Result<T>.Success((T)(object)true);

            return Result<T>.Failure($"{verb} {endpoint} returned no content.");
        }

        // Case 3: Try to parse JSON
        try
        {
            var stream = await response.Content.ReadAsStreamAsync();
            var value = await JsonSerializer.DeserializeAsync<T>(stream, _jsonOptions);

            if (value == null)
                return Result<T>.Failure($"{verb} {endpoint} returned null JSON.");

            return Result<T>.Success(value);
        }
        catch (JsonException)
        {
            // Maybe server returned a plain string or HTML
            var raw = await response.Content.ReadAsStringAsync();
            if (typeof(T) == typeof(string))
                return Result<T>.Success((T)(object)raw.Trim());

            // If POST succeeded but returned plain text, just mark success
            if (verb == "POST" && response.IsSuccessStatusCode)
                return Result<T>.Success(default!);

            return Result<T>.Failure($"{verb} {endpoint} returned non-JSON response: {raw}");
        }
    }

    // CRUD methods    
    public async Task<Result<T>> GetAsync<T>(string endpoint)
    {
        var client = CreateClient();
        var response = await client.GetAsync(endpoint);
        return await ReadResponseAsync<T>(response, "GET", endpoint);
    }

    public async Task<Result<T>> PostAsync<T>(string endpoint, T data)
    {
        var client = CreateClient();
        var response = await client.PostAsJsonAsync(endpoint, data, _jsonOptions);
        return await ReadResponseAsync<T>(response, "POST", endpoint);
    }

    public async Task<Result<T>> PutAsync<T>(string endpoint, T data)
    {
        var client = CreateClient();
        var response = await client.PutAsJsonAsync(endpoint, data, _jsonOptions);
        return await ReadResponseAsync<T>(response, "PUT", endpoint);
    }

    public async Task<Result<bool>> DeleteAsync(string endpoint)
    {
        var client = CreateClient();
        var response = await client.DeleteAsync(endpoint);
        var result = await ReadResponseAsync<string>(response, "DELETE", endpoint);

        return result.IsSuccess
            ? Result<bool>.Success(true)
            : Result<bool>.Failure(result.ErrorMessage);
    }
}
