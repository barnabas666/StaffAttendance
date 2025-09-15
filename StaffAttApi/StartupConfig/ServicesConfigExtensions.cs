using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using StaffAttLibrary.Data;
using StaffAttLibrary.Data.PostgreSQL;
using StaffAttLibrary.Data.SQL;
using StaffAttLibrary.Data.SQLite;
using StaffAttLibrary.Db.PostgreSQL;
using StaffAttLibrary.Db.SQL;
using StaffAttLibrary.Db.SQLite;
using System.Text;

namespace StaffAttApi.StartupConfig;

public static class ServicesConfigExtensions
{
    public static void AddStandardServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
    }

    public static void AddCustomServices(this WebApplicationBuilder builder)
    {
        // Register services for dependency injection according to the DbType specified in appsettings.json
        string dbType = builder.Configuration.GetValue<string>("DbType") ?? "SQLite";
        if (dbType.Equals("SQLite", StringComparison.OrdinalIgnoreCase))
        {
            builder.Services.AddSingleton<ISqliteDataAccess, SqliteDataAccess>();
            builder.Services.AddSingleton<IStaffService, StaffSqliteService>();
            builder.Services.AddSingleton<ICheckInService, CheckInSqliteService>();
            builder.Services.AddSingleton<IStaffData, StaffSqliteData>();
            builder.Services.AddSingleton<ICheckInData, CheckInSqliteData>();
            builder.Services.AddSingleton<IStaffDataProcessor, StaffSqliteDataProcessor>();
        }
        else if (dbType.Equals("Postgres", StringComparison.OrdinalIgnoreCase))
        {
            builder.Services.AddSingleton<IPostgresDataAccess, PostgresDataAccess>();
            builder.Services.AddSingleton<IStaffService, StaffPostgresService>();
            builder.Services.AddSingleton<ICheckInService, CheckInPostgresService>();
            builder.Services.AddSingleton<IStaffData, StaffPostgresData>();
            builder.Services.AddSingleton<ICheckInData, CheckInPostgresData>();
            builder.Services.AddSingleton<IStaffDataProcessor, StaffPostgresDataProcessor>();
        }
        else
        {
            builder.Services.AddSingleton<ISqlDataAccess, SqlDataAccess>();
            builder.Services.AddSingleton<IStaffService, StaffService>();
            builder.Services.AddSingleton<ICheckInService, CheckInService>();
            builder.Services.AddSingleton<IStaffData, StaffData>();
            builder.Services.AddSingleton<ICheckInData, CheckInData>();
            builder.Services.AddSingleton<IStaffDataProcessor, StaffDataProcessor>();
        }

        builder.Services.AddSingleton<IConnectionStringData, ConnectionStringData>();
    }

    public static void AddAuthServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorization(opts =>
        {
            opts.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        builder.Services.AddAuthentication("Bearer")
            .AddJwtBearer(opts =>
            {
                opts.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration.GetValue<string>("Authentication:Issuer"),
                    ValidAudience = builder.Configuration.GetValue<string>("Authentication:Audience"),
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(
                        builder.Configuration.GetValue<string>("Authentication:SecretKey")))
                };
            });
    }
}
