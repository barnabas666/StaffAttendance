using StaffAtt.Web.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace StaffAtt.Web.Helpers;

/// <summary>
/// Implementation of <see cref="IApiClient"/> for making HTTP requests to a RESTful API.
/// Provides methods for sending GET, POST, PUT, and DELETE requests with JSON serialization/deserialization.
/// Handles authorization by including a JWT token from the session if available.
/// </summary>
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

    /// <summary>
    /// Creates and configures an <see cref="HttpClient"/> instance with the necessary headers, including
    /// authorization if a JWT token is present in the session.
    /// </summary>
    /// <returns></returns>
    private HttpClient CreateClient()
    {
        var client = _httpClientFactory.CreateClient("api");

        var token = _httpContextAccessor.HttpContext?.Session.GetString("jwt");
        if (!string.IsNullOrEmpty(token))
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return client;
    }

    /// <summary>   
    /// Reads the response from an HTTP request and deserializes the JSON content.
    /// </summary>
    /// <param name="response">The HTTP response message to read from.</param>
    /// <param name="verb">The HTTP verb used in the request (e.g., GET, POST).</param>
    /// <param name="endpoint">The URI of the resource being accessed.</param>
    /// <returns>A <see cref="Result{T}"/> object containing the deserialized JSON content, or an error message if the operation failed.</returns>
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

    /// <summary>
    /// Sends a GET request to the specified endpoint and reads the response.
    /// </summary>
    /// <param name="endpoint">The URI of the resource to retrieve. This must be a valid, non-null, and non-empty string.</param>
    /// <returns>A <see cref="Result{T}"/> object containing the retrieved resource, or an error message if the operation failed.</returns>
    public async Task<Result<T>> GetAsync<T>(string endpoint)
    {
        var client = CreateClient();
        var response = await client.GetAsync(endpoint);
        return await ReadResponseAsync<T>(response, "GET", endpoint);
    }

    /// <summary>
    /// Sends a POST request with JSON data to the specified endpoint and reads the response.
    /// </summary>
    /// <param name="endpoint">The URI of the resource to create. This must be a valid, non-null, and non-empty string.</param>
    /// <param name="data">The data to create the resource with. This must be a valid, non-null object.</param>
    /// <returns>A <see cref="Result{T}"/> object containing the created resource, or an error message if the operation failed.</returns>
    public async Task<Result<T>> PostAsync<T>(string endpoint, T data)
    {
        var client = CreateClient();
        var response = await client.PostAsJsonAsync(endpoint, data, _jsonOptions);
        return await ReadResponseAsync<T>(response, "POST", endpoint);
    }

    /// <summary>
    /// Sends a PUT request with JSON data to the specified endpoint and reads the response.
    /// </summary>
    /// <param name="endpoint">The URI of the resource to update. This must be a valid, non-null, and non-empty string.</param>
    /// <param name="data">The data to update the resource with. This must be a valid, non-null object.</param>
    /// <returns>A <see cref="Result{T}"/> object containing the updated resource, or an error message if the operation failed.</returns>
    public async Task<Result<T>> PutAsync<T>(string endpoint, T data)
    {
        var client = CreateClient();
        var response = await client.PutAsJsonAsync(endpoint, data, _jsonOptions);
        return await ReadResponseAsync<T>(response, "PUT", endpoint);
    }

    /// <summary>
    /// Sends an asynchronous DELETE request to the specified endpoint and returns the result.
    /// </summary>
    /// <remarks>The method uses an HTTP client to send the DELETE request and processes the response to
    /// determine the outcome. If the operation fails, the returned <see cref="Result{T}"/> will contain an error
    /// message describing the failure.</remarks>
    /// <param name="endpoint">The URI of the resource to delete. This must be a valid, non-null, and non-empty string.</param>
    /// <returns>A <see cref="Result{T}"/> object containing a boolean value indicating whether the operation was successful.
    /// Returns <see langword="true"/> if the DELETE request was successful; otherwise, <see langword="false"/>.</returns>
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
