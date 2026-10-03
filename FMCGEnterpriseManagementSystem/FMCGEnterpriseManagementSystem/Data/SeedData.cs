/***************************************************************************************
*    Title: Seed Data
*    Author: iqran0906, Naseeha27
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Data/SeedData.cs
***************************************************************************************/

/***************************************************************************************
*    Title: Identity management in ASP.NET Core
*    Author: Microsoft
*    Date: 2026
*    Code version: ASP.NET Core Identity
*    Availability: https://learn.microsoft.com/aspnet/core/security/authentication/identity
***************************************************************************************/
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

            // Checks whether the configured user already exists.
            var user =
                await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new User
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    IsActive = true
                };

                // Creates the user using ASP.NET Core Identity password handling.
                var result =
                    await userManager.CreateAsync(
                        user,
                        password);

                if (!result.Succeeded)
                {
                    return;
                }
            }

            // Ensures the seeded user has the required application role.
            if (!await userManager.IsInRoleAsync(user, role))
            {
                await userManager.AddToRoleAsync(
                    user,
                    role);
            }
        }
    }
}