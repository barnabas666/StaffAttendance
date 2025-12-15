namespace StaffAttApi.Models;

public class AuthLoginResponse
{
    public string Token { get; set; } = "";
    public string Email { get; set; } = "";
    public List<string> Roles { get; set; } = new();
}
