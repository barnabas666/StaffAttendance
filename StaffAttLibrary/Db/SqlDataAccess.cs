using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace StaffAttLibrary.Db;

/// <summary>
/// Class servicing SQL database connection, using Dapper.
/// </summary>
public class SqlDataAccess : ISqlDataAccess
{
    /// <summary>
    /// IConfiguration interface is used to read Settings and Connection Strings from appsettings.json file.    
    /// </summary>
    private readonly IConfiguration _config;

    /// <summary>
    /// Constructor, IConfiguration comes from Dependency Injection from our frontend (UI).
    /// </summary>
    /// <param name="config">Is used to read Settings and Connection Strings from appsettings.json file.</param>
    public SqlDataAccess(IConfiguration config)
    {
        _config = config;
    }

    /// <summary>
    /// Connect to SQL database and load data without blocking UI thread.
    /// </summary>
    /// <typeparam name="T">Generic parameter - Model we get from Db.</typeparam>
    /// <typeparam name="U">Generic parameter - dynamic parameters inserted into Stored Procedure.</typeparam>
    /// <param name="storedProcedure">Stored Procedure to execute SQL query.</param>
    /// <param name="parameters">Parameters inserted into Stored Procedure.</param>
    /// <param name="connectionStringName">Connection String Name from appsettings.json.</param>
    /// <returns>Collection of T representing some Model.</returns>
    public async Task<List<T>> LoadDataAsync<T, U>(string storedProcedure, U parameters, string connectionStringName)
    {
        string connectionString = _config.GetConnectionString(connectionStringName);

        using (IDbConnection connection = new SqlConnection(connectionString))
        {
            var rows = await connection.QueryAsync<T>(storedProcedure,
                                                      parameters,
                                                      commandType: CommandType.StoredProcedure);
            return rows.ToList();
        }
    }

    /// <summary>
    /// Connect to SQL database and save data without blocking UI thread. 
    /// </summary>
    /// <typeparam name="T">Generic parameter - dynamic parameters inserted into Stored Procedure.</typeparam>
    /// <param name="storedProcedure">Stored Procedure to execute SQL query.</param>
    /// <param name="parameters">Parameters inserted into Stored Procedure.</param>
    /// <param name="connectionStringName">Connection String Name from appsettings.json.</param>
    /// <returns>Returns number of inserted/affected rows</returns>
    public async Task<int> SaveDataAsync<T>(string storedProcedure, T parameters, string connectionStringName)
    {
        string connectionString = _config.GetConnectionString(connectionStringName);

        using (IDbConnection connection = new SqlConnection(connectionString))
        {
            return await connection.ExecuteAsync(storedProcedure,
                                                 parameters,
                                                 commandType: CommandType.StoredProcedure);
        }
    }

    /// <summary>
    /// Connect to SQL database and save data without blocking UI thread.
    /// </summary>
    /// <typeparam name="T">Generic parameter - dynamic parameters inserted into Stored Procedure.</typeparam>
    /// <param name="storedProcedure">Stored Procedure to execute SQL query.</param>
    /// <param name="parameters">Parameters inserted into Stored Procedure.</param>
    /// <param name="connectionStringName">Connection String Name from appsettings.json.</param>
    /// <returns>Returns Id of last inserted object, query/SP should end with: SELECT SCOPE_IDENTITY();</returns>
    public async Task<int> SaveDataGetIdAsync<T>(string storedProcedure, T parameters, string connectionStringName)
    {
        string connectionString = _config.GetConnectionString(connectionStringName);

        using (IDbConnection connection = new SqlConnection(connectionString))
        {
            return await connection.ExecuteScalarAsync<int>(storedProcedure,
                                                            parameters,
                                                            commandType: CommandType.StoredProcedure);
        }
    }
}
