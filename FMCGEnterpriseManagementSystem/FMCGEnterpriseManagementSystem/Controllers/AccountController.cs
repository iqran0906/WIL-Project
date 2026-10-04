using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    // Controller responsible for handling user account authentication.
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


        // =========================
        // LOGIN
        // =========================

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

            var user =
                await _userManager.FindByEmailAsync(model.Email);

            if (user != null)
            {
                await _activityService.LogAsync(
                    user.Id,
                    user.UserName,
                    "Account",
                    "Logged in");
            }

            return RedirectToAction("Dashboard", "Home");
        }


        // =========================
        // LOGOUT
        // =========================

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();

            return RedirectToAction(nameof(Login));
        }


        // =========================
        // FORGOT PASSWORD
        // =========================

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordViewModel());
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(
            ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user =
                await _userManager.FindByEmailAsync(model.Email);

            // Do not reveal whether an account exists.
            if (user == null || !user.IsActive)
            {
                return RedirectToAction(
                    nameof(ForgotPasswordConfirmation));
            }

            var token =
                await _userManager.GeneratePasswordResetTokenAsync(user);

            var encodedToken =
                WebEncoders.Base64UrlEncode(
                    Encoding.UTF8.GetBytes(token));

            /*
             * TEMPORARY DEVELOPMENT FLOW
             *
             * Once Sayali's email notification API is connected,
             * the reset URL generated here should be emailed to
             * the user instead of redirecting directly to it.
             */

            return RedirectToAction(
                nameof(ResetPassword),
                new
                {
                    email = user.Email,
                    token = encodedToken
                });
        }


        // =========================
        // FORGOT PASSWORD CONFIRMATION
        // =========================

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }


        // =========================
        // RESET PASSWORD
        // =========================

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(
            string? email,
            string? token)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction(nameof(Login));
            }

            var model = new ResetPasswordViewModel
            {
                Email = email,
                Token = token
            };

            return View(model);
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(
            ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user =
                await _userManager.FindByEmailAsync(model.Email);

            if (user == null || !user.IsActive)
            {
                return RedirectToAction(
                    nameof(ResetPasswordConfirmation));
            }

            string decodedToken;

            try
            {
                decodedToken =
                    Encoding.UTF8.GetString(
                        WebEncoders.Base64UrlDecode(model.Token));
            }
            catch
            {
                ModelState.AddModelError(
                    string.Empty,
                    "The password reset link is invalid.");

                return View(model);
            }

            var result =
                await _userManager.ResetPasswordAsync(
                    user,
                    decodedToken,
                    model.Password);

            if (result.Succeeded)
            {
                await _activityService.LogAsync(
                    user.Id,
                    user.UserName,
                    "Account",
                    "Password reset");

                return RedirectToAction(
                    nameof(ResetPasswordConfirmation));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error.Description);
            }

            return View(model);
        }


        // =========================
        // RESET PASSWORD CONFIRMATION
        // =========================

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }


        // =========================
        // ACCESS DENIED
        // =========================

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}