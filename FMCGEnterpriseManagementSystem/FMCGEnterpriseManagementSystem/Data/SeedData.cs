
// Title: ASP.NET Core Identity
// Author: Microsoft
// Date: 10-11-2025
// Code version: ASP.NET Core 10.0
// Availability: https://learn.microsoft.com/aspnet/core/security/authentication/identity

using FMCGEnterpriseManagementSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace FMCGEnterpriseManagementSystem.Data
{
    // Provides the initial roles and user accounts required by the application.
    public static class SeedData
    {
        public static async Task InitializeAsync(
            IServiceProvider serviceProvider,
            IConfiguration configuration)
        {
            // Retrieves the Identity services used to manage roles and users.
            var roleManager =
                serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var userManager =
                serviceProvider.GetRequiredService<UserManager<User>>();

            // Defines the application roles that should exist in the database.
            string[] roles =
            {
                "Administrator",
                "Employee",
                "SalesRepresentative"
            };

            // Creates each role only when it does not already exist.
            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(
                        new IdentityRole(roleName));
                }
            }

            // Creates or updates the seeded administrator account.
            await SeedUserAsync(
                userManager,
                configuration["SeedAdmin:Email"],
                configuration["SeedAdmin:Password"],
                "Administrator");

            // Creates or updates the seeded employee account.
            await SeedUserAsync(
                userManager,
                configuration["SeedEmployee:Email"],
                configuration["SeedEmployee:Password"],
                "Employee");

            // Creates or updates the seeded sales representative account.
            await SeedUserAsync(
                userManager,
                configuration["SeedSalesRepresentative:Email"],
                configuration["SeedSalesRepresentative:Password"],
                "SalesRepresentative");
        }

        private static async Task SeedUserAsync(
            UserManager<User> userManager,
            string? email,
            string? password,
            string role)
        {
            // Skips account creation when the required configuration is missing.
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                return;
            }

            // Check whether the configured user already exists.
            var user = await userManager.FindByEmailAsync(email);

            // Never change the password or permissions of an existing account.
            if (user != null)
            {
                return;
            }

            // Create a new account only when it does not already exist.
            user = new User
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                IsActive = true
            };

            // ASP.NET Core Identity securely hashes the password.
            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                return;
            }

            // Assign the initial role only when the account is first created.
            var roleResult = await userManager.AddToRoleAsync(user, role);

            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Unable to assign the {role} role to the seeded account.");
            }
        }
    }
}