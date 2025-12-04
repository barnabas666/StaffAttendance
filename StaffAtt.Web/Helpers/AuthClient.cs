using StaffAtt.Web.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace StaffAtt.Web.Helpers;

/// <summary>
/// Client for authentication-related API calls.
/// </summary>
public class AuthClient : IAuthClient
{
    private readonly IHttpClientFactory _factory;
    private readonly IUserService _userService;

    public AuthClient(IHttpClientFactory factory, IUserService userService)
    {
        _factory = factory;
        _userService = userService;
    }

    private HttpClient Client => _factory.CreateClient("api");

    public async Task<Result<AuthLoginResponse>> LoginAsync(string email, string password)
    {
        var payload = new { email, password };
        var res = await Client.PostAsJsonAsync("auth/login", payload);

        if (!res.IsSuccessStatusCode)
            return Result<AuthLoginResponse>.Failure(await res.Content.ReadAsStringAsync());

        var dto = await res.Content.ReadFromJsonAsync<AuthLoginResponse>();
        if (dto == null)
            return Result<AuthLoginResponse>.Failure("Invalid login response");

        return Result<AuthLoginResponse>.Success(dto);
    }

    public async Task<Result<bool>> RegisterAsync(string email, string password)
    {
        var payload = new { email, password };
        var res = await Client.PostAsJsonAsync("auth/register", payload);

        if (!res.IsSuccessStatusCode)
            return Result<bool>.Failure(await res.Content.ReadAsStringAsync());

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> ResendConfirmationAsync(string email)
    {
        var res = await Client.PostAsJsonAsync("auth/resend-confirmation", new { email });

        if (!res.IsSuccessStatusCode)
            return Result<bool>.Failure(await res.Content.ReadAsStringAsync());

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> ConfirmEmailAsync(string userId, string code)
    {
        var url =
            $"auth/confirm-email?userId={Uri.EscapeDataString(userId)}&code={Uri.EscapeDataString(code)}";

        var res = await Client.GetAsync(url);

        if (!res.IsSuccessStatusCode)
            return Result<bool>.Failure(await res.Content.ReadAsStringAsync());

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> ChangePasswordAsync(string currentPassword, string newPassword)
    {
        var client = Client;

        // attach token from session (IUserService)
        var token = _userService.GetToken();
        if (!string.IsNullOrEmpty(token))
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var payload = new { currentPassword, newPassword };
        var res = await client.PostAsJsonAsync("auth/change-password", payload);

        if (!res.IsSuccessStatusCode)
            return Result<bool>.Failure(await res.Content.ReadAsStringAsync());

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> ForgotPasswordAsync(string email)
    {
        var payload = new { email };
        var res = await Client.PostAsJsonAsync("auth/forgot-password", payload);

        return res.IsSuccessStatusCode
            ? Result<bool>.Success(true)
            : Result<bool>.Failure(await res.Content.ReadAsStringAsync());
    }

    public async Task<Result<bool>> ResetPasswordAsync(string userId, string code, string newPassword)
    {
        var payload = new { userId, code, newPassword };
        var res = await Client.PostAsJsonAsync("auth/reset-password", payload);

        return res.IsSuccessStatusCode
            ? Result<bool>.Success(true)
            : Result<bool>.Failure(await res.Content.ReadAsStringAsync());
    }

    public async Task<Result<bool>> DeleteUserAsync(string email)
    {
        var client = Client;

        var token = _userService.GetToken();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var res = await client.DeleteAsync($"auth/user/{Uri.EscapeDataString(email)}");

        if (!res.IsSuccessStatusCode)
            return Result<bool>.Failure(await res.Content.ReadAsStringAsync());

        return Result<bool>.Success(true);
    }
}
