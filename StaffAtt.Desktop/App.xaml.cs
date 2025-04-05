using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StaffAttLibrary.Data;
using StaffAttLibrary.Db;
using System.Configuration;
using System.Data;
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

        services.AddSingleton(new ConnectionStringData
        {
            SqlConnectionName = "Testing"
        });

        services.AddSingleton<ISqlDataAccess, SqlDataAccess>();
        services.AddSingleton<IStaffService, StaffService>();
        services.AddSingleton<ICheckInService, CheckInService>();
        services.AddSingleton<IStaffData, StaffData>();
        services.AddSingleton<ICheckInData, CheckInData>();
        services.AddTransient<MainWindow>();
        services.AddTransient<CheckInForm>(); // cant be AddSingleton or after attempt to reopen this Window app crash
    }
}

