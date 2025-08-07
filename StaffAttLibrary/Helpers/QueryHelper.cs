namespace StaffAttLibrary.Helpers;
public static class QueryHelper
{
    /// <summary>
    /// Loads the content of a SQLite query file from the Queries/SQLite folder.
    /// </summary>
    /// <param name="queryFileName">The name of the .sql file (e.g., "Aliases_GetByAliasAndPIN.sql").</param>
    /// <returns>The SQL query as a string.</returns>
    public static async Task<string> LoadSqliteQueryAsync(string queryFileName)
    {
        return await LoadQueryAsync("SQLite", queryFileName);
    }

    /// <summary>
    /// Loads the content of a PostgreSQL query file from the Queries/PostgreSQL folder.
    /// </summary>
    /// <param name="queryFileName">The name of the .sql file (e.g., "Aliases_GetByAliasAndPIN.sql").</param>
    /// <returns>The SQL query as a string.</returns>
    public static async Task<string> LoadPostgresQueryAsync(string queryFileName)
    {
        return await LoadQueryAsync("PostgreSQL", queryFileName);
    }

    /// <summary>
    /// Generic method to load query files from different database type folders.
    /// </summary>
    /// <param name="dbType">Database type folder name (e.g., "SQLite", "PostgreSQL").</param>
    /// <param name="queryFileName">The name of the .sql file.</param>
    /// <returns>The SQL query as a string.</returns>
    private static async Task<string> LoadQueryAsync(string dbType, string queryFileName)
    {
        string libraryDir = Path.GetDirectoryName(typeof(QueryHelper).Assembly.Location)!;
        string sqlPath = Path.Combine(libraryDir, "Queries", dbType, queryFileName);
        return await File.ReadAllTextAsync(sqlPath);
    }
}
