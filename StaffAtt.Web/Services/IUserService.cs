namespace StaffAtt.Web.Services;

public interface IUserService
{
    Task SignInAsync(string token, string email, IEnumerable<string> roles);

    void SignOut();

    string? GetToken();
    string? GetEmail();
    IEnumerable<string> GetRoles();
}
