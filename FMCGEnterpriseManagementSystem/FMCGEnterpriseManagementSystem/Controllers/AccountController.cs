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
        // Services used for authentication, activity logging
        // and communication with the email API.
        private readonly IAuthService _authService;
        private readonly IActivityService _activityService;
        private readonly IEmailApiClientService _emailApiClientService;

        // ASP.NET Core Identity manager used to retrieve users
        // and securely manage password reset tokens.
        private readonly UserManager<User> _userManager;


        // Dependency injection provides the required services.
        public AccountController(
            IAuthService authService,
            IActivityService activityService,
            UserManager<User> userManager,
            IEmailApiClientService emailApiClientService)
        {
            _authService = authService;
            _activityService = activityService;
            _userManager = userManager;
            _emailApiClientService = emailApiClientService;
        }


        // =========================
        // LOGIN
        // =========================

        // Displays the login page to users who are not authenticated.
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            // Prevents an authenticated user from returning
            // to the login page.
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction(
                    "Dashboard",
                    "Home");
            }

            return View(
                new LoginViewModel());
        }


        // Processes the submitted login form.
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            // Checks whether the account is active
            // before attempting authentication.
            var isActive =
                await _authService.IsUserActiveAsync(
                    model.Email);


            if (!isActive)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid email address or password.");

                return View(model);
            }


            // Attempts to authenticate the user.
            var loggedIn =
                await _authService.LoginAsync(
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


            // Retrieves the authenticated account
            // so the login can be recorded.
            var user =
                await _userManager.FindByEmailAsync(
                    model.Email);


            if (user != null)
            {
                await _activityService.LogAsync(
                    user.Id,
                    user.UserName,
                    "Account",
                    "Logged in");
            }


            return RedirectToAction(
                "Dashboard",
                "Home");
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

            return RedirectToAction(
                nameof(Login));
        }


        // =========================
        // FORGOT PASSWORD
        // =========================

        // Displays the form where a user enters
        // the email address associated with their account.
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View(
                new ForgotPasswordViewModel());
        }


        // Generates a secure Identity password reset token
        // and requests that the email API send the reset link.
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
                await _userManager.FindByEmailAsync(
                    model.Email);


            /*
             * Do not reveal whether an email address
             * belongs to an account.
             *
             * Unknown and inactive accounts therefore
             * receive the same confirmation screen.
             */
            if (user == null || !user.IsActive)
            {
                return RedirectToAction(
                    nameof(ForgotPasswordConfirmation));
            }


            // ASP.NET Core Identity generates the
            // secure password reset token.
            var token =
                await _userManager
                    .GeneratePasswordResetTokenAsync(
                        user);


            // The Identity token may contain characters
            // that are unsafe inside a URL, so it is
            // encoded before being added to the link.
            var encodedToken =
                WebEncoders.Base64UrlEncode(
                    Encoding.UTF8.GetBytes(
                        token));


            // Generates an absolute URL back to the
            // MVC application's ResetPassword action.
            var resetUrl =
                Url.Action(
                    nameof(ResetPassword),
                    "Account",
                    new
                    {
                        email = user.Email,
                        token = encodedToken
                    },
                    Request.Scheme);


            if (string.IsNullOrWhiteSpace(resetUrl))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to create the password reset link. " +
                    "Please try again.");

                return View(model);
            }


            // Sends the secure reset URL through the
            // existing Exclusive Distributors email API.
            var emailResult =
                await _emailApiClientService
                    .SendPasswordResetEmailAsync(
                        user.Email!,
                        resetUrl);


            if (!emailResult.Success)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "The password reset email could not be sent. " +
                    "Please try again.");

                return View(model);
            }


            return RedirectToAction(
                nameof(ForgotPasswordConfirmation));
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

        // Opens the reset page after the user follows
        // the secure link received by email.
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(
            string? email,
            string? token)
        {
            // A reset cannot be performed without
            // both the email and reset token.
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction(
                    nameof(Login));
            }


            var model =
                new ResetPasswordViewModel
                {
                    Email = email,
                    Token = token
                };


            return View(model);
        }


        // Processes the new password.
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
                await _userManager.FindByEmailAsync(
                    model.Email);


            /*
             * Do not expose whether an account exists.
             */
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
                        WebEncoders.Base64UrlDecode(
                            model.Token));
            }
            catch
            {
                ModelState.AddModelError(
                    string.Empty,
                    "The password reset link is invalid.");

                return View(model);
            }


            // Identity validates the reset token and
            // applies the configured password rules.
            var result =
                await _userManager.ResetPasswordAsync(
                    user,
                    decodedToken,
                    model.Password);


            if (result.Succeeded)
            {
                // Records the security-related account activity.
                await _activityService.LogAsync(
                    user.Id,
                    user.UserName,
                    "Account",
                    "Password reset");


                return RedirectToAction(
                    nameof(ResetPasswordConfirmation));
            }


            // Displays Identity validation messages,
            // for example password complexity errors
            // or an invalid/expired token.
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