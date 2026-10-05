// Title: Asynchronous programming with async and await
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    // Defines the operations used to manage employee records.
    public interface IEmployeeService
    {
        // Retrieves all employees from the system.
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();

        // Retrieves an employee using their unique employee ID.
        // Returns null if the employee cannot be found.
        Task<Employee?> GetEmployeeByIdAsync(string id);

        // Retrieves an employee using their employee number.
        // Returns null if no matching employee exists.
        Task<Employee?> GetEmployeeByNumberAsync(string employeeNumber);

        // Searches for employees using a keyword.
        Task<IEnumerable<Employee>> SearchEmployeesAsync(string keyword);

        // Checks whether an employee number already exists.
        // An optional employee ID can be excluded when updating an existing employee.
        Task<bool> EmployeeNumberExistsAsync(
    string employeeNumber,
    string? excludeEmployeeId = null);

        // Checks whether an email address already exists.
        // An optional employee ID can be excluded when updating an existing employee.
        Task<bool> EmailExistsAsync(
            string email,
            string? excludeEmployeeId = null);

        // Creates a new employee and returns whether the operation was successful.
        Task<bool> CreateEmployeeAsync(Employee employee);

        // Updates an existing employee and returns whether the operation was successful.
        Task<bool> UpdateEmployeeAsync(Employee employee);

        // Deactivates an employee using their employee ID.
        // Returns whether the operation was successful.
        Task<bool> DeactivateEmployeeAsync(string id);
        Task<bool> ReactivateEmployeeAsync(string id);
    }
}