// Title: Entity Framework Core - Saving Data
// Author: Microsoft
// Date: 14-06-2025
// Code version: Entity Framework Core / .NET 10
// Availability: https://learn.microsoft.com/en-us/ef/core/saving/

using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FMCGEnterpriseManagementSystem.Repositories
{
    // Purpose: Provides database operations for employee management.
    public class EmployeeRepository : IEmployeeRepository
    {
        // Provides access to the application's database.
        private readonly ApplicationDbContext _context;

        // Receives the database context through dependency injection.
        public EmployeeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Retrieves all employees without tracking them for read-only use.
        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _context.Employees
                .AsNoTracking()
                .ToListAsync();
        }

        // Retrieves an employee by ID together with their next-of-kin details.
        public async Task<Employee?> GetByIdAsync(string id)
        {
            return await _context.Employees
                .Include(e => e.NextOfKin)
                .FirstOrDefaultAsync(e => e.EmployeeID == id);
        }

        // Retrieves an employee using their employee number.
        public async Task<Employee?> GetByEmployeeNumberAsync(string employeeNumber)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeNumber == employeeNumber);
        }

        // Searches employees by employee number, name, email, or job title.
        public async Task<IEnumerable<Employee>> SearchAsync(string keyword)
        {
            keyword = keyword.Trim();

            return await _context.Employees
                .AsNoTracking()
                .Where(e =>
                    e.EmployeeNumber.Contains(keyword) ||
                    e.FirstName.Contains(keyword) ||
                    e.LastName.Contains(keyword) ||
                    (e.FirstName + " " + e.LastName).Contains(keyword) ||
                    e.Email.Contains(keyword) ||
                    e.JobTitle.Contains(keyword))
                .ToListAsync();
        }

        // Adds a new employee to the database context.
        public async Task AddAsync(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
        }

        // Marks an employee as modified in the database context.
        public Task UpdateAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            return Task.CompletedTask;
        }

        // Checks whether an employee number already exists, excluding the current employee when required.
        public async Task<bool> EmployeeNumberExistsAsync(
    string employeeNumber,
    string? excludeEmployeeId = null)
        {
            return await _context.Employees.AnyAsync(e =>
                e.EmployeeNumber == employeeNumber &&
                (excludeEmployeeId == null || e.EmployeeID != excludeEmployeeId));
        }

        // Checks whether an email address already exists, excluding the current employee when required.
        public async Task<bool> EmailExistsAsync(
            string email,
            string? excludeEmployeeId = null)
        {
            return await _context.Employees.AnyAsync(e =>
                e.Email == email &&
                (excludeEmployeeId == null || e.EmployeeID != excludeEmployeeId));
        }

        // Saves all pending employee changes to the database.
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}