namespace StaffAtt.Web.Models;

/// <summary>
/// Holds JWT token after successful authentication. 
/// We register it as scoped in Program.cs so we can access it from any component or service.
/// </summary>
public class TokenModel
{
    public string Token { get; set; }
}
