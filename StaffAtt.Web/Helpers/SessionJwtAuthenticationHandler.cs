using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace StaffAtt.Web.Helpers;

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

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var token = _userService.GetToken();

        if (string.IsNullOrEmpty(token))
            return Task.FromResult(AuthenticateResult.NoResult());

        var email = _userService.GetEmail();
        var roles = _userService.GetRoles();

        var claims = new List<Claim> { new Claim(ClaimTypes.Name, email ?? "") };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    // CRITICAL: redirect instead of 401
    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.Redirect("/Identity/Account/Login");
        return Task.CompletedTask;
    }
}
