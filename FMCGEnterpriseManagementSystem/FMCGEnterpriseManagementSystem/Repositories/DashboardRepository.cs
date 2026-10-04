// Title: Grouping Data: LINQ
// Author: Maseeha17
// Date: 31-05-2024
// Code version: .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/linq/standard-query-operators/grouping-data

using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FMCGEnterpriseManagementSystem.Repositories.Implementations
{
    // Purpose: Provides database queries used to populate the application dashboard.
    public class DashboardRepository : IDashboardRepository
    {
        // Provides access to the application's database.
        private readonly ApplicationDbContext _context;

        // Receives the database context through dependency injection.
        public DashboardRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Counts all active products.
        public async Task<int> GetTotalProductsCountAsync()
        {
            return await _context.Products.CountAsync(p => p.IsActive);
        }

        // Counts all suppliers.
        public async Task<int> GetTotalSuppliersCountAsync()
        {
            return await _context.Suppliers.CountAsync();
        }

        // Counts all customers.
        public async Task<int> GetTotalCustomersCountAsync()
        {
            return await _context.Customers.CountAsync();
        }

        // Calculates the total value of inventory using the selling price.
        public async Task<decimal> GetTotalInventoryValueAsync()
        {
            return await _context.Inventories
                .Include(i => i.Product)
                .SumAsync(i => i.QuantityOnHand * (i.Product != null ? i.Product.SellingPrice : 0));
        }

        // Retrieves inventory items where stock is at or below the reorder level.
        public async Task<IEnumerable<Inventory>> GetLowStockInventoriesAsync()
        {
            return await _context.Inventories
                .Include(i => i.Product)
                .Where(i => i.QuantityOnHand <= i.ReorderLevel)
                .AsNoTracking()
                .ToListAsync();
        }

        // Groups active inventory by category and calculates item and quantity totals.
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

            // Converts the grouped results into a dictionary for dashboard use.
            return summaries.ToDictionary(
                s => s.Category,
                s => (s.ItemCount, s.TotalQty)
            );
        }
    }
}