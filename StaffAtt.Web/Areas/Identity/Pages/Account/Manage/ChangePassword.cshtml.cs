#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StaffAtt.Web.Services;
using System.ComponentModel.DataAnnotations;

namespace StaffAtt.Web.Areas.Identity.Pages.Account.Manage
{
    public class ChangePasswordModel : PageModel
    {
        private readonly IAuthClient _authClient;
        private readonly IUserService _userService;
        private readonly ILogger<ChangePasswordModel> _logger;

        public ChangePasswordModel(IAuthClient authClient,
                                   IUserService userService,
                                   ILogger<ChangePasswordModel> logger)
        {
            _authClient = authClient;
            _userService = userService;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Current password")]
            public string OldPassword { get; set; }

            [Required]
            [StringLength(100, MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var result = await _authClient.ChangePasswordAsync(Input.OldPassword, Input.NewPassword);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Password change failed: {Err}", result.ErrorMessage);
                TempData["Error"] = "Password change failed: " + result.ErrorMessage;
                return RedirectToPage("/Account/Manage/ChangePassword");
            }

            _logger.LogInformation("Password changed for user {Email}", _userService.GetEmail());
            TempData["Success"] = "Your password was successfully updated.";
            return RedirectToAction("Details", "Staff");
        }
    }
}
