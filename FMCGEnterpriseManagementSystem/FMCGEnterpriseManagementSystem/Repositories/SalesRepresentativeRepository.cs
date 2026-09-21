using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FMCGEnterpriseManagementSystem.Repositories
{
    public class SalesRepresentativeRepository
        : ISalesRepresentativeRepository
    {
        private readonly ApplicationDbContext _context;

        public SalesRepresentativeRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SalesRepresentative>> GetAllAsync()
        {
            return await _context.SalesRepresentatives
                .Include(sr => sr.Employee)
                .OrderBy(sr => sr.Employee.FirstName)
                .ThenBy(sr => sr.Employee.LastName)
                .ToListAsync();
        }

        public async Task<SalesRepresentative?> GetByIdAsync(
            int salesRepresentativeId)
        {
            return await _context.SalesRepresentatives
                .Include(sr => sr.Employee)
                .FirstOrDefaultAsync(
                    sr => sr.SalesRepresentativeId ==
                          salesRepresentativeId);
        }

        public async Task<SalesRepresentative?> GetByEmployeeIdAsync(
            string employeeId)
        {
            return await _context.SalesRepresentatives
                .Include(sr => sr.Employee)
                .FirstOrDefaultAsync(
                    sr => sr.EmployeeID == employeeId);
        }

        public async Task<SalesRepresentative?> GetByCodeAsync(
            string salesRepCode)
        {
            return await _context.SalesRepresentatives
                .Include(sr => sr.Employee)
                .FirstOrDefaultAsync(
                    sr => sr.SalesRepCode == salesRepCode);
        }

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

        public async Task AddAsync(
            SalesRepresentative salesRepresentative)
        {
            await _context.SalesRepresentatives
                .AddAsync(salesRepresentative);
        }

        public void Update(
            SalesRepresentative salesRepresentative)
        {
            _context.SalesRepresentatives
                .Update(salesRepresentative);
        }

        public async Task<bool> SalesRepCodeExistsAsync(
            string salesRepCode,
            int? excludeSalesRepresentativeId = null)
        {
            var query =
                _context.SalesRepresentatives
                    .AsQueryable();

            if (excludeSalesRepresentativeId.HasValue)
            {
                query = query.Where(
                    sr => sr.SalesRepresentativeId !=
                          excludeSalesRepresentativeId.Value);
            }

            return await query.AnyAsync(
                sr => sr.SalesRepCode == salesRepCode);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}