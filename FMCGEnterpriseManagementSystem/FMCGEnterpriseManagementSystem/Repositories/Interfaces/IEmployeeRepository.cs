/***************************************************************************************
*    Title: Employee Repository Interface
*    Author: iqran0906
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Repositories/Interfaces/IEmployeeRepository.cs
***************************************************************************************/

using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.Repositories.Interfaces
{
    public interface IEmployeeRepository // Defines what database operations Employee Management needs
    {
        Task<IEnumerable<Employee>> GetAllAsync();

        Task<Employee?> GetByIdAsync(string id);

        Task<Employee?> GetByEmployeeNumberAsync(string employeeNumber);

        Task<IEnumerable<Employee>> SearchAsync(string keyword);

        Task AddAsync(Employee employee);

        Task UpdateAsync(Employee employee);

        Task<bool> EmployeeNumberExistsAsync(
            string employeeNumber,
            string? excludeEmployeeId = null);

        Task<bool> EmailExistsAsync(
            string email,
            string? excludeEmployeeId = null);

        Task SaveChangesAsync();
    }
}