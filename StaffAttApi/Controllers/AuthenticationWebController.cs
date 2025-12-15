using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Ocsp;
using StaffAttApi.Helpers;
using StaffAttApi.Models;
using StaffAttApi.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace StaffAttApi.Controllers
{
    /// <summary>
    /// API controller for handling user authentication and authorization.
    /// Provides endpoints for user registration, login, email confirmation, password reset, and user management.
    /// </summary>
    [Route("api/auth")]
    [ApiController]
    public class AuthenticationWebController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IJwtService _jwtService;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<AuthenticationWebController> _logger;
        private readonly IConfiguration _cfg;

        public AuthenticationWebController(
            UserManager<IdentityUser> userManager,
            IJwtService jwtService,
            IEmailSender emailSender,
            ILogger<AuthenticationWebController> logger,
            IConfiguration cfg)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _emailSender = emailSender;
            _logger = logger;
            _cfg = cfg;
        }

        // POST auth/register
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult> Register([FromBody] RegisterRequest req)
        {
            var existing = await _userManager.FindByEmailAsync(req.Email);
            if (existing != null)
                return Conflict("User with that email already exists.");

            var user = new IdentityUser { UserName = req.Email, Email = req.Email };
            var createResult = await _userManager.CreateAsync(user, req.Password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                return BadRequest(errors);
            }

            // Assign Member role by default (ensure roles seeded elsewhere)
            if (!await _userManager.IsInRoleAsync(user, "Member"))
                await _userManager.AddToRoleAsync(user, "Member");

            // Generate email confirmation token and send confirmation link
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            // build link to the Web app RegisterConfirmation page (web will call /auth/confirm-email)
            // prefer config key "WebAppBaseUrl" (set in Web's appsettings.json)
            var webBase = _cfg.GetValue<string>("WebAppBaseUrl")?.TrimEnd('/') ?? _cfg.GetValue<string>("ApiBaseUrl") ?? "";
            var confirmUrl = $"{webBase}/Identity/Account/RegisterConfirmation?userId={user.Id}&code={code}";

            // Send email
            await _emailSender.SendEmailAsync(req.Email,
                                              "Confirm your email",
                                              EmailMessages.GetConfirmLinkMessage(confirmUrl));
            return Ok();
        }

        // GET auth/confirm-email?userId=...&code=...
        [HttpGet("confirm-email")]
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string code)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
                return BadRequest("Missing parameters.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return BadRequest(errors);
            }

            return Ok();
        }

        // POST auth/resend-confirmation
        [HttpPost("resend-confirmation")]
        [AllowAnonymous]
        public async Task<ActionResult> ResendConfirmation([FromBody] RegisterRequest body) // { email }
        {
            var user = await _userManager.FindByEmailAsync(body.Email);
            if (user == null) return NotFound("User not found.");

            if (await _userManager.IsEmailConfirmedAsync(user))
                return BadRequest("Email already confirmed.");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var webBase = _cfg.GetValue<string>("WebAppBaseUrl")?.TrimEnd('/') ?? _cfg.GetValue<string>("ApiBaseUrl") ?? "";
            var confirmUrl = $"{webBase}/Identity/Account/RegisterConfirmation?userId={user.Id}&code={code}";

            // Send email
            await _emailSender.SendEmailAsync(body.Email,
                                              "Confirm your email",
                                              EmailMessages.GetConfirmLinkMessage(confirmUrl));

            return Ok();
        }

        // POST auth/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthLoginResponse>> Login([FromBody] LoginRequest req)
        {
            var user = await _userManager.FindByEmailAsync(req.Email);
            if (user == null) return Unauthorized("Invalid credentials.");

            if (!await _userManager.CheckPasswordAsync(user, req.Password))
                return Unauthorized("Invalid credentials.");

            if (!await _userManager.IsEmailConfirmedAsync(user))
                return Unauthorized("Email not confirmed.");

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtService.GenerateToken(user.Id, user.Email ?? string.Empty, roles);

            var response = new AuthLoginResponse
            {
                Token = token,
                Email = user.Email ?? "",
                Roles = roles.ToList()
            };

            return Ok(response);
        }

        // POST auth/change-password
        [HttpPost("change-password")]
        [Authorize]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest req)
        {
            // get user from claims (sub)
            var debug = string.Join(" | ", User.Claims.Select(c => $"{c.Type}={c.Value}"));
            Console.WriteLine(debug);

            //var userId = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Invalid token.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var res = await _userManager.ChangePasswordAsync(user, req.CurrentPassword, req.NewPassword);
            if (!res.Succeeded)
            {
                var errors = string.Join("; ", res.Errors.Select(e => e.Description));
                return BadRequest(errors);
            }

            return Ok();
        }

        // POST auth/forgot-password
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordRequest req)
        {
            var user = await _userManager.FindByEmailAsync(req.Email);
            if (user == null)
                return Ok(); // Do not reveal user existence

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            string webBase = _cfg.GetValue<string>("WebAppBaseUrl")!.TrimEnd('/');
            string callbackUrl = $"{webBase}/Identity/Account/ResetPassword?userId={user.Id}&code={code}";

            await _emailSender.SendEmailAsync(req.Email,
                                              "Reset your password",
                                              EmailMessages.GetPasswordResetMessage(callbackUrl));
            return Ok();
        }

        // POST auth/reset-password
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordRequest req)
        {
            var user = await _userManager.FindByIdAsync(req.UserId);
            if (user == null)
                return BadRequest("User not found.");

            var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(req.Code));

            var result = await _userManager.ResetPasswordAsync(user, token, req.NewPassword);

            if (!result.Succeeded)
                return BadRequest(string.Join("; ", result.Errors.Select(e => e.Description)));

            return Ok();
        }

        // DELETE auth/user/{email}
        [HttpDelete("user/{email}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> DeleteUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return NotFound("User not found.");

            // remove roles
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Any())
                await _userManager.RemoveFromRolesAsync(user, roles);

            // delete user
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return BadRequest(string.Join("; ", result.Errors.Select(x => x.Description)));

            return Ok();
        }
    }
}
