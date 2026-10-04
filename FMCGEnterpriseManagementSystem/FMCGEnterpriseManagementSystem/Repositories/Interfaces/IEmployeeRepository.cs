// Title: Entity Framework Core - Saving Data
// Author: Microsoft
// Date: 14-06-2025
// Code version: Entity Framework Core / .NET 10
// Availability: https://learn.microsoft.com/en-us/ef/core/saving/

using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.Repositories.Interfaces
{
    // Purpose: Defines the database operations required for employee management.
    public interface IEmployeeRepository
    {
        // Retrieves all employees.
        Task<IEnumerable<Employee>> GetAllAsync();

        // Retrieves an employee by their ID.
        Task<Employee?> GetByIdAsync(string id);

        // Retrieves an employee using their employee number.
        Task<Employee?> GetByEmployeeNumberAsync(string employeeNumber);

        // Searches for employees using a keyword.
        Task<IEnumerable<Employee>> SearchAsync(string keyword);

        // Adds a new employee to the database.
        Task AddAsync(Employee employee);

        // Updates an existing employee.
        Task UpdateAsync(Employee employee);

        // Checks whether an employee number already exists.
        Task<bool> EmployeeNumberExistsAsync(
            string employeeNumber,
            string? excludeEmployeeId = null);

        // Checks whether an email address already exists.
        Task<bool> EmailExistsAsync(
            string email,
            string? excludeEmployeeId = null);

        // Saves pending database changes.
        Task SaveChangesAsync();
    }
}


