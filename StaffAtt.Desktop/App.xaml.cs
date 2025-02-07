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

        services.AddSingleton<ISqlDataAccess, SqlDataAccess>();
        services.AddSingleton<IDatabaseData, SqlData>();
        services.AddSingleton<MainWindow>();
        services.AddSingleton<CheckInForm>();
    }
}

