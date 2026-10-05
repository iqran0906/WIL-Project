//   Title: Authentication Service
//   Author: Microsoft
//   Date: 3 October 2026
//   Code version: Version 1.0
//   Availability: ASP.NET Core Identity (Microsoft, MIT) https://learn.microsoft.com/aspnet/core/security/authentication/identity

using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace FMCGEnterpriseManagementSystem.Services
{
    // Service responsible for handling user authentication and sign-in operations.
    public class AuthService : IAuthService
    {
        // Manages user accounts and provides methods for finding and managing users.
        private readonly UserManager<User> _userManager;

        // Manages user sign-in operations, including password authentication and sign-out.
        private readonly SignInManager<User> _signInManager;

        // Initialises the authentication service with the required Identity managers.
        public AuthService(
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Authenticates a user using their email address and password.
        // The rememberMe parameter determines whether the login session should persist.
        public async Task<bool> LoginAsync(
            string email,
            string password,
            bool rememberMe)
        {
            // Searches for a user account using the supplied email address.
            var user =
                await _userManager.FindByEmailAsync(email);

            // Prevents inactive or non-existent users from signing in.
            if (user == null || !user.IsActive)
            {
                return false;
            }

            // Attempts to sign the user in using their password.
            // Lockout is enabled when authentication fails repeatedly.
            var result =
                await _signInManager.PasswordSignInAsync(
                    user,
                    password,
                    rememberMe,
                    lockoutOnFailure: true);

            // Returns true when the authentication attempt succeeds.
            return result.Succeeded;
        }

        // Signs the currently authenticated user out of the application.
        public async Task LogoutAsync()
        {
            // Clears the user's authentication session.
            await _signInManager.SignOutAsync();
        }

        // Checks whether a user account exists and is currently active.
        public async Task<bool> IsUserActiveAsync(string email)
        {
            // Searches for the user account using the supplied email address.
            var user =
                await _userManager.FindByEmailAsync(email);

            // Returns true only when the user exists and their account is active.
            return user != null && user.IsActive;
        }
    }
}