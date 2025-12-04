#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StaffAtt.Web.Helpers;
using System.ComponentModel.DataAnnotations;

namespace StaffAtt.Web.Areas.Identity.Pages.Account
{
    public class ResendEmailConfirmationModel : PageModel
    {
        private readonly IAuthClient _authClient;
        private readonly ILogger<ResendEmailConfirmationModel> _logger;

        public ResendEmailConfirmationModel(IAuthClient authClient,
                                            ILogger<ResendEmailConfirmationModel> logger)
        {
            _authClient = authClient;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var result = await _authClient.ResendConfirmationAsync(Input.Email);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Resend confirmation failed for {Email}: {Error}", Input.Email, result.ErrorMessage);
                TempData["Error"] = "Error sending confirmation email. " + result.ErrorMessage;
                return RedirectToPage();
            }

            _logger.LogInformation("Resend confirmation sent to {Email}", Input.Email);
            TempData["Success"] = "If your email exists, a new confirmation link has been sent.";
            return RedirectToPage("/Account/Login");
        }
    }
}
