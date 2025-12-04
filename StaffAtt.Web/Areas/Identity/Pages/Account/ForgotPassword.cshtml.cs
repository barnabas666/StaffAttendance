using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StaffAtt.Web.Helpers;
using System.ComponentModel.DataAnnotations;

namespace StaffAtt.Web.Areas.Identity.Pages.Account;

public class ForgotPasswordModel : PageModel
{
    private readonly IAuthClient _authClient;
    private readonly ILogger<ForgotPasswordModel> _logger;

    public ForgotPasswordModel(IAuthClient authClient, ILogger<ForgotPasswordModel> logger)
    {
        _authClient = authClient;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var result = await _authClient.ForgotPasswordAsync(Input.Email);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Error sending password reset email to {Email}: {ErrorMessage}", Input.Email, result.ErrorMessage);
            TempData["Error"] = "Error sending password reset email. " + result.ErrorMessage;
            return RedirectToPage();
        }

        _logger.LogInformation("Password reset email sent to {Email}", Input.Email);
        TempData["Success"] = "If your email exists, a password reset link has been sent.";
        return RedirectToPage("/Account/Login");
    }
}
