// Title: Saving Data - Entity Framework Core
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
    // Repository responsible for database operations related to sales representatives and their associated employees.
    public class SalesRepresentativeRepository
        : ISalesRepresentativeRepository
    {
        // Provides access to the application's database through Entity Framework Core.
        private readonly ApplicationDbContext _context;

        // Constructor receives the database context through dependency injection.
        public SalesRepresentativeRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // Retrieves all sales representatives together with their associated employee information.
        // Results are ordered alphabetically by employee first and last name.
        public async Task<IEnumerable<SalesRepresentative>> GetAllAsync()
        {
            return await _context.SalesRepresentatives
                .Include(sr => sr.Employee)
                .OrderBy(sr => sr.Employee.FirstName)
                .ThenBy(sr => sr.Employee.LastName)
                .ToListAsync();
        }

        // Retrieves a sales representative using their unique ID.
        // The associated employee information is also included.
        public async Task<SalesRepresentative?> GetByIdAsync(
            int salesRepresentativeId)
        {
            return await _context.SalesRepresentatives
                .Include(sr => sr.Employee)
                .FirstOrDefaultAsync(
                    sr => sr.SalesRepresentativeId ==
                          salesRepresentativeId);
        }

        // Retrieves a sales representative using the associated employee ID.
        public async Task<SalesRepresentative?> GetByEmployeeIdAsync(
            string employeeId)
        {
            return await _context.SalesRepresentatives
                .Include(sr => sr.Employee)
                .FirstOrDefaultAsync(
                    sr => sr.EmployeeID == employeeId);
        }

        // Retrieves a sales representative using their sales representative code.
        public async Task<SalesRepresentative?> GetByCodeAsync(
            string salesRepCode)
        {
            return await _context.SalesRepresentatives
                .Include(sr => sr.Employee)
                .FirstOrDefaultAsync(
                    sr => sr.SalesRepCode == salesRepCode);
        }

        // Retrieves employees who are active and have not already been assigned as sales representatives.
        // The results are ordered alphabetically by employee name.
        public async Task<IEnumerable<Employee>> GetEligibleEmployeesAsync()
        {
            return await _context.Employees
                .Where(e =>
                    e.IsActive &&
                    !_context.SalesRepresentatives
                        .Any(sr => sr.EmployeeID == e.EmployeeID))
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToListAsync();
        }

        // Adds a new sales representative to the Entity Framework context.
        public async Task AddAsync(
            SalesRepresentative salesRepresentative)
        {
            await _context.SalesRepresentatives
                .AddAsync(salesRepresentative);
        }

        // Marks an existing sales representative as modified so that its changes can be saved to the database.
        public void Update(
            SalesRepresentative salesRepresentative)
        {
            _context.SalesRepresentatives
                .Update(salesRepresentative);
        }

        // Checks whether a sales representative code already exists.
        // An optional ID can be excluded when checking during an update.
        public async Task<bool> SalesRepCodeExistsAsync(
            string salesRepCode,
            int? excludeSalesRepresentativeId = null)
        {
            // Creates a query that can be further filtered if required.
            var query =
                _context.SalesRepresentatives
                    .AsQueryable();

            // Excludes the current sales representative when an ID is provided.
            // This prevents an existing record from being considered a duplicate of itself during an update.
            if (excludeSalesRepresentativeId.HasValue)
            {
                query = query.Where(
                    sr => sr.SalesRepresentativeId !=
                          excludeSalesRepresentativeId.Value);
            }

            // Checks whether another sales representative uses the specified code.
            return await query.AnyAsync(
                sr => sr.SalesRepCode == salesRepCode);
        }

        // Saves all pending changes made through the repository to the database.
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}