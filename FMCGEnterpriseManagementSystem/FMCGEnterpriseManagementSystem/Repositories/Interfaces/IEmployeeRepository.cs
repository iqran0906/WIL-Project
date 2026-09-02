using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.Repositories.Interfaces
{
    public interface IEmployeeRepository // Defines what database operations Employee Management needs
    {
        Task<IEnumerable<Employee>> GetAllAsync();

        Task<Employee?> GetByIdAsync(string id);

        Task<Employee?> GetByEmployeeNumberAsync(string employeeNumber);

        Task AddAsync(Employee employee);

        Task UpdateAsync(Employee employee);

        Task<bool> EmployeeNumberExistsAsync(string employeeNumber);

        Task<bool> EmailExistsAsync(string email);

        Task SaveChangesAsync();
    }
}