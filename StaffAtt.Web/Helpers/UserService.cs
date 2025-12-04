using Microsoft.AspNetCore.Http;

namespace StaffAtt.Web.Helpers;

/// <summary>
/// Service for managing user authentication and session data.
/// </summary>
public class UserService : IUserService
{
    private readonly IHttpContextAccessor _ctx;

    public UserService(IHttpContextAccessor ctx)
    {
        _ctx = ctx;
    }

    public Task SignInAsync(string token, string email, IEnumerable<string> roles)
    {
        var session = _ctx.HttpContext!.Session;
        session.SetString("jwt", token);
        session.SetString("email", email);
        session.SetString("roles", string.Join(",", roles));

        return Task.CompletedTask;
    }

    public void SignOut()
    {
        var session = _ctx.HttpContext!.Session;
        session.Remove("jwt");
        session.Remove("email");
        session.Remove("roles");
    }

    public string? GetToken() => _ctx.HttpContext!.Session.GetString("jwt");

    public string? GetEmail() => _ctx.HttpContext!.Session.GetString("email");

    public IEnumerable<string> GetRoles()
    {
        var r = _ctx.HttpContext!.Session.GetString("roles");
        return string.IsNullOrEmpty(r)
            ? Enumerable.Empty<string>()
            : r.Split(',', StringSplitOptions.RemoveEmptyEntries);
    }
}
