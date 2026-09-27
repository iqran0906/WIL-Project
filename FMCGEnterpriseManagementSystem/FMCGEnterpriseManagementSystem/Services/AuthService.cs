using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace FMCGEnterpriseManagementSystem.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthService(
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<bool> LoginAsync(
            string email,
            string password,
            bool rememberMe)
        {
            var user =
                await _userManager.FindByEmailAsync(email);

            if (user == null || !user.IsActive)
            {
                return false;
            }

            var result =
                await _signInManager.PasswordSignInAsync(
                    user,
                    password,
                    rememberMe,
                    lockoutOnFailure: true);

            return result.Succeeded;
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<bool> IsUserActiveAsync(string email)
        {
            var user =
                await _userManager.FindByEmailAsync(email);

            return user != null && user.IsActive;
        }
    }
}