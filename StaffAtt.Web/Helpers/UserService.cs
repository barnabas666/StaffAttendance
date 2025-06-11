using Microsoft.AspNetCore.Identity;

namespace StaffAtt.Web.Helpers;

/// <summary>
/// Service to handle user operations. Contains instances of UserManager and SignInManager.
/// Contains methods to find user by email, add user to role, and sign in user.
/// </summary>
public class UserService : IUserService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public UserService(UserManager<IdentityUser> userManager,
                       SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    /// <summary>
    /// Finds a user by email address.
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public Task<IdentityUser?> FindByEmailAsync(string email)
    {
        return _userManager.FindByEmailAsync(email);
    }

    /// <summary>
    /// Adds a user to a role.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="role"></param>
    /// <returns></returns>
    public Task<IdentityResult> AddToRoleAsync(IdentityUser user, string role)
    {
        return _userManager.AddToRoleAsync(user, role);
    }

    /// <summary>
    /// Signs in a user with the specified persistence option.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="isPersistent"></param>
    /// <returns></returns>
    public Task SignInAsync(IdentityUser user, bool isPersistent)
    {
        return _signInManager.SignInAsync(user, isPersistent);
    }

    /// <summary>
    /// Deletes the specified user asynchronously.
    /// </summary>
    /// <param name="user">The user to delete. Cannot be null.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task DeleteIdentityUserAsync(IdentityUser user)
    {
        await _userManager.DeleteAsync(user);
    }
}
