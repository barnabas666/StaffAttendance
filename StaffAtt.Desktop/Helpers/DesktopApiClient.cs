using StaffAtt.Desktop.Models;
using StaffAttShared.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Json;

namespace StaffAtt.Desktop.Helpers;
public class DesktopApiClient : IDesktopApiClient
{
    private readonly HttpClient _client;
    
    public DesktopApiClient(IHttpClientFactory factory)
    {
        _client = factory.CreateClient("api");
    }

    public async Task<DesktopResult<DesktopLoginResponse>> LoginAsync(string alias, string pin)
    {
        try
        {
            var body = new { Alias = alias, PIN = pin };
            var resp = await _client.PostAsJsonAsync("AuthenticationDesktop/token", body);

            if (!resp.IsSuccessStatusCode)
            {
                string msg = resp.StatusCode == System.Net.HttpStatusCode.Unauthorized
                    ? "Invalid alias or PIN."
                    : "Login failed. Please try again.";

                return DesktopResult<DesktopLoginResponse>.Fail(msg);
            }

            // Read token
            string tokenStr = await resp.Content.ReadAsStringAsync();            

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenStr);

            // Decode JWT for aliasId
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(tokenStr);
            int aliasId = int.Parse(jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);

            var loginResponse = new DesktopLoginResponse
            {
                Token = tokenStr,
                AliasId = aliasId
            };

            return DesktopResult<DesktopLoginResponse>.Success(loginResponse);
        }
        catch
        {
            return DesktopResult<DesktopLoginResponse>.Fail("Network error. Please try again.");
        }
    }

    public async Task<DesktopResult<StaffBasicDto>> GetStaffByAliasIdAsync(int aliasId)
    {
        return await GetResultAsync<StaffBasicDto>(
            $"staff/basic/alias/{aliasId}",
            "Unable to load staff information.");
    }

    public async Task<DesktopResult<CheckInDto?>> GetLastCheckInAsync(int staffId)
    {
        try
        {
            var resp = await _client.GetAsync($"checkin/last/{staffId}");
            if (!resp.IsSuccessStatusCode)
            {
                // For UI, we don't want to show error, so just fail silently at call site
                return DesktopResult<CheckInDto?>.Fail("Unable to load last check-in.");
            }

            // The API might return null or empty body if no last check-in exists.
            var value = await resp.Content.ReadFromJsonAsync<CheckInDto>();
            // value can be null => "no last record", which is fine
            return DesktopResult<CheckInDto?>.Success(value);
        }
        catch
        {
            return DesktopResult<CheckInDto?>.Fail("Network error while loading last check-in.");
        }
    }

    public async Task<DesktopResult<bool>> DoCheckInAsync(int staffId)
    {
        return await PostResultAsync($"checkin/do/{staffId}", "Unable to perform Check-In/Out.");
    }

    private async Task<DesktopResult<T>> GetResultAsync<T>(string url, string errorMessage)
    {
        try
        {
            var resp = await _client.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return DesktopResult<T>.Fail(errorMessage);

            var value = await resp.Content.ReadFromJsonAsync<T>();
            if (value == null)
                return DesktopResult<T>.Fail("Malformed response.");

            return DesktopResult<T>.Success(value);
        }
        catch
        {
            return DesktopResult<T>.Fail("Network error.");
        }
    }

    private async Task<DesktopResult<bool>> PostResultAsync(string url, string errorMessage)
    {
        try
        {
            var resp = await _client.PostAsync(url, null);
            if (!resp.IsSuccessStatusCode)
                return DesktopResult<bool>.Fail(errorMessage);

            return DesktopResult<bool>.Success(true);
        }
        catch
        {
            return DesktopResult<bool>.Fail("Network error.");
        }
    }
}
