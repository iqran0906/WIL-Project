using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FMCGEnterpriseManagementSystem.Repositories.Implementations
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly ApplicationDbContext _context;

        public DashboardRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetTotalProductsCountAsync()
        {
            return await _context.Products.CountAsync(p => p.IsActive);
        }

        public async Task<int> GetTotalSuppliersCountAsync()
        {
            return await _context.Suppliers.CountAsync();
        }

        public async Task<int> GetTotalCustomersCountAsync()
        {
            return await _context.Customers.CountAsync();
        }

        public async Task<decimal> GetTotalInventoryValueAsync()
        {
            return await _context.Inventories
                .Include(i => i.Product)
                .SumAsync(i => i.QuantityOnHand * (i.Product != null ? i.Product.SellingPrice : 0));
        }

        public async Task<IEnumerable<Inventory>> GetLowStockInventoriesAsync()
        {
            return await _context.Inventories
                .Include(i => i.Product)
                .Where(i => i.QuantityOnHand <= i.ReorderLevel)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Dictionary<string, (int ItemCount, int TotalQty)>> GetCategoryStockSummariesAsync()
        {
            var summaries = await _context.Inventories
                .Include(i => i.Product)
                .Where(i => i.Product != null && i.Product.IsActive)
                .GroupBy(i => string.IsNullOrEmpty(i.Product.Category) ? "Uncategorized" : i.Product.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    ItemCount = g.Count(),
                    TotalQty = g.Sum(i => i.QuantityOnHand)
                })
                .ToListAsync();

            return summaries.ToDictionary(
                s => s.Category,
                s => (s.ItemCount, s.TotalQty)
            );
        }
    }
}