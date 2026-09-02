using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FMCGEnterpriseManagementSystem.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _context.Employees
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Employee?> GetByIdAsync(string id)
        {
            return await _context.Employees
                .Include(e => e.NextOfKin)
                .FirstOrDefaultAsync(e => e.EmployeeID == id);
        }

        public async Task<Employee?> GetByEmployeeNumberAsync(string employeeNumber)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeNumber == employeeNumber);
        }

        public async Task AddAsync(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
        }

        public Task UpdateAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            return Task.CompletedTask;
        }

        public async Task<bool> EmployeeNumberExistsAsync(
    string employeeNumber,
    string? excludeEmployeeId = null)
        {
            return await _context.Employees.AnyAsync(e =>
                e.EmployeeNumber == employeeNumber &&
                (excludeEmployeeId == null || e.EmployeeID != excludeEmployeeId));
        }

        public async Task<bool> EmailExistsAsync(
            string email,
            string? excludeEmployeeId = null)
        {
            return await _context.Employees.AnyAsync(e =>
                e.Email == email &&
                (excludeEmployeeId == null || e.EmployeeID != excludeEmployeeId));
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}