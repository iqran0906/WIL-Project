// Title: Asynchronous programming with async and await
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    // Defines the contract for managing employee user accounts.
    public interface IUserAccountService
    {
        // Retrieves employees who do not currently have user accounts.
        Task<IEnumerable<Employee>> GetEmployeesWithoutAccountsAsync();

        // Retrieves an employee using their unique employee identifier.
        // Returns null when the employee cannot be found.
        Task<Employee?> GetEmployeeByIdAsync(string employeeId);

        // Creates a user account for an employee using the supplied
        // employee ID, email address, password, and assigned role.
        // Returns true when the account is successfully created.
        Task<bool> CreateAccountAsync(
            string employeeId,
            string email,
            string password,
            string role);

        // Activates an employee's user account.
        // Returns true when the account is successfully activated.
        Task<bool> ActivateAccountAsync(string employeeId);

        // Deactivates an employee's user account.
        // Returns true when the account is successfully deactivated.
        Task<bool> DeactivateAccountAsync(string employeeId);

        // Retrieves employees who already have user accounts.
        Task<IEnumerable<Employee>> GetEmployeesWithAccountsAsync();
    }
}