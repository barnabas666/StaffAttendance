
namespace StaffAttLibrary.Db;

/// <summary>
/// Interface for class servicing SQL database connection
/// </summary>
public interface ISqlDataAccess
{
    Task<List<T>> LoadData<T, U>(string storedProcedure, U parameters, string connectionStringName);
    Task<int> SaveData<T>(string storedProcedure, T parameters, string connectionStringName);
    Task<int> SaveDataGetId<T>(string storedProcedure, T parameters, string connectionStringName);
}