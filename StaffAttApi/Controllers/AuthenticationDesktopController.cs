using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StaffAttLibrary.Data;
using StaffAttLibrary.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StaffAttApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationDesktopController : ControllerBase
{
    private readonly IStaffService _staffService;
    private readonly IConfiguration _config;

    public AuthenticationDesktopController(IStaffService staffService, IConfiguration config)
    {
        _staffService = staffService;
        _config = config;
    }

    public record AuthenticationData(string? Alias, string? PIN);
    public record AliasData(int Id, string Alias);

    [HttpPost("token")]
    [AllowAnonymous]
    public async Task<ActionResult<string>> Authenticate([FromBody] AuthenticationData data)
    {
        var aliasData = await ValidateCredentials(data);

        if (aliasData is null)
        {
            return Unauthorized();
        }

        string token = GenerateToken(aliasData);

        return Ok(token);
    }

    private string GenerateToken(AliasData aliasData)
    {
        var secretKey = new SymmetricSecurityKey(
            Encoding.ASCII.GetBytes(
                _config.GetValue<string>("Authentication:SecretKey")));

        var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        List<Claim> claims = new();
        claims.Add(new(JwtRegisteredClaimNames.Sub, aliasData.Id.ToString()));
        claims.Add(new(JwtRegisteredClaimNames.UniqueName, aliasData.Alias));

        var token = new JwtSecurityToken(
            _config.GetValue<string>("Authentication:Issuer"),
            _config.GetValue<string>("Authentication:Audience"),
            claims,
            DateTime.UtcNow,
            DateTime.UtcNow.AddMinutes(10),
            signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<AliasData?> ValidateCredentials(AuthenticationData data)
    {
        AliasModel? aliasModel = null;

        aliasModel = await _staffService.AliasVerificationAsync(data.Alias, data.PIN);
        
        if (aliasModel != null)
        {
            return new AliasData(aliasModel.Id, aliasModel.Alias);
        }

        return null;
    }
}
