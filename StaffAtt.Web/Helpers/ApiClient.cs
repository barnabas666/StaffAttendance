using System.Net.Http.Headers;
using System.Text.Json;

namespace StaffAtt.Web.Helpers;

public class ApiClient : IApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiClient(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;

        // consistent serializer options across methods
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    private HttpClient CreateClient()
    {
        var client = _httpClientFactory.CreateClient("api");

        var token = _httpContextAccessor.HttpContext?.Session.GetString("JwtToken");
        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return client;
    }

    // shared helper for consistent deserialization
    private async Task<Result<T>> DeserializeResponseAsync<T>(HttpResponseMessage response, string endpoint, string verb)
    {
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return Result<T>.Failure($"{verb} {endpoint} failed with {(int)response.StatusCode} {response.ReasonPhrase}. {error}");
        }

        try
        {
            // Try to parse JSON
            var value = await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
            if (value is not null)
                return Result<T>.Success(value);

            // Sometimes servers return an empty body
            return Result<T>.Failure($"{verb} {endpoint} returned empty response.");
        }
        catch (JsonException)
        {
            // If not JSON, try reading as plain text
            var raw = await response.Content.ReadAsStringAsync();
            if (typeof(T) == typeof(string))
                return Result<T>.Success((T)(object)raw.Trim());

            return Result<T>.Failure($"Failed to parse response from {verb} {endpoint}. Expected JSON but got raw text: {raw}");
        }
        catch (Exception ex)
        {
            return Result<T>.Failure($"Unexpected error while reading response from {verb} {endpoint}. {ex.Message}");
        }
    }

    public async Task<Result<T>> GetAsync<T>(string endpoint)
    {
        var client = CreateClient();
        var response = await client.GetAsync(endpoint);
        return await DeserializeResponseAsync<T>(response, endpoint, "GET");
    }

    public async Task<Result<T>> PostAsync<T>(string endpoint, T data)
    {
        var client = CreateClient();
        var response = await client.PostAsJsonAsync(endpoint, data, _jsonOptions);
        return await DeserializeResponseAsync<T>(response, endpoint, "POST");
    }

    public async Task<Result<T>> PutAsync<T>(string endpoint, T data)
    {
        var client = CreateClient();
        var response = await client.PutAsJsonAsync(endpoint, data, _jsonOptions);
        return await DeserializeResponseAsync<T>(response, endpoint, "PUT");
    }

    public async Task<Result<bool>> DeleteAsync(string endpoint)
    {
        var client = CreateClient();
        var response = await client.DeleteAsync(endpoint);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return Result<bool>.Failure($"DELETE {endpoint} failed with {(int)response.StatusCode} {response.ReasonPhrase}. {error}");
        }

        return Result<bool>.Success(true);
    }
}
