namespace StaffAttLibrary.Data;

public interface IConnectionStringData
{
    string SqlConnectionName { get; set; }
    string SQLiteConnectionName { get; set; }
}