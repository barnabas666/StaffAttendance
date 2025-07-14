using System;
using System.IO;
using System.Threading.Tasks;

namespace StaffAttLibrary.Helpers;
public static class SqliteQueryHelper
{
    /// <summary>
    /// Loads the content of a SQLite query file from the SqliteQueries folder in StaffAttLibrary.
    /// </summary>
    /// <param name="queryFileName">The name of the .sql file (e.g., "Aliases_GetByAliasAndPIN.sql").</param>
    /// <returns>The SQL query as a string.</returns>
    public static async Task<string> LoadQueryAsync(string queryFileName)
    {
        // Get the directory of the StaffAttLibrary assembly
        string libraryDir = Path.GetDirectoryName(typeof(SqliteQueryHelper).Assembly.Location)!;
        string sqlPath = Path.Combine(libraryDir, "SqliteQueries", queryFileName);
        return await File.ReadAllTextAsync(sqlPath);
    }
}
