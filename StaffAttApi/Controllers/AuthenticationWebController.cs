using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StaffAttApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationWebController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthenticationWebController> _logger;

    public AuthenticationWebController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IConfiguration config,
        ILogger<AuthenticationWebController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _config = config;
        _logger = logger;
    }

    public record LoginData(string Email, string Password);

    [HttpPost("token")]
    [AllowAnonymous]
    public async Task<ActionResult<string>> Authenticate([FromBody] LoginData data)
    {
        _logger.LogInformation("POST: api/AuthenticationWeb/token (Email: {Email})", data.Email);

        IdentityUser? user = await _userManager.FindByEmailAsync(data.Email);
        if (user == null)
            return Unauthorized();

        var result = await _signInManager.CheckPasswordSignInAsync(user, data.Password, false);
        if (!result.Succeeded)
            return Unauthorized();

        // Optionally add roles/claims
        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName ?? user.Email ?? ""),
            new(JwtRegisteredClaimNames.Email, user.Email ?? "")
        };
        foreach (var role in roles)
        {
            claims.Add(new(ClaimTypes.Role, role));
        }

        var secretKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config.GetValue<string>("Authentication:SecretKey")));
        var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _config.GetValue<string>("Authentication:Issuer"),
            _config.GetValue<string>("Authentication:Audience"),
            claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: signingCredentials);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(jwt);
    }
}
