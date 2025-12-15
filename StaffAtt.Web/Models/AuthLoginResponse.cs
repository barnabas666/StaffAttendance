namespace StaffAtt.Web.Models;

/// <summary>
/// Represents the response returned after a successful authentication login, containing the access token, user email,
/// and assigned roles.
/// </summary>
public class AuthLoginResponse
{
    public string Token { get; set; } = "";
    public string Email { get; set; } = "";
    public List<string> Roles { get; set; } = new();
}
