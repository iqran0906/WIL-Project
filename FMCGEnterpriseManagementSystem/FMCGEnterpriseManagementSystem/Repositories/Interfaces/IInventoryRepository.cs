// Title: Saving Data - Entity Framework Core
// Author: Microsoft
// Date: 14-06-2025
// Code version: Entity Framework Core / .NET 10
// Availability: https://learn.microsoft.com/en-us/ef/core/saving/

using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.Repositories.Interfaces
{
    // Purpose: Defines the database operations required for inventory management.
    public interface IInventoryRepository
    {
        // Retrieves all inventory records.
        Task<IEnumerable<Inventory>> GetAllAsync();

        // Retrieves an inventory record by its ID.
        Task<Inventory?> GetByIdAsync(int id);

        // Retrieves inventory using the related product ID.
        Task<Inventory?> GetByProductIdAsync(int productId);

        // Adds a new inventory record.
        Task AddAsync(Inventory inventory);

        // Updates an existing inventory record.
        Task UpdateAsync(Inventory inventory);

        // Checks whether enough stock is available for a requested quantity.
        Task<bool> HasSufficientStockAsync(int productId, int quantity);

        // Deducts the requested quantity from available stock.
        Task DeductStockAsync(int productId, int quantity);

        // Deletes an inventory record using its ID.
        Task DeleteAsync(int id);
    }
}