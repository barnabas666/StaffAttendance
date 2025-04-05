using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffAttLibrary.Data;
public class ConnectionStringData
{
    /// <summary>
    /// Connection string name - key. In Appsettings.json file in our UI we have connection string with given name.
    /// </summary>
    public string SqlConnectionName { get; set; } = "Testing";
}
