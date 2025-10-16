using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using StaffAtt.Identity;
using StaffAtt.Web.Helpers;
using StaffAtt.Web.Models;
using StaffAttLibrary.Data;
using StaffAttLibrary.Data.PostgreSQL;
using StaffAttLibrary.Data.SQL;
using StaffAttLibrary.Data.SQLite;
using StaffAttLibrary.Db.PostgreSQL;
using StaffAttLibrary.Db.SQL;
using StaffAttLibrary.Db.SQLite;

namespace StaffAtt.Web.StartupConfig;

public static class ServicesConfigExtensions
{
    public static void AddCustomServices(this WebApplicationBuilder builder)
    {
        // Register HttpClient with base address from config        
        builder.Services.AddHttpClient("api", opts =>
        {
            opts.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ApiUrl"));
        });

        // Register services for dependency injection according to the DbType specified in appsettings.json
        string dbType = builder.Configuration.GetValue<string>("DbType") ?? "SQLite";
        if (dbType.Equals("SQLite", StringComparison.OrdinalIgnoreCase))
        {
            builder.Services.AddTransient<ISqliteDataAccess, SqliteDataAccess>();
            builder.Services.AddTransient<IStaffService, StaffSqliteService>();
            builder.Services.AddTransient<ICheckInService, CheckInSqliteService>();
            builder.Services.AddTransient<IStaffData, StaffSqliteData>();
            builder.Services.AddTransient<ICheckInData, CheckInSqliteData>();
            builder.Services.AddTransient<IStaffDataProcessor, StaffSqliteDataProcessor>();
        }
        else if (dbType.Equals("Postgres", StringComparison.OrdinalIgnoreCase))
        {
            builder.Services.AddTransient<IPostgresDataAccess, PostgresDataAccess>();
            builder.Services.AddTransient<IStaffService, StaffPostgresService>();
            builder.Services.AddTransient<ICheckInService, CheckInPostgresService>();
            builder.Services.AddTransient<IStaffData, StaffPostgresData>();
            builder.Services.AddTransient<ICheckInData, CheckInPostgresData>();
            builder.Services.AddTransient<IStaffDataProcessor, StaffPostgresDataProcessor>();
        }
        else
        {
            builder.Services.AddTransient<ISqlDataAccess, SqlDataAccess>();
            builder.Services.AddTransient<IStaffService, StaffService>();
            builder.Services.AddTransient<ICheckInService, CheckInService>();
            builder.Services.AddTransient<IStaffData, StaffData>();
            builder.Services.AddTransient<ICheckInData, CheckInData>();
            builder.Services.AddTransient<IStaffDataProcessor, StaffDataProcessor>();
        }

        builder.Services.AddTransient<IConnectionStringData, ConnectionStringData>();
        builder.Services.AddTransient<IPhoneNumberDtoParser, PhoneNumberDtoParser>();
        builder.Services.AddTransient<IUserContext, UserContext>();
        builder.Services.AddTransient<IUserService, UserService>();
        builder.Services.AddTransient<IDepartmentSelectListService, DepartmentSelectListService>();
        builder.Services.AddTransient<IStaffSelectListService, StaffSelectListService>();
        builder.Services.AddTransient<IEmailSender, EmailSender>();

        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        // old Services, to be removed later after refactoring Controllers to consume API with DTOs
        builder.Services.AddTransient<IPhoneNumberParser, PhoneNumberParser>();

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<IApiClient, ApiClient>();
    }

    public static void AddIdentityAndMvcServices(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("IdentityDb") ??
            throw new InvalidOperationException("Connection string 'IdentityDb' not found.");

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        builder.Services.AddControllersWithViews();
        builder.Services.AddSession();

        builder.Services.Configure<IdentityOptions>(options =>
        {
            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15); // Block for 15 minutes
            options.Lockout.MaxFailedAccessAttempts = 3; // Lock after 3 failed attempts
            options.Lockout.AllowedForNewUsers = true;   // Enable lockout for new users
            // Disable 2FA at framework level    
            options.SignIn.RequireConfirmedPhoneNumber = false;
        });
    }
}
