using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffAttLibrary.Db.PostgreSQL;

public class PostgresDataAccess : IPostgresDataAccess
{
    private readonly IConfiguration _config;

    public PostgresDataAccess(IConfiguration config)
    {
        _config = config;
    }

    /// <summary>
    /// Executes the specified SQL query asynchronously and retrieves the result as a list of objects of type
    /// <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>This method uses Dapper to execute the query and map the results to objects of type
    /// <typeparamref name="T"/>. Ensure that the SQL query and parameter object are properly constructed to match the
    /// expected result type.</remarks>
    /// <typeparam name="T">The type of objects to map the query results to.</typeparam>
    /// <typeparam name="U">The type of the parameter object used in the query.</typeparam>
    /// <param name="sqlStatement">The SQL query to execute.</param>
    /// <param name="parameters">An object containing the parameters for the SQL query. Can be an anonymous object or a strongly-typed object.</param>
    /// <param name="connectionStringName">The name of the connection string to use, as defined in the configuration.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of objects of type
    /// <typeparamref name="T"/> representing the query results.</returns>
    public async Task<List<T>> LoadDataAsync<T, U>(string sqlStatement, U parameters, string connectionStringName)
    {
        string connectionString = _config.GetConnectionString(connectionStringName);
        using (IDbConnection connection = new NpgsqlConnection(connectionString))
        {
            var rows = await connection.QueryAsync<T>(sqlStatement, parameters);
            return rows.ToList();
        }
    }

    /// <summary>
    /// Executes a SQL statement asynchronously and returns the number of rows affected.
    /// </summary>
    /// <remarks>This method uses the connection string specified by <paramref name="connectionStringName"/>
    /// to establish a database connection. Ensure that the connection string is properly configured in the
    /// application's configuration file.</remarks>
    /// <typeparam name="T">The type of the parameters object used in the SQL statement.</typeparam>
    /// <param name="sqlStatement">The SQL statement to execute. Cannot be null or empty.</param>
    /// <param name="parameters">An object containing the parameters to be passed to the SQL statement. Can be null if the statement does not
    /// require parameters.</param>
    /// <param name="connectionStringName">The name of the connection string to use from the configuration. Cannot be null or empty.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of rows affected by the
    /// SQL statement.</returns>
    public async Task<int> SaveDataAsync<T>(string sqlStatement, T parameters, string connectionStringName)
    {
        string connectionString = _config.GetConnectionString(connectionStringName);
        using (IDbConnection connection = new NpgsqlConnection(connectionString))
        {
            return await connection.ExecuteAsync(sqlStatement, parameters);
        }
    }

    /// <summary>
    /// Executes the specified SQL statement asynchronously and returns the resulting scalar value as an integer.
    /// </summary>
    /// <remarks>This method uses the connection string specified by <paramref name="connectionStringName"/>
    /// to establish a database connection. Ensure that the SQL statement provided is valid and that the parameters
    /// match the expected placeholders in the query.</remarks>
    /// <typeparam name="T">The type of the parameters object used in the SQL statement.</typeparam>
    /// <param name="sqlStatement">The SQL statement to execute. Must be a valid SQL query that returns a scalar value.</param>
    /// <param name="parameters">An object containing the parameters to be passed to the SQL statement. Can be an anonymous object or a
    /// strongly-typed object.</param>
    /// <param name="connectionStringName">The name of the connection string to use, as defined in the application's configuration.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the scalar value  returned by the
    /// SQL statement, cast to an integer.</returns>
    public async Task<int> SaveDataGetIdAsync<T>(string sqlStatement, T parameters, string connectionStringName)
    {
        string connectionString = _config.GetConnectionString(connectionStringName);
        using (IDbConnection connection = new NpgsqlConnection(connectionString))
        {
            return await connection.ExecuteScalarAsync<int>(sqlStatement, parameters);
        }
    }
}
