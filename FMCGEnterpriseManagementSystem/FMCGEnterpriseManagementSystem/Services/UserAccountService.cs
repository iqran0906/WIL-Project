// Title: Asynchronous programming with async and await
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FMCGEnterpriseManagementSystem.Services
{
    // Handles employee user accounts, including account creation,
    // activation and deactivation.
    public class UserAccountService : IUserAccountService
    {
        // Provides access to the application's database.
        private readonly ApplicationDbContext _context;

        // Provides ASP.NET Core Identity functionality for managing users.
        private readonly UserManager<User> _userManager;

        // Constructor receives the database context and UserManager
        // through dependency injection.
        public UserAccountService(
            ApplicationDbContext context,
            UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Retrieves active employees who do not yet have a user account.
        public async Task<IEnumerable<Employee>> GetEmployeesWithoutAccountsAsync()
        {
            return await _context.Employees
                // Prevents EF Core from tracking the returned employee objects
                // because they are only being displayed/read.
                .AsNoTracking()

                // Only active employees without an associated user account
                // are included.
                .Where(e => e.UserId == null && e.IsActive)

                // Sorts employees alphabetically by first name and then surname.
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)

                // Executes the query asynchronously.
                .ToListAsync();
        }

        // Retrieves employees who already have user accounts.
        public async Task<IEnumerable<Employee>> GetEmployeesWithAccountsAsync()
        {
            return await _context.Employees
                // Loads the related User information together with the employee.
                .Include(e => e.User)

                // Only employees linked to a user account are returned.
                .Where(e => e.UserId != null)

                // Sorts the employees alphabetically.
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)

                // Executes the database query asynchronously.
                .ToListAsync();
        }

        // Retrieves one employee using their employee ID.
        public async Task<Employee?> GetEmployeeByIdAsync(string employeeId)
        {
            return await _context.Employees
                // Includes the employee's related user account.
                .Include(e => e.User)

                // Finds the employee matching the supplied ID.
                .FirstOrDefaultAsync(e => e.EmployeeID == employeeId);
        }

        // Creates an Identity account for an employee and assigns the selected role.
        public async Task<bool> CreateAccountAsync(
            string employeeId,
            string email,
            string password,
            string role)
        {
            // Only Employee and SalesRepresentative roles are allowed
            // to be created through this service.
            if (role != "Employee" &&
                role != "SalesRepresentative")
            {
                return false;
            }

            // Additional validation is required when creating
            // a Sales Representative account.
            if (role == "SalesRepresentative")
            {
                // Checks that the employee is registered as an active
                // sales representative.
                var salesRepresentative = await _context.SalesRepresentatives
                    .AsNoTracking()
                    .FirstOrDefaultAsync(sr =>
                        sr.EmployeeID == employeeId &&
                        sr.IsActive);

                // An employee cannot receive this role unless
                // an active sales representative record exists.
                if (salesRepresentative == null)
                {
                    return false;
                }
            }

            // Finds the employee who will receive the account.
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeID == employeeId);

            // The employee must exist, be active, and not already
            // be linked to another user account.
            if (employee == null ||
                !employee.IsActive ||
                !string.IsNullOrWhiteSpace(employee.UserId))
            {
                return false;
            }

            // Checks whether the supplied email address is already
            // registered as an Identity user.
            var existingUser =
                await _userManager.FindByEmailAsync(email);

            // Prevents duplicate user accounts using the same email.
            if (existingUser != null)
            {
                return false;
            }

            // Creates the Identity user object.
            var user = new User
            {
                // Uses the email address as the username.
                UserName = email,

                // Stores the email address.
                Email = email,

                // Marks the email as confirmed.
                EmailConfirmed = true,

                // New accounts are active by default.
                IsActive = true
            };

            // Creates the user account using ASP.NET Core Identity
            // and applies the supplied password.
            var createResult =
                await _userManager.CreateAsync(user, password);

            // If Identity could not create the account,
            // the operation is stopped.
            if (!createResult.Succeeded)
            {
                return false;
            }

            // Assigns the selected role to the newly created user.
            var roleResult =
                await _userManager.AddToRoleAsync(user, role);

            // If the role assignment fails, the newly created user
            // is removed to avoid leaving an incomplete account.
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return false;
            }

            try
            {
                // Links the employee record to the newly created
                // Identity user account.
                employee.UserId = user.Id;

                // Records when the employee record was updated.
                employee.UpdatedAt = DateTime.UtcNow;

                // Saves the employee-to-user relationship.
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                // If saving the employee relationship fails,
                // remove the Identity account to keep the data consistent.
                await _userManager.DeleteAsync(user);
                return false;
            }
        }

        // Activates an existing employee user account.
        public async Task<bool> ActivateAccountAsync(string employeeId)
        {
            // Retrieves the employee together with the associated Identity user.
            var employee = await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.EmployeeID == employeeId);

            // An account cannot be activated if the employee or
            // associated user account does not exist.
            if (employee?.User == null)
            {
                return false;
            }

            // Marks the Identity account as active.
            employee.User.IsActive = true;

            // Updates the employee's last modified timestamp.
            employee.UpdatedAt = DateTime.UtcNow;

            // Saves the user account changes through Identity.
            var result =
                await _userManager.UpdateAsync(employee.User);

            // Stops if the Identity update failed.
            if (!result.Succeeded)
            {
                return false;
            }

            // Saves the employee changes to the database.
            await _context.SaveChangesAsync();

            return true;
        }

        // Deactivates an existing employee user account.
        public async Task<bool> DeactivateAccountAsync(string employeeId)
        {
            // Retrieves the employee and associated Identity user.
            var employee = await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.EmployeeID == employeeId);

            // The account cannot be deactivated if the employee
            // or associated user does not exist.
            if (employee?.User == null)
            {
                return false;
            }

            // Marks the Identity account as inactive.
            employee.User.IsActive = false;

            // Records when the employee was updated.
            employee.UpdatedAt = DateTime.UtcNow;

            // Updates the Identity user.
            var result =
                await _userManager.UpdateAsync(employee.User);

            // Stops if Identity could not update the user.
            if (!result.Succeeded)
            {
                return false;
            }

            // Saves the employee changes to the database.
            await _context.SaveChangesAsync();

            return true;
        }
    }
}