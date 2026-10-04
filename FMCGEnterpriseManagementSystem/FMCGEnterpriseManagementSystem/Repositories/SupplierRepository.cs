// Title: Implement CRUD - ASP.NET Core MVC with Entity Framework Core
// Author: Microsoft
// Date: 10-04-2024
// Code version: ASP.NET Core 10.0
// Availability: https://learn.microsoft.com/en-us/aspnet/core/data/ef-mvc/crud

using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FMCGEnterpriseManagementSystem.Repositories
{
    // Repository responsible for database operations related to suppliers.
    public class SupplierRepository : ISupplierRepository
    {
        // Provides access to the application's database.
        private readonly ApplicationDbContext _context;

        // Constructor receives the database context through dependency injection.
        public SupplierRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Retrieves all suppliers from the database.
        public async Task<IEnumerable<Supplier>> GetAllAsync()
        {
            return await _context.Suppliers.ToListAsync();
        }

        // Retrieves a specific supplier using its supplier ID.
        public async Task<Supplier?> GetByIdAsync(int id)
        {
            return await _context.Suppliers
                .FirstOrDefaultAsync(s => s.SupplierId == id);
        }

        // Adds a new supplier to the database and saves the changes.
        public async Task AddAsync(Supplier supplier)
        {
            await _context.Suppliers.AddAsync(supplier);
            await _context.SaveChangesAsync();
        }

        // Updates an existing supplier and saves the changes to the database.
        public async Task UpdateAsync(Supplier supplier)
        {
            _context.Suppliers.Update(supplier);
            await _context.SaveChangesAsync();
        }

        // Finds a supplier by ID and removes it when the supplier exists.
        public async Task DeleteAsync(int id)
        {
            var supplier = await GetByIdAsync(id);

            // Only removes and saves the supplier when a matching record exists.
            if (supplier != null)
            {
                _context.Suppliers.Remove(supplier);
                await _context.SaveChangesAsync();
            }
        }
    }
}