// Title: Saving Data - Entity Framework Core
// Author: Microsoft
// Date: 14-06-2025
// Code version: Entity Framework Core / .NET 10
// Availability: https://learn.microsoft.com/en-us/ef/core/saving/


using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.Repositories.Interfaces
{
    // Purpose: Defines the database operations required for product management.
    public interface IProductRepository
    {
        // Retrieves all products.
        Task<IEnumerable<Product>> GetAllAsync();

        // Retrieves a product by its ID.
        Task<Product?> GetByIdAsync(int id);

        // Retrieves a product using its product code.
        Task<Product?> GetByCodeAsync(string productCode);

        // Adds a new product.
        Task AddAsync(Product product);

        // Updates an existing product.
        Task UpdateAsync(Product product);

        // Deletes a product using its ID.
        Task DeleteAsync(int id);

        // Saves pending changes to the database and returns whether the operation succeeded.
        Task<bool> SaveChangesAsync();
    }
}