using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StaffAtt.Web.Helpers;

namespace StaffAtt.Web.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly IUserService _userService;

        public LogoutModel(IUserService userService)
        {
            _userService = userService;
        }

        public IActionResult OnPost()
        {
            _userService.SignOut();
            TempData["Success"] = "You have been logged out successfully.";
            return RedirectToPage("/Account/Login");
        }
    }
}
