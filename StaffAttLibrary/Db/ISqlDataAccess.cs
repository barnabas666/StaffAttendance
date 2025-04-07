
namespace StaffAttLibrary.Db;

/// <summary>
/// Interface for class servicing SQL database connection
/// </summary>
public interface ISqlDataAccess
{
    Task<List<T>> LoadDataAsync<T, U>(string storedProcedure, U parameters, string connectionStringName);
    Task<int> SaveDataAsync<T>(string storedProcedure, T parameters, string connectionStringName);
    Task<int> SaveDataGetIdAsync<T>(string storedProcedure, T parameters, string connectionStringName);
}
