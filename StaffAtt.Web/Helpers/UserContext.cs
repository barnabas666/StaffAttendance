using System.Security.Claims;

namespace StaffAtt.Web.Helpers;

/// <summary>
/// Service to access information about the currently logged-in user.
/// </summary>
public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _ctx;

    public UserContext(IHttpContextAccessor ctx)
    {
        _ctx = ctx;
    }

    public string? GetUserEmail()
    {
        return _ctx.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);
    }

    public IEnumerable<string> GetRoles()
    {
        return _ctx.HttpContext?.User?
            .FindAll(ClaimTypes.Role)
            .Select(r => r.Value)
            ?? Enumerable.Empty<string>();
    }
}
