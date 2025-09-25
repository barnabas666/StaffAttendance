using System.Text.Json.Serialization;

namespace StaffAtt.Web.Models;

/// <summary>
/// Model for user authentication.
/// </summary>
public record AuthenticationModel
{    
    [JsonPropertyName("email")]
    public string Email { get; set; }
    [JsonPropertyName("password")]
    public string Password { get; set; }
}
