using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FMCGEnterpriseManagementSystem.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly ApplicationDbContext _context;

        public InventoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Inventory>> GetAllAsync()
        {
            return await _context.Set<Inventory>()
                .Include(i => i.Product)
                .Include(i => i.StockBatches)
                .ToListAsync();
        }

        public async Task<Inventory?> GetByIdAsync(int id)
        {
            return await _context.Set<Inventory>()
                .Include(i => i.Product)
                .Include(i => i.StockBatches)
                .FirstOrDefaultAsync(i => i.InventoryId == id);
        }

        public async Task<Inventory?> GetByProductIdAsync(int productId)
        {
            return await _context.Set<Inventory>()
                .Include(i => i.Product)
                .Include(i => i.StockBatches)
                .FirstOrDefaultAsync(i => i.ProductId == productId);
        }

        public async Task<bool> HasSufficientStockAsync(int productId, int quantity)
        {
            var inventory = await GetByProductIdAsync(productId);

            return inventory != null &&
                   inventory.QuantityOnHand >= quantity;
        }

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

        public async Task AddAsync(Inventory item)
        {
            await _context.Set<Inventory>().AddAsync(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Inventory item)
        {
            _context.Set<Inventory>().Update(item);
            await _context.SaveChangesAsync();
        }

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