using Microsoft.EntityFrameworkCore;
using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;

namespace FMCGEnterpriseManagementSystem.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly ApplicationDbContext _context;

        public InventoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Inventory> GetByProductIdAsync(string productId)
        {
            return await _context.Inventories
                .FirstOrDefaultAsync(i => i.ProductID == productId);
        }

        public async Task<bool> HasSufficientStockAsync(string productId, int quantity)
        {
            var inventory = await GetByProductIdAsync(productId);
            return inventory != null && inventory.QuantityOnHand >= quantity;
        }

        public async Task DeductStockAsync(string productId, int quantity)
        {
            var inventory = await GetByProductIdAsync(productId);
            if (inventory != null)
            {
                inventory.QuantityOnHand -= quantity;
                _context.Inventories.Update(inventory);
                await _context.SaveChangesAsync();
            }
        }
    }
}