using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FMCGEnterpriseManagementSystem.Services
{
    public class UserAccountService : IUserAccountService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public UserAccountService(
            ApplicationDbContext context,
            UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IEnumerable<Employee>> GetEmployeesWithoutAccountsAsync()
        {
            return await _context.Employees
                .AsNoTracking()
                .Where(e => e.UserId == null && e.IsActive)
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToListAsync();
        }

        public async Task<Employee?> GetEmployeeByIdAsync(string employeeId)
        {
            return await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.EmployeeID == employeeId);
        }

        public async Task<bool> CreateAccountAsync(
            string employeeId,
            string email,
            string password,
            string role)
        {
            if (role != "Employee" &&
                role != "SalesRepresentative")
            {
                return false;
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeID == employeeId);

            if (employee == null ||
                !employee.IsActive ||
                !string.IsNullOrWhiteSpace(employee.UserId))
            {
                return false;
            }

            var existingUser =
                await _userManager.FindByEmailAsync(email);

            if (existingUser != null)
            {
                return false;
            }

            var user = new User
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                IsActive = true
            };

            var createResult =
                await _userManager.CreateAsync(user, password);

            if (!createResult.Succeeded)
            {
                return false;
            }

            var roleResult =
                await _userManager.AddToRoleAsync(user, role);

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return false;
            }

            try
            {
                employee.UserId = user.Id;
                employee.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                await _userManager.DeleteAsync(user);
                return false;
            }
        }

        public async Task<bool> ActivateAccountAsync(string employeeId)
        {
            var employee = await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.EmployeeID == employeeId);

            if (employee?.User == null)
            {
                return false;
            }

            employee.User.IsActive = true;
            employee.UpdatedAt = DateTime.UtcNow;

            var result =
                await _userManager.UpdateAsync(employee.User);

            if (!result.Succeeded)
            {
                return false;
            }

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeactivateAccountAsync(string employeeId)
        {
            var employee = await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.EmployeeID == employeeId);

            if (employee?.User == null)
            {
                return false;
            }

            employee.User.IsActive = false;
            employee.UpdatedAt = DateTime.UtcNow;

            var result =
                await _userManager.UpdateAsync(employee.User);

            if (!result.Succeeded)
            {
                return false;
            }

            await _context.SaveChangesAsync();

            return true;
        }
    }
}