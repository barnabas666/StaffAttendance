namespace StaffAttLibrary.Db.PostgreSQL;

public interface IPostgresDataAccess
{
    Task<List<T>> LoadDataAsync<T, U>(string sqlStatement, U parameters, string connectionStringName);
    Task<int> SaveDataAsync<T>(string sqlStatement, T parameters, string connectionStringName);
    Task<int> SaveDataGetIdAsync<T>(string sqlStatement, T parameters, string connectionStringName);
}