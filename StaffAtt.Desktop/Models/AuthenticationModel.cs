using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
