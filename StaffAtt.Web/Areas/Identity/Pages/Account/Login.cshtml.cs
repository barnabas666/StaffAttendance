#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StaffAtt.Web.Services;
using System.ComponentModel.DataAnnotations;

namespace StaffAtt.Web.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IAuthClient _authClient;
        private readonly IUserService _userService;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(IAuthClient authClient,
                          IUserService userService,
                          ILogger<LoginModel> logger)
        {
            _authClient = authClient;
            _userService = userService;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (!ModelState.IsValid)
                return Page();

            var result = await _authClient.LoginAsync(Input.Email, Input.Password);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Invalid login attempt.");
                _logger.LogWarning("Login failed for {Email}", Input.Email);
                return Page();
            }

            var dto = result.Value;

            await _userService.SignInAsync(dto.Token, dto.Email, dto.Roles);

            _logger.LogWarning("After SignInAsync: session jwt='{Jwt}', email='{Email}'",
            HttpContext.Session.GetString("jwt"),
            HttpContext.Session.GetString("email"));

            _logger.LogInformation("User {Email} logged in.", dto.Email);

            return LocalRedirect(returnUrl);
        }
    }
}
