// Title: Saving Data - Entity Framework Core
// Author: Microsoft
// Date: 14-06-2025
// Code version: Entity Framework Core / .NET 10
// Availability: https://learn.microsoft.com/en-us/ef/core/saving/

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;

namespace FMCGEnterpriseManagementSystem.Repositories
{
    // Repository responsible for database operations related to products.
    public class ProductRepository : IProductRepository
    {
        // Provides access to the application's database.
        private readonly ApplicationDbContext _context;

        // Constructor receives the database context through dependency injection.
        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Retrieves all products from the database.
        // Supplier information is included with each product.
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Supplier)
                .ToListAsync();
        }

        // Retrieves a specific product using its product ID.
        // The associated supplier is also loaded.
        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        // Retrieves a product using its unique product code.
        // Supplier information is also included.
        public async Task<Product?> GetByCodeAsync(string productCode)
        {
            return await _context.Products
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(p => p.ProductCode == productCode);
        }

        // Adds a new product to the Entity Framework change tracker.
        // The changes are saved when SaveChangesAsync is called.
        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
        }

        // Marks an existing product as modified so that its changes
        // can be persisted to the database.
        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await Task.CompletedTask;
        }

        // Finds a product by ID and removes it from the database context when the product exists.
        public async Task DeleteAsync(int id)
        {
            var product = await GetByIdAsync(id);

            if (product != null)
            {
                _context.Products.Remove(product);
            }
        }

        // Saves all pending changes made through the repository.
        // Returns true when at least one database record was changed.
        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}