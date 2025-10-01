using System.Net.Http.Headers;

namespace StaffAtt.Web.Helpers;

public class ApiClient : IApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

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
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return client;
    }

    public async Task<Result<T>> GetAsync<T>(string endpoint)
    {
        var client = CreateClient();
        var response = await client.GetAsync(endpoint);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return Result<T>.Failure(
                $"GET {endpoint} failed with {(int)response.StatusCode} {response.ReasonPhrase}. {error}"
            );
        }

        try
        {
            var value = await response.Content.ReadFromJsonAsync<T>();
            if (value is null)
                return Result<T>.Failure($"GET {endpoint} returned empty response.");

            return Result<T>.Success(value);
        }
        catch (Exception ex)
        {
            return Result<T>.Failure($"Failed to deserialize response from {endpoint}. {ex.Message}");
        }
    }

    public async Task<Result<T>> PostAsync<T>(string endpoint, T data)
    {
        var client = CreateClient();
        var response = await client.PostAsJsonAsync(endpoint, data);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return Result<T>.Failure(
                $"POST {endpoint} failed with {(int)response.StatusCode} {response.ReasonPhrase}. {error}"
            );
        }

        return Result<T>.Success(data);
    }
}

