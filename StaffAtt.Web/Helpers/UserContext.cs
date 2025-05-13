using System.Security.Claims;

namespace StaffAtt.Web.Helpers;

/// <summary>
/// Service to handle user claims.
/// This class is used to get the current user's email from the ClaimsPrincipal.
/// </summary>
public class UserContext : IUserContext
{
    private readonly ClaimsPrincipal _user;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _user = httpContextAccessor.HttpContext.User;
    }

    /// <summary>
    /// Gets the email of the current user from the claims.
    /// </summary>
    /// <returns></returns>
    public string GetUserEmail()
    {
        return _user.FindFirst(ClaimTypes.Email)?.Value;
    }
}
