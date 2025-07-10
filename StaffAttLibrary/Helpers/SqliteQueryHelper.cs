using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffAttLibrary.Helpers;
public static class SqliteQueryHelper
{
    /// <summary>
    /// Loads the content of a SQLite query file from the SqliteQueries folder.
    /// </summary>
    /// <param name="queryFileName">The name of the .sql file (e.g., "Aliases_GetByAliasAndPIN.sql").</param>
    /// <returns>The SQL query as a string.</returns>
    public static async Task<string> LoadQueryAsync(string queryFileName)
    {
        string sqlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SqliteQueries", queryFileName);
        return await File.ReadAllTextAsync(sqlPath);
    }
}
