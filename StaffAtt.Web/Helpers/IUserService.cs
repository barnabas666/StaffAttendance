using Microsoft.AspNetCore.Identity;

namespace StaffAtt.Web.Helpers;
public interface IUserService
{
    Task<IdentityResult> AddToRoleAsync(IdentityUser user, string role);
    Task<IdentityUser?> FindByEmailAsync(string email);
    Task SignInAsync(IdentityUser user, bool isPersistent);
    Task DeleteUserAsync(IdentityUser user);
}