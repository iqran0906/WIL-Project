using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();

        Task<Employee?> GetEmployeeByIdAsync(string id);

        Task<Employee?> GetEmployeeByNumberAsync(string employeeNumber);

        Task<IEnumerable<Employee>> SearchEmployeesAsync(string keyword);

        Task<bool> EmployeeNumberExistsAsync(
    string employeeNumber,
    string? excludeEmployeeId = null);

        Task<bool> EmailExistsAsync(
            string email,
            string? excludeEmployeeId = null);

        Task<bool> CreateEmployeeAsync(Employee employee);

        Task<bool> UpdateEmployeeAsync(Employee employee);

        Task<bool> DeactivateEmployeeAsync(string id);
    }
}