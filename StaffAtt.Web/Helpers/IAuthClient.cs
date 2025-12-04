using StaffAtt.Web.Models;

namespace StaffAtt.Web.Helpers;

public interface IAuthClient
{
    Task<Result<AuthLoginResponse>> LoginAsync(string email, string password);
    Task<Result<bool>> RegisterAsync(string email, string password);
    Task<Result<bool>> ResendConfirmationAsync(string email);
    Task<Result<bool>> ConfirmEmailAsync(string userId, string code);
    Task<Result<bool>> ChangePasswordAsync(string currentPassword, string newPassword);
    Task<Result<bool>> ForgotPasswordAsync(string email);
    Task<Result<bool>> ResetPasswordAsync(string userId, string code, string newPassword);
    Task<Result<bool>> DeleteUserAsync(string email);

}
