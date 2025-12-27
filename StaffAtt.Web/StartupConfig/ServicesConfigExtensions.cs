using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.IdentityModel.Tokens;
using StaffAtt.Web.Helpers;
using StaffAtt.Web.Services;
using System.Text;

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

        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<IApiClient, ApiClient>();
        builder.Services.AddScoped<IAuthClient, AuthClient>();
    }

    public static void AddAuthAndMvcServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages();

        // Distributed cache required by session middleware
        builder.Services.AddDistributedMemoryCache();

        // Configure session (make cookie essential so it's created even without consent)
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;          // important for cookies to be created
            options.Cookie.SameSite = SameSiteMode.Lax;
            // you can tweak SecurePolicy depending on local dev vs prod:
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        });

        builder.Services.Configure<CookiePolicyOptions>(options =>
        {
            options.MinimumSameSitePolicy = SameSiteMode.Lax;
            options.HttpOnly = HttpOnlyPolicy.Always;
            options.Secure = CookieSecurePolicy.SameAsRequest;
        });

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = "SessionJwtScheme";
            options.DefaultChallengeScheme = "SessionJwtScheme";
        })
        .AddScheme<AuthenticationSchemeOptions, SessionJwtAuthenticationHandler>("SessionJwtScheme", null)
        .AddJwtBearer("Bearer", opts =>
        {
            opts.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Authentication:Issuer"],
                ValidAudience = builder.Configuration["Authentication:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(builder.Configuration["Authentication:SecretKey"]!))
            };
        });

        builder.Services.AddAuthorization(); // optional but recommended
    }
}
