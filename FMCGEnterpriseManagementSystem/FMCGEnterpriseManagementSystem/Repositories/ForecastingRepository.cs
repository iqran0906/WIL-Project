using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FMCGEnterpriseManagementSystem.Repositories.Implementations
{
    public class ForecastingRepository : IForecastingRepository
    {
        private readonly ApplicationDbContext _context;

        public ForecastingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Gets items where current stock is at or below reorder level
        public async Task<IEnumerable<Inventory>> GetLowStockForRestockForecastAsync()
        {
            return await _context.Inventories
                .Include(i => i.Product)
                .Where(i => i.QuantityOnHand <= i.ReorderLevel)
                .AsNoTracking()
                .ToListAsync();
        }

        // Gets all inventory records with active products for analytical predictions
        public async Task<IEnumerable<Inventory>> GetInventoryForDemandAnalysisAsync()
        {
            return await _context.Inventories
                .Include(i => i.Product)
                .Where(i => i.Product != null && i.Product.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}