

using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    // Controller responsible for handling user account authentication.
    public class AccountController : Controller
    {
        // Services used for authentication and recording user activity.
        private readonly IAuthService _authService;
        private readonly IActivityService _activityService;

        // ASP.NET Core Identity manager used to retrieve user account information.
        private readonly UserManager<User> _userManager;

        // Dependency injection provides the required services to the controller.
        public AccountController(
            IAuthService authService,
            IActivityService activityService,
            UserManager<User> userManager)
        {
            _authService = authService;
            _activityService = activityService;
            _userManager = userManager;
        }

        // Displays the login page to users who are not authenticated.
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            // Prevents an already authenticated user from returning to the login page.
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Dashboard", "Home");
            }

            return View(new LoginViewModel());
        }

        // Processes the submitted login form.
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // Checks whether the information entered by the user satisfies the validation rules in the ViewModel.
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Checks whether the user account is active before attempting login.
            var isActive =
                await _authService.IsUserActiveAsync(model.Email);

            if (!isActive)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid email address or password.");

                return View(model);
            }

            // Attempts to authenticate the user using the authentication service.
            var loggedIn = await _authService.LoginAsync(
                model.Email,
                model.Password,
                model.RememberMe);

            // Displays an error if the login credentials are not accepted.
            if (!loggedIn)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid email address or password.");

                return View(model);
            }

            // Retrieves the logged-in user's account so that the login action can be recorded in the recent activity section.

            /***************************************************************************************
            *    Title: ASP.NET Core Identity
            *    Author: Microsoft
            *    Date: 2026
            *    Code version: ASP.NET Core
            *    Availability: https://learn.microsoft.com/aspnet/core/security/authentication/identity
            ***************************************************************************************/
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null)
            {
                // Records the successful login as recent account activity.
                await _activityService.LogAsync(
                    user.Id,
                    user.UserName,
                    "Account",
                    "Logged in");
            }

            // Sends the authenticated user to the Dashboard.
            return RedirectToAction("Dashboard", "Home");
        }

        // Logs out the currently authenticated user.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();

            // Returns the user to the login page after logging out.
            return RedirectToAction(nameof(Login));
        }

        // Displays the page shown when a user does not have permission to access a particular resource.
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}