// Purpose: Contract (interface) for the user account service.
// Authors: iqran0906 (from git history)

using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    public interface IUserAccountService
    {
        Task<IEnumerable<Employee>> GetEmployeesWithoutAccountsAsync();

        Task<Employee?> GetEmployeeByIdAsync(string employeeId);

        Task<bool> CreateAccountAsync(
            string employeeId,
            string email,
            string password,
            string role);

        Task<bool> ActivateAccountAsync(string employeeId);

        Task<bool> DeactivateAccountAsync(string employeeId);
        Task<IEnumerable<Employee>> GetEmployeesWithAccountsAsync();
    }
}