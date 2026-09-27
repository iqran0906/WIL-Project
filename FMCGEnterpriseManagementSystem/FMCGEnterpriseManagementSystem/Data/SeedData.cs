using FMCGEnterpriseManagementSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace FMCGEnterpriseManagementSystem.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(
            IServiceProvider serviceProvider,
            IConfiguration configuration)
        {
            var roleManager =
                serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var userManager =
                serviceProvider.GetRequiredService<UserManager<User>>();

            string[] roles =
            {
                "Administrator",
                "Employee",
                "SalesRepresentative"
            };

            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(
                        new IdentityRole(roleName));
                }
            }

            await SeedUserAsync(
                userManager,
                configuration["SeedAdmin:Email"],
                configuration["SeedAdmin:Password"],
                "Administrator");

            await SeedUserAsync(
                userManager,
                configuration["SeedEmployee:Email"],
                configuration["SeedEmployee:Password"],
                "Employee");

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
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                return;
            }

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

                var result =
                    await userManager.CreateAsync(
                        user,
                        password);

                if (!result.Succeeded)
                {
                    return;
                }
            }

            if (!await userManager.IsInRoleAsync(user, role))
            {
                await userManager.AddToRoleAsync(
                    user,
                    role);
            }
        }
    }
}