namespace StaffAttLibrary.Data;
public class ConnectionStringData : IConnectionStringData
{
    /// <summary>
    /// Connection string name - key. In Appsettings.json file in our UI we have connection string with given name.
    /// </summary>
    public string SqlConnectionName { get; set; } = "Testing";
    public string SQLiteConnectionName { get; set; } = "SQLiteDb";
}
