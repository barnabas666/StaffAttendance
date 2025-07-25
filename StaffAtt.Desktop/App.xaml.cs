﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StaffAttLibrary.Data;
using StaffAttLibrary.Db;
using System.IO;
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
        IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", false)
                .Build();

        services.AddSingleton<IConfiguration>(configuration);

        // Register services for dependency injection according to the DbType specified in appsettings.json
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
        services.AddTransient<CheckInForm>(); // cant be AddSingleton or after attempt to reopen this Window app crash
    }
}

