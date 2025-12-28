using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StaffAtt.Desktop.Helpers;
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
        var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";

        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .Build();

        services.AddSingleton<IConfiguration>(configuration);

        // Register HttpClient with base address from config        
        services.AddHttpClient("api", opts =>
        {
            opts.BaseAddress = new Uri(configuration.GetValue<string>("ApiUrl"));
        });

        services.AddSingleton<IDesktopApiClient, DesktopApiClient>();
        services.AddTransient<MainWindow>();
        services.AddTransient<CheckInForm>();
    }
}

