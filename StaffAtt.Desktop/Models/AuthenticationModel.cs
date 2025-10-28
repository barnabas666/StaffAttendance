using System.Text.Json.Serialization;

namespace StaffAtt.Desktop.Models;

/// <summary>
/// Model for user authentication.
/// </summary>
public record AuthenticationModel
{
    [JsonPropertyName("alias")]
    public string Alias { get; set; }
    [JsonPropertyName("pin")]
    public string PIN { get; set; }
}
