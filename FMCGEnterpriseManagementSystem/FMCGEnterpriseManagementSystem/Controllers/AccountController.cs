// Purpose: Login, logout and access-denied pages.
// Authors: iqran0906 (from git history)
// Uses: ASP.NET Core Identity (Microsoft, MIT) https://learn.microsoft.com/aspnet/core/security/authentication/identity

using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IActivityService _activityService;
        private readonly UserManager<User> _userManager;

        public AccountController(
            IAuthService authService,
            IActivityService activityService,
            UserManager<User> userManager)
        {
            _authService = authService;
            _activityService = activityService;
            _userManager = userManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Dashboard", "Home");
            }

            return View(new LoginViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var isActive =
                await _authService.IsUserActiveAsync(model.Email);

            if (!isActive)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid email address or password.");

                return View(model);
            }

            var loggedIn = await _authService.LoginAsync(
                model.Email,
                model.Password,
                model.RememberMe);

            if (!loggedIn)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid email address or password.");

                return View(model);
            }

            // Recent Activity (the user is not signed in on this request yet, so look them up)
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                await _activityService.LogAsync(user.Id, user.UserName, "Account", "Logged in");
            }

            return RedirectToAction("Dashboard", "Home");
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}