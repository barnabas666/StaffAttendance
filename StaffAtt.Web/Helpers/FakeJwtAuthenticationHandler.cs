using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace StaffAtt.Web.Helpers;

public class FakeJwtAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IUserService _userService;

    public FakeJwtAuthenticationHandler(
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

        Logger.LogWarning("FakeAuthHandler: token = {Token}", token ?? "<NULL>");

        if (string.IsNullOrEmpty(token))
            return Task.FromResult(AuthenticateResult.NoResult());

        var email = _userService.GetEmail();
        Logger.LogWarning("FakeAuthHandler: email = {Email}", email ?? "<NULL>");

        var roles = _userService.GetRoles();
        Logger.LogWarning("FakeAuthHandler: roles = {Roles}", string.Join(",", roles));

        var claims = new List<Claim> { new Claim(ClaimTypes.Name, email ?? "") };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var identity = new ClaimsIdentity(
        claims,
        Scheme.Name,
        ClaimTypes.Name,
        ClaimTypes.Role
        );

        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        // redirect to real login page
        Response.Redirect("/Identity/Account/Login");
        return Task.CompletedTask;
    }
}
