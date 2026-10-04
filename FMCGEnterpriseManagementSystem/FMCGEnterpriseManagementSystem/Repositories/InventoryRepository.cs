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
    // Purpose: Provides database operations for inventory management.
    public class InventoryRepository : IInventoryRepository
    {
        // Provides access to the application's database.
        private readonly ApplicationDbContext _context;

        // Receives the database context through dependency injection.
        public InventoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Retrieves all inventory records with their products and stock batches.
        public async Task<IEnumerable<Inventory>> GetAllAsync()
        {
            return await _context.Set<Inventory>()
                .Include(i => i.Product)
                .Include(i => i.StockBatches)
                .ToListAsync();
        }

        // Retrieves an inventory record by its ID with related product and batch information.
        public async Task<Inventory?> GetByIdAsync(int id)
        {
            return await _context.Set<Inventory>()
                .Include(i => i.Product)
                .Include(i => i.StockBatches)
                .FirstOrDefaultAsync(i => i.InventoryId == id);
        }

        // Retrieves an inventory record using the related product ID.
        public async Task<Inventory?> GetByProductIdAsync(int productId)
        {
            return await _context.Set<Inventory>()
                .Include(i => i.Product)
                .Include(i => i.StockBatches)
                .FirstOrDefaultAsync(i => i.ProductId == productId);
        }

        // Checks whether enough stock is available for the requested quantity.
        public async Task<bool> HasSufficientStockAsync(int productId, int quantity)
        {
            var inventory = await GetByProductIdAsync(productId);

            return inventory != null &&
                   inventory.QuantityOnHand >= quantity;
        }

        // Deducts the requested quantity from the available inventory.
        public async Task DeductStockAsync(int productId, int quantity)
        {
            var inventory = await GetByProductIdAsync(productId);

            if (inventory != null)
            {
                inventory.QuantityOnHand -= quantity;
                _context.Inventories.Update(inventory);
                await _context.SaveChangesAsync();
            }
        }

        // Adds a new inventory record and saves the changes.
        public async Task AddAsync(Inventory item)
        {
            await _context.Set<Inventory>().AddAsync(item);
            await _context.SaveChangesAsync();
        }

        // Updates an existing inventory record and saves the changes.
        public async Task UpdateAsync(Inventory item)
        {
            _context.Set<Inventory>().Update(item);
            await _context.SaveChangesAsync();
        }

        // Finds and deletes an inventory record when it exists.
        public async Task DeleteAsync(int id)
        {
            var item = await GetByIdAsync(id);

            if (item != null)
            {
                _context.Set<Inventory>().Remove(item);
                await _context.SaveChangesAsync();
            }
        }
    }
}