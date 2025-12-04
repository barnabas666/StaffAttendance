using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StaffAtt.Web.Helpers;
using System.ComponentModel.DataAnnotations;

namespace StaffAtt.Web.Areas.Identity.Pages.Account;

public class ResetPasswordModel : PageModel
{
    private readonly IAuthClient _authClient;
    private readonly ILogger<ResetPasswordModel> _logger;

    public ResetPasswordModel(IAuthClient authClient, ILogger<ResetPasswordModel> logger)
    {
        _authClient = authClient;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        public string UserId { get; set; } = "";
        public string Code { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";
    }

    public void OnGet(string userId, string code)
    {
        Input.UserId = userId;
        Input.Code = code;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var result = await _authClient.ResetPasswordAsync(
            Input.UserId, Input.Code, Input.Password);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Password reset failed for UserId {UserId}: {ErrorMessage}", Input.UserId, result.ErrorMessage);
            TempData["Error"] = "Password reset failed: " + result.ErrorMessage;
            return RedirectToPage();
        }

        _logger.LogInformation("Password reset successful for UserId {UserId}", Input.UserId);
        TempData["Success"] = "Your password has been reset successfully.";
        return RedirectToPage("/Account/Login");
    }
}
