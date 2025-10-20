using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace StaffAttApi.Helpers;

public class PostgresHealthCheck : IHealthCheck
{
    private readonly string _connectionString;
    public PostgresHealthCheck(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("PostgresDb");
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync(cancellationToken);
            await using var cmd = new NpgsqlCommand("SELECT 1", conn);
            await cmd.ExecuteScalarAsync(cancellationToken);
            return HealthCheckResult.Healthy("Database connection is OK");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database connection failed", ex);
        }
    }
}