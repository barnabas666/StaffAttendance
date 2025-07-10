using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffAttLibrary.Db;
public class SqliteDataAccess : ISqliteDataAccess
{
    private readonly IConfiguration _config;

    public SqliteDataAccess(IConfiguration config)
    {
        _config = config;
    }

    /// <summary>
    /// Executes a SQL query asynchronously and retrieves the results as a list of objects of the specified type.
    /// </summary>
    /// <remarks>This method uses Dapper to execute the query and map the results. Ensure that the type
    /// <typeparamref name="T"/> has a structure that matches the columns returned by the query for proper
    /// mapping.</remarks>
    /// <typeparam name="T">The type of objects to map the query results to.</typeparam>
    /// <typeparam name="U">The type of the parameters object used in the query.</typeparam>
    /// <param name="sqlStatement">The SQL query to execute.</param>
    /// <param name="parameters">An object containing the parameters for the SQL query. Can be null if the query does not require parameters.</param>
    /// <param name="connectionStringName">The name of the connection string to use, as defined in the configuration.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of objects of type
    /// <typeparamref name="T"/> representing the rows returned by the query. If no rows are returned, the list will be
    /// empty.</returns>
    public async Task<List<T>> LoadDataAsync<T, U>(string sqlStatement, U parameters, string connectionStringName)
    {
        string connectionString = _config.GetConnectionString(connectionStringName);

        using (IDbConnection connection = new SQLiteConnection(connectionString))
        {
            var rows = await connection.QueryAsync<T>(sqlStatement, parameters);
            return rows.ToList();
        }
    }

    /// <summary>
    /// Executes a SQL statement asynchronously using the specified parameters and connection string.
    /// </summary>
    /// <remarks>This method uses the connection string specified by <paramref name="connectionStringName"/>
    /// to establish a database connection and execute the provided SQL statement. The method is asynchronous and
    /// returns a task that completes when the operation is finished.</remarks>
    /// <typeparam name="T">The type of the parameters object to be passed to the SQL statement.</typeparam>
    /// <param name="sqlStatement">The SQL statement to execute. Must be a valid SQL command.</param>
    /// <param name="parameters">An object containing the parameters to be used in the SQL statement. Can be null if no parameters are required.</param>
    /// <param name="connectionStringName">The name of the connection string to use, as defined in the configuration.</param>
    /// <returns></returns>
    public async Task<int> SaveDataAsync<T>(string sqlStatement, T parameters, string connectionStringName)
    {
        string connectionString = _config.GetConnectionString(connectionStringName);

        using (IDbConnection connection = new SQLiteConnection(connectionString))
        {
            return await connection.ExecuteAsync(sqlStatement, parameters);
        }
    }

    /// <summary>
    /// Save data to SQLite database and return Id of last inserted object.
    /// The SQL statement should end with: "; SELECT last_insert_rowid();"
    /// </summary>
    /// <typeparam name="T">Type of parameters for the SQL statement.</typeparam>
    /// <param name="sqlStatement">SQL statement for insert, should end with "SELECT last_insert_rowid();".</param>
    /// <param name="parameters">Parameters for the SQL statement.</param>
    /// <param name="connectionStringName">Connection string name from configuration.</param>
    /// <returns>Id of the last inserted row.</returns>
    public async Task<int> SaveDataGetIdAsync<T>(string sqlStatement, T parameters, string connectionStringName)
    {
        string connectionString = _config.GetConnectionString(connectionStringName);

        using (IDbConnection connection = new SQLiteConnection(connectionString))
        {
            return await connection.ExecuteScalarAsync<int>(sqlStatement, parameters);
        }
    }
}
