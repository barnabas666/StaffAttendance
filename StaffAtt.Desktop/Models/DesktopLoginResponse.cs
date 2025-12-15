using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffAtt.Desktop.Models;

/// <summary>
/// Represents the response returned after a successful desktop login, containing authentication and user alias
/// information.
/// </summary>
public class DesktopLoginResponse
{
    public string Token { get; set; } = "";
    public int AliasId { get; set; }
}
