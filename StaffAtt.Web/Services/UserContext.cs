using System.Security.Claims;

namespace StaffAtt.Web.Services;

/// <summary>
/// Implementation of <see cref="IUserContext"/> for accessing user information from the HTTP context.
/// Provides methods to retrieve the user's email and roles based on claims.
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
