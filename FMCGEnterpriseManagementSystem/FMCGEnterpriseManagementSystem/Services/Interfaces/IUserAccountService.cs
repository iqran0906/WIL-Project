/***************************************************************************************
*    Title: User Account Service Interface
*    Author: iqran0906
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Services/Interfaces/IUserAccountService.cs
***************************************************************************************/

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