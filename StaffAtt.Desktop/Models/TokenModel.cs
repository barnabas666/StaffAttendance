using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffAtt.Desktop.Models;

/// <summary>
/// Holds JWT token after successful authentication. 
/// We register it as scoped in App.xaml.cs so we can access it from any component or service.
/// </summary>
public class TokenModel
{
    public string Token { get; set; }
}
