using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffAttLibrary.Db;
public class SqlDataAccess : ISqlDataAccess
{
    private readonly IConfiguration _config;

    public SqlDataAccess(IConfiguration config)
    {
        _config = config;
    }

    public async Task<List<T>> LoadData<T, U>(string storedProcedure, U parameters, string connectionStringName)
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

    // returns number of inserted/affected rows
    public async Task<int> SaveData<T>(string storedProcedure, T parameters, string connectionStringName)
    {
        string connectionString = _config.GetConnectionString(connectionStringName);

        using (IDbConnection connection = new SqlConnection(connectionString))
        {
            return await connection.ExecuteAsync(storedProcedure,
                                                 parameters,
                                                 commandType: CommandType.StoredProcedure);
        }
    }

    // returns Id of last inserted object, query/SP should end with: SELECT SCOPE_IDENTITY();
    public async Task<int> SaveDataGetId<T>(string storedProcedure, T parameters, string connectionStringName)
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
