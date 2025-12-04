using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StaffAtt.Web.Helpers;
using StaffAtt.Web.Models;
using System.ComponentModel.DataAnnotations;

namespace StaffAtt.Web.Areas.Identity.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly IAuthClient _authClient;
    private readonly ILogger<RegisterModel> _logger;

    public RegisterModel(IAuthClient authClient, ILogger<RegisterModel> logger)
    {
        _authClient = authClient;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required, EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6,
            ErrorMessage = "Password must be at least 6 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var result = await _authClient.RegisterAsync(Input.Email, Input.Password);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Registration failed.");
            return Page();
        }

        _logger.LogInformation("Registration successful for {Email}", Input.Email);

        // Redirect to Register Confirmation like default Identity
        return RedirectToPage("RegisterConfirmation", new { email = Input.Email });
    }
}
