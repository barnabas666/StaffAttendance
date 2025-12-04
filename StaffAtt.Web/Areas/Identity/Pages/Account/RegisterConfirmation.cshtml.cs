#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StaffAtt.Web.Helpers;

namespace StaffAtt.Web.Areas.Identity.Pages.Account
{
    public class RegisterConfirmationModel : PageModel
    {
        private readonly IAuthClient _authClient;
        private readonly ILogger<RegisterConfirmationModel> _logger;

        public RegisterConfirmationModel(IAuthClient authClient,
                                         ILogger<RegisterConfirmationModel> logger)
        {
            _authClient = authClient;
            _logger = logger;
        }

        [BindProperty(SupportsGet = true)]
        public string Email { get; set; }

        public bool ConfirmationSent { get; set; }
        public bool ConfirmationSucceeded { get; set; }
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string email, string userId, string code, string returnUrl = null)
        {
            Email = email;

            // CASE 1 — AFTER REGISTER → SHOW "Email sent" message
            if (!string.IsNullOrEmpty(email) && string.IsNullOrEmpty(userId) && string.IsNullOrEmpty(code))
            {
                ConfirmationSent = true;
                return Page();
            }

            // CASE 2 — CONFIRMATION CLICKED FROM EMAIL
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
            {
                TempData["Error"] = "Invalid or expired confirmation link.";
                return RedirectToPage("/Account/Login");
            }

            var result = await _authClient.ConfirmEmailAsync(userId, code);

            if (!result.IsSuccess)
            {
                ErrorMessage = result.ErrorMessage ?? "Email confirmation failed.";
                _logger.LogWarning("Email confirmation failed for {Email}", email);
                return Page();
            }

            ConfirmationSucceeded = true;
            _logger.LogInformation("Email confirmed for {Email}", email);
            return Page();
        }
    }
}
