using Microsoft.AspNetCore.Identity;

namespace StaffAtt.Web.StartupConfig;

public static class IdentitySeedExtensions
{
    /// <summary>
    /// Seed default Roles and Admin user into Identity database.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static async Task SeedRolesAndAdminAsync(IServiceProvider services)
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
}
