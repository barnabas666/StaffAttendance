using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using StaffAtt.Web.Data;
using StaffAtt.Web.Helpers;
using StaffAttLibrary.Data;
using StaffAttLibrary.Db;

var builder = WebApplication.CreateBuilder(args);

// "IdentityDb" is connectionString for Identity framework.
var connectionString = builder.Configuration.GetConnectionString("IdentityDb") ??
    throw new InvalidOperationException("Connection string 'IdentityDb' not found.");
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(connectionString));
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

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
builder.Services.AddTransient<IPhoneNumberParser, PhoneNumberParser>();
builder.Services.AddTransient<IUserContext, UserContext>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IDepartmentSelectListService, DepartmentSelectListService>();
builder.Services.AddTransient<IStaffSelectListService, StaffSelectListService>();
builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15); // Block for 15 minutes
    options.Lockout.MaxFailedAccessAttempts = 3; // Lock after 3 failed attempts
    options.Lockout.AllowedForNewUsers = true;   // Enable lockout for new users
    // Disable 2FA at framework level    
    options.SignIn.RequireConfirmedPhoneNumber = false;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Staff}/{action=Details}/{id?}");
app.MapRazorPages();

// Seed roles and admin user
async Task SeedRolesAndAdminAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    var roles = new[] { "Administrator", "Member" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    string email = "admin@admin.com";
    string password = "Pwd.1111";

    if (await userManager.FindByEmailAsync(email) == null)
    {
        var adminUser = new IdentityUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true // Optional: Skip email confirmation for seeded user
        };
        var result = await userManager.CreateAsync(adminUser, password);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Administrator");
        }
    }
}

await SeedRolesAndAdminAsync(app.Services);

app.Run();
