using StaffAtt.Web.Models;

namespace StaffAtt.Web.Helpers;

public class FakeAuthClient : IAuthClient
{
    public Task<Result<AuthLoginResponse>> LoginAsync(string email, string password)
    {
        if (email == "test@test.com" && password == "Pwd.1234")
        {
            return Task.FromResult(Result<AuthLoginResponse>.Success(
                new AuthLoginResponse
                {                    
                    Email = email,
                    Token = "fake-jwt-token-12345",
                    Roles = new List<string> { "Member" }
                }
            ));
        }

        return Task.FromResult(Result<AuthLoginResponse>.Failure("Invalid credentials"));
    }

    public Task<Result<bool>> RegisterAsync(string email, string password)
        => Task.FromResult(Result<bool>.Success(true));

    public Task<Result<bool>> ResendConfirmationAsync(string email)
        => Task.FromResult(Result<bool>.Success(true));

    public Task<Result<bool>> ConfirmEmailAsync(string userId, string code)
        => Task.FromResult(Result<bool>.Success(true));

    public Task<Result<bool>> ChangePasswordAsync(string currentPassword, string newPassword)
        => Task.FromResult(Result<bool>.Success(true));

    public Task<Result<bool>> ForgotPasswordAsync(string email)
        => Task.FromResult(Result<bool>.Success(true));

    public Task<Result<bool>> ResetPasswordAsync(string userId, string code, string newPassword)
        => Task.FromResult(Result<bool>.Success(true));

    public Task<Result<bool>> DeleteUserAsync(string email)
        => Task.FromResult(Result<bool>.Success(true));
}
