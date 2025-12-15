using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using StaffAtt.Web.Services;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace StaffAtt.Web.Helpers;

/// <summary>
/// Custom authentication handler that retrieves JWT tokens from the user session.
/// Integrates with ASP.NET Core authentication framework to authenticate users based on session-stored JWTs.
/// </summary>
public class SessionJwtAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IUserService _userService;

    public SessionJwtAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IUserService userService)
        : base(options, logger, encoder, clock)
    {
        _userService = userService;
    }

    /// <summary>
    /// Handles the authentication process by retrieving the JWT token from the user session.
    /// </summary>
    /// <returns></returns>
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var token = _userService.GetToken();

        if (string.IsNullOrEmpty(token))
            return Task.FromResult(AuthenticateResult.NoResult());

        var email = _userService.GetEmail();
        var roles = _userService.GetRoles();

        var claims = new List<Claim> { new Claim(ClaimTypes.Name, email ?? "") };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        // Create the identity and principal which will be associated with the authentication ticket
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
    
    /// <summary>
    /// Handles authentication challenges by redirecting unauthenticated users to the login page.
    /// </summary>
    /// <param name="properties"></param>
    /// <returns></returns>
    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.Redirect("/Identity/Account/Login");
        return Task.CompletedTask;
    }
}
