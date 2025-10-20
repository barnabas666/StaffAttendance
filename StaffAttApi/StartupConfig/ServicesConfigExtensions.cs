using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using StaffAtt.Identity; // Use the same ApplicationDbContext as your web app
using StaffAttApi.Helpers;
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
        builder.Services.AddResponseCaching();
    }

    public static void AddCustomServices(this WebApplicationBuilder builder)
    {
        // Register services for dependency injection according to the DbType specified in appsettings.json
        string dbType = builder.Configuration.GetValue<string>("DbType") ?? "Postgres";
        if (dbType.Equals("SQLite", StringComparison.OrdinalIgnoreCase))
        {
            builder.Services.AddSingleton<ISqliteDataAccess, SqliteDataAccess>();
            builder.Services.AddSingleton<IStaffService, StaffSqliteService>();
            builder.Services.AddSingleton<ICheckInService, CheckInSqliteService>();
            builder.Services.AddSingleton<IStaffData, StaffSqliteData>();
            builder.Services.AddSingleton<ICheckInData, CheckInSqliteData>();
            builder.Services.AddSingleton<IStaffDataProcessor, StaffSqliteDataProcessor>();
        }
        else if (dbType.Equals("Sql", StringComparison.OrdinalIgnoreCase))
        {
            builder.Services.AddSingleton<ISqlDataAccess, SqlDataAccess>();
            builder.Services.AddSingleton<IStaffService, StaffService>();
            builder.Services.AddSingleton<ICheckInService, CheckInService>();
            builder.Services.AddSingleton<IStaffData, StaffData>();
            builder.Services.AddSingleton<ICheckInData, CheckInData>();
            builder.Services.AddSingleton<IStaffDataProcessor, StaffDataProcessor>();
        }
        else
        {
            builder.Services.AddSingleton<IPostgresDataAccess, PostgresDataAccess>();
            builder.Services.AddSingleton<IStaffService, StaffPostgresService>();
            builder.Services.AddSingleton<ICheckInService, CheckInPostgresService>();
            builder.Services.AddSingleton<IStaffData, StaffPostgresData>();
            builder.Services.AddSingleton<ICheckInData, CheckInPostgresData>();
            builder.Services.AddSingleton<IStaffDataProcessor, StaffPostgresDataProcessor>();
        }

        builder.Services.AddSingleton<IConnectionStringData, ConnectionStringData>();
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
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

    public static void AddIdentityServices(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("IdentityDb")
            ?? throw new InvalidOperationException("Connection string 'IdentityDb' not found.");

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(connectionString)); // Or UseSqlServer, etc.

        builder.Services.AddDefaultIdentity<IdentityUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = true;
            // Lockout settings (optional, match your web app)
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts = 3;
            options.Lockout.AllowedForNewUsers = true;
            options.SignIn.RequireConfirmedPhoneNumber = false;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>();
    }

    public static void AddHealthCheckServices(this WebApplicationBuilder builder)
    {
        var dbType = builder.Configuration.GetValue<string>("DbType") ?? "Postgres";
        var healthChecks = builder.Services.AddHealthChecks();

        if (dbType.Equals("Postgres", StringComparison.OrdinalIgnoreCase))
        {
            healthChecks.AddCheck<PostgresHealthCheck>("Postgres Database");
        }
        else
        {
            healthChecks.AddCheck("self", () => HealthCheckResult.Healthy("API is OK"));
        }

        builder.Services.AddHealthChecksUI(opts =>
        {
            opts.AddHealthCheckEndpoint("API", "/health");
            opts.SetEvaluationTimeInSeconds(10);
            opts.SetMinimumSecondsBetweenFailureNotifications(30);
        }).AddInMemoryStorage();
    }
}
