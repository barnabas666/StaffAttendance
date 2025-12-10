using StaffAtt.Web.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace StaffAtt.Web.Helpers;

/// <summary>
/// Implementation of <see cref="IAuthClient"/> for handling authentication-related HTTP requests.
/// Provides methods for user login, registration, email confirmation, password management, and user deletion.
/// Utilizes an <see cref="IHttpClientFactory"/> to create HTTP clients and an <see cref="IUserService"/> to manage user session data.
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

    /// <summary>
    /// Logs in a user with the provided email and password. 
    /// Returns an authentication response containing a token and user details upon success.
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Registers a new user with the provided email and password.
    /// Returns a success result upon successful registration.
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public async Task<Result<bool>> RegisterAsync(string email, string password)
    {
        var payload = new { email, password };
        var res = await Client.PostAsJsonAsync("auth/register", payload);

        if (!res.IsSuccessStatusCode)
            return Result<bool>.Failure(await res.Content.ReadAsStringAsync());

        return Result<bool>.Success(true);
    }

    /// <summary>
    /// Resends the email confirmation to the specified email address.
    /// Returns a success result upon successful request.
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public async Task<Result<bool>> ResendConfirmationAsync(string email)
    {
        var res = await Client.PostAsJsonAsync("auth/resend-confirmation", new { email });

        if (!res.IsSuccessStatusCode)
            return Result<bool>.Failure(await res.Content.ReadAsStringAsync());

        return Result<bool>.Success(true);
    }

    /// <summary>
    /// Confirms the user's email using the provided user ID and confirmation code.
    /// Returns a success result upon successful confirmation.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    public async Task<Result<bool>> ConfirmEmailAsync(string userId, string code)
    {
        var url =
            $"auth/confirm-email?userId={Uri.EscapeDataString(userId)}&code={Uri.EscapeDataString(code)}";

        var res = await Client.GetAsync(url);

        if (!res.IsSuccessStatusCode)
            return Result<bool>.Failure(await res.Content.ReadAsStringAsync());

        return Result<bool>.Success(true);
    }

    /// <summary>
    /// Changes the password for the currently authenticated user.
    /// Returns a success result upon successful password change.
    /// </summary>
    /// <param name="currentPassword"></param>
    /// <param name="newPassword"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Initiates the forgot password process for the specified email address.
    /// Returns a success result upon successful request.
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public async Task<Result<bool>> ForgotPasswordAsync(string email)
    {
        var payload = new { email };
        var res = await Client.PostAsJsonAsync("auth/forgot-password", payload);

        return res.IsSuccessStatusCode
            ? Result<bool>.Success(true)
            : Result<bool>.Failure(await res.Content.ReadAsStringAsync());
    }

    /// <summary>
    /// Resets the password for a user using the provided user ID, reset code, and new password.
    /// Returns a success result upon successful password reset.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="code"></param>
    /// <param name="newPassword"></param>
    /// <returns></returns>
    public async Task<Result<bool>> ResetPasswordAsync(string userId, string code, string newPassword)
    {
        var payload = new { userId, code, newPassword };
        var res = await Client.PostAsJsonAsync("auth/reset-password", payload);

        return res.IsSuccessStatusCode
            ? Result<bool>.Success(true)
            : Result<bool>.Failure(await res.Content.ReadAsStringAsync());
    }

    /// <summary>
    /// Deletes the user account associated with the specified email address.
    /// Returns a success result upon successful deletion.
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
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
