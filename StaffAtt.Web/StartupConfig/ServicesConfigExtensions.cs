using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using StaffAtt.Identity;
using StaffAtt.Web.Helpers;
using StaffAtt.Web.Models;

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

        builder.Services.AddTransient<IPhoneNumberDtoParser, PhoneNumberDtoParser>();
        builder.Services.AddTransient<IUserContext, UserContext>();
        builder.Services.AddTransient<IUserService, UserService>();
        builder.Services.AddTransient<IDepartmentSelectListService, DepartmentSelectListService>();
        builder.Services.AddTransient<IStaffSelectListService, StaffSelectListService>();
        builder.Services.AddTransient<IEmailSender, EmailSender>();

        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
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
