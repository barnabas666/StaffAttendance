using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http; // Add this using directive
using StaffAtt.Desktop.Models;
using StaffAttLibrary.Data;
using StaffAttLibrary.Data.PostgreSQL;
using StaffAttLibrary.Data.SQL;
using StaffAttLibrary.Data.SQLite;
using StaffAttLibrary.Db.PostgreSQL;
using StaffAttLibrary.Db.SQL;
using StaffAttLibrary.Db.SQLite;
using System.IO;
using System.Net.Http; // Optionally add this if not present
using System.Windows;

namespace StaffAtt.Desktop;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static ServiceProvider serviceProvider;

    public App()
    {
        ServiceCollection serviceCollection = new();
        serviceCollection.ConfigureServices();

        serviceProvider = serviceCollection.BuildServiceProvider();

        var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }
}

public static class ServiceCollectionExtensions
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";

        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .Build();

        services.AddSingleton<IConfiguration>(configuration);
        // Register TokenModel as scoped, so we have one instance per user session (circuit). 
        services.AddScoped<TokenModel>();

        // Register HttpClient with base address from config        
        services.AddHttpClient("api", opts =>
        {
            opts.BaseAddress = new Uri(configuration.GetValue<string>("ApiUrl"));
        });

        // Register services for dependency injection according to the DbType specified in config
        string dbType = configuration["DbType"] ?? "SQLite";
        if (dbType.Equals("SQLite", StringComparison.OrdinalIgnoreCase))
        {
            services.AddSingleton<ISqliteDataAccess, SqliteDataAccess>();
            services.AddSingleton<IStaffService, StaffSqliteService>();
            services.AddSingleton<ICheckInService, CheckInSqliteService>();
            services.AddSingleton<IStaffData, StaffSqliteData>();
            services.AddSingleton<ICheckInData, CheckInSqliteData>();
            services.AddSingleton<IStaffDataProcessor, StaffSqliteDataProcessor>();
        }
        else if (dbType.Equals("Postgres", StringComparison.OrdinalIgnoreCase))
        {
            services.AddSingleton<IPostgresDataAccess, PostgresDataAccess>();
            services.AddSingleton<IStaffService, StaffPostgresService>();
            services.AddSingleton<ICheckInService, CheckInPostgresService>();
            services.AddSingleton<IStaffData, StaffPostgresData>();
            services.AddSingleton<ICheckInData, CheckInPostgresData>();
            services.AddSingleton<IStaffDataProcessor, StaffPostgresDataProcessor>();
        }
        else
        {
            services.AddSingleton<ISqlDataAccess, SqlDataAccess>();
            services.AddSingleton<IStaffService, StaffService>();
            services.AddSingleton<ICheckInService, CheckInService>();
            services.AddSingleton<IStaffData, StaffData>();
            services.AddSingleton<ICheckInData, CheckInData>();
            services.AddSingleton<IStaffDataProcessor, StaffDataProcessor>();
        }

        services.AddSingleton<IConnectionStringData, ConnectionStringData>();
        services.AddTransient<MainWindow>();
        services.AddTransient<CheckInForm>();

    }
}

