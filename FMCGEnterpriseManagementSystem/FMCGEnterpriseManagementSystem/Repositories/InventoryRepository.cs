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
                .ToListAsync();
        }

        public async Task<Inventory?> GetByIdAsync(int id)
        {
            return await _context.Set<Inventory>()
                .Include(i => i.Product)
                .FirstOrDefaultAsync(i => i.InventoryId == id);
        }

        public async Task<Inventory?> GetByProductIdAsync(int productId)
        {
            return await _context.Set<Inventory>()
                .Include(i => i.Product)
                .FirstOrDefaultAsync(i => i.ProductId == productId);
        }

        public async Task AddAsync(Inventory inventory)
        {
            await _context.Set<Inventory>().AddAsync(inventory);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Inventory inventory)
        {
            _context.Set<Inventory>().Update(inventory);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var inventory = await GetByIdAsync(id);
            if (inventory != null)
            {
                _context.Set<Inventory>().Remove(inventory);
                await _context.SaveChangesAsync();
            }
        }
    }
}