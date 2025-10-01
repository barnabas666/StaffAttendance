// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StaffAtt.Web.Models;
using System.ComponentModel.DataAnnotations;

namespace StaffAtt.Web.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private HttpClient httpClient;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(IHttpClientFactory httpClientFactory, SignInManager<IdentityUser> signInManager, ILogger<LoginModel> logger)
        {
            httpClient = httpClientFactory.CreateClient("api");           
            _signInManager = signInManager;
            _logger = logger;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");

                    // start of Authentication - Token request code
                    var loginModel = new AuthenticationModel { Email = Input.Email, Password = Input.Password };
                    var response = await httpClient.PostAsJsonAsync<AuthenticationModel>("AuthenticationWeb/token", loginModel);

                    if (response.IsSuccessStatusCode)
                    {
                        var token = await response.Content.ReadAsStringAsync();
                        // HttpContext is from PageModel, here we store the token in session
                        HttpContext.Session.SetString("JwtToken", token);
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        string message = $"Token request failed ({(int)response.StatusCode} {response.ReasonPhrase})";
                        if (response.Content.Headers.ContentType?.MediaType == "application/problem+json")
                        {
                            message += $"\nDetails: {errorContent}";
                        }
                        ModelState.AddModelError(string.Empty, message);
                        return Page();
                    }
                    // end of Authentication - Token request code

                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    // Fetch the user to get lockout end time
                    var user = await _signInManager.UserManager.FindByEmailAsync(Input.Email);
                    if (user != null)
                    {
                        var lockoutEnd = await _signInManager.UserManager.GetLockoutEndDateAsync(user);
                        if (lockoutEnd.HasValue)
                        {
                            var remaining = lockoutEnd.Value.UtcDateTime - DateTime.UtcNow;
                            var minutes = Math.Ceiling(remaining.TotalMinutes);
                            ModelState.AddModelError(string.Empty, $"This account has been locked out for {minutes} minutes, please try again later.");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "This account has been locked out, please try again later.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "This account has been locked out, please try again later.");
                    }
                    _logger.LogWarning("User account locked out.");
                    return Page(); // Stay on login page and show message
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
