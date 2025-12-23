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
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IJwtService _jwtService;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<AuthenticationWebController> _logger;
        private readonly IConfiguration _cfg;

        public AuthenticationWebController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IJwtService jwtService,
            IEmailSender emailSender,
            ILogger<AuthenticationWebController> logger,
            IConfiguration cfg)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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
            _logger.LogInformation("POST api/auth/register (Email={Email})", req.Email);

            var existing = await _userManager.FindByEmailAsync(req.Email);
            if (existing != null)
            {
                _logger.LogWarning("Registration failed - user already exists (Email={Email})", req.Email);
                return Conflict("User with that email already exists.");
            }

            var user = new IdentityUser { UserName = req.Email, Email = req.Email };
            var createResult = await _userManager.CreateAsync(user, req.Password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                _logger.LogWarning("Registration failed - identity validation error (Email={Email}, Errors={Errors})",
                                   req.Email,
                                   errors);
                return BadRequest(errors);
            }

            // Assign Member role by default (ensure roles seeded elsewhere)
            if (!await _userManager.IsInRoleAsync(user, "Member"))
                await _userManager.AddToRoleAsync(user, "Member");

            _logger.LogInformation("User registered successfully (Email={Email})", req.Email);

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
            _logger.LogInformation("GET api/auth/confirm-email (UserId={UserId})", userId);

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
            {
                _logger.LogWarning("Email confirmation failed - missing parameters");
                return BadRequest("Missing parameters.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Email confirmation failed - user not found (UserId={UserId})", userId);
                return NotFound("User not found.");
            }

            var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("Email confirmation failed (UserId={UserId}, Errors={Errors})",
                                   userId,
                                   errors);
                return BadRequest(errors);
            }

            _logger.LogInformation("Email confirmed successfully (UserId={UserId})", userId);
            return Ok();
        }

        // POST auth/resend-confirmation
        [HttpPost("resend-confirmation")]
        [AllowAnonymous]
        public async Task<ActionResult> ResendConfirmation([FromBody] RegisterRequest body) // { email }
        {
            _logger.LogInformation("POST api/auth/resend-confirmation (Email={Email})", body.Email);

            var user = await _userManager.FindByEmailAsync(body.Email);
            if (user == null)
            {
                // SECURITY: do not reveal user existence
                _logger.LogWarning("Resend confirmation requested for non-existing user (Email={Email})", body.Email);
                return Ok();
            }

            if (await _userManager.IsEmailConfirmedAsync(user))
            {
                _logger.LogInformation("Resend confirmation skipped - email already confirmed (Email={Email})", body.Email);
                return Ok();
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var webBase = _cfg.GetValue<string>("WebAppBaseUrl")?.TrimEnd('/') ?? _cfg.GetValue<string>("ApiBaseUrl") ?? "";
            var confirmUrl = $"{webBase}/Identity/Account/RegisterConfirmation?userId={user.Id}&code={code}";

            // Send email
            await _emailSender.SendEmailAsync(body.Email,
                                              "Confirm your email",
                                              EmailMessages.GetConfirmLinkMessage(confirmUrl));

            _logger.LogInformation("Confirmation email resent successfully (Email={Email})", body.Email);
            return Ok();
        }

        // POST auth/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthLoginResponse>> Login([FromBody] LoginRequest req)
        {
            _logger.LogInformation("POST api/auth/login attempt (Email={Email})", req.Email);

            var user = await _userManager.FindByEmailAsync(req.Email);
            if (user == null)
            {
                _logger.LogWarning("Login failed - user not found (Email={Email})", req.Email);
                return Unauthorized("Invalid credentials.");
            }

            var result = await _signInManager.PasswordSignInAsync(
                user,
                req.Password,
                isPersistent: false,
                lockoutOnFailure: true // THIS enables lockout
            );

            if (result.IsLockedOut)
            {
                var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
                _logger.LogWarning("Login blocked - account locked (Email={Email}, Until={Until})",
                                   user.Email,
                                   lockoutEnd);
                var minutes = lockoutEnd.HasValue
                    ? Math.Ceiling((lockoutEnd.Value - DateTimeOffset.UtcNow).TotalMinutes)
                    : 0;

                return Unauthorized($"Account locked. Try again in {minutes} minutes.");
            }

            if (!result.Succeeded)
            {
                _logger.LogWarning("Login failed - invalid password (Email={Email})", user.Email);
                return Unauthorized("Invalid credentials.");
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                _logger.LogWarning("Login failed - email not confirmed (Email={Email})", user.Email);
                return Unauthorized("Email not confirmed.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtService.GenerateToken(user.Id, user.Email ?? string.Empty, roles);

            _logger.LogInformation("Login successful (Email={Email}, Roles={Roles})",
                                   user.Email,
                                   string.Join(",", roles));

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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _logger.LogInformation("POST api/auth/change-password (UserId={UserId})", userId);

            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Change password failed - user not found (UserId={UserId})", userId);
                return Unauthorized("Invalid token.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Change password failed - user not found (UserId={UserId})", userId);
                return NotFound();
            }

            var res = await _userManager.ChangePasswordAsync(user, req.CurrentPassword, req.NewPassword);
            if (!res.Succeeded)
            {
                var errors = string.Join("; ", res.Errors.Select(e => e.Description));
                _logger.LogWarning("Change password failed (UserId={UserId}, Errors={Errors})",
                                   userId,
                                   errors);
                return BadRequest(errors);
            }

            return Ok();
        }

        // POST auth/forgot-password
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordRequest req)
        {
            _logger.LogInformation("POST api/auth/forgot-password (Email={Email})", req.Email);

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
            _logger.LogInformation("POST api/auth/reset-password (UserId={UserId})", req.UserId);

            var user = await _userManager.FindByIdAsync(req.UserId);
            if (user == null)
            {
                _logger.LogWarning("Reset password failed - user not found (UserId={UserId})", req.UserId);
                return BadRequest("Invalid reset request.");
            }

            var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(req.Code));

            var result = await _userManager.ResetPasswordAsync(user, token, req.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("Reset password failed (UserId={UserId}, Errors={Errors})",
                                   req.UserId,
                                   errors);
                return BadRequest("Invalid reset request.");
            }

            _logger.LogInformation("Password reset successful (UserId={UserId})", req.UserId);
            return Ok();
        }

        // DELETE auth/user/{email}
        [HttpDelete("user/{email}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> DeleteUser(string email)
        {
            _logger.LogWarning("DELETE api/auth/user requested (Email={Email})", email);

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("Delete user failed - not found (Email={Email})", email);
                return NotFound();
            }

            // remove roles
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Any())
                await _userManager.RemoveFromRolesAsync(user, roles);

            // delete user
            var result = await _userManager.DeleteAsync(user);

            _logger.LogInformation("User deleted successfully (Email={Email})", email);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("Delete user failed (Email={Email}, Errors={Errors})",
                                   email,
                                   errors);
                return BadRequest(errors);
            }

            return Ok();
        }
    }
}
