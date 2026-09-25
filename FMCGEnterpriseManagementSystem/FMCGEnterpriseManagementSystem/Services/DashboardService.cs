using System;
using System.Linq;
using System.Threading.Tasks;
using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FMCGEnterpriseManagementSystem.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardViewModel> GetDashboardAnalyticsAsync()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .ToListAsync();

            // Calculate Metrics
            int totalProducts = products.Count;
            int lowStockItems = products.Count(p => p.QuantityInStock > 0 && p.QuantityInStock <= p.ReorderLevel);
            int outOfStockItems = products.Count(p => p.QuantityInStock <= 0);

            decimal totalInventoryValue = products.Sum(p => p.QuantityInStock * p.UnitPrice);

            // Estimate restock cost to bring low/out-of-stock items up to 2x reorder point
            decimal projectedRestockBudget = products
                .Where(p => p.QuantityInStock <= p.ReorderLevel)
                .Sum(p => (Math.Max(p.ReorderLevel * 2, 10) - p.QuantityInStock) * p.CostPrice);

            // Group Stock Count by Category
            var categoryGroupings = products
                .GroupBy(p => p.Category != null ? p.Category.CategoryName : "Unassigned")
                .Select(g => new
                {
                    CategoryName = g.Key,
                    TotalQuantity = g.Sum(p => p.QuantityInStock)
                })
                .OrderByDescending(g => g.TotalQuantity)
                .ToList();

            string[] categoryNames = categoryGroupings.Select(g => g.CategoryName).ToArray();
            int[] categoryQuantities = categoryGroupings.Select(g => g.TotalQuantity).ToArray();

            // Generate Critical Stock Watchlist
            var criticalAlerts = products
                .Where(p => p.QuantityInStock <= p.ReorderLevel)
                .OrderBy(p => p.QuantityInStock)
                .Take(10)
                .Select(p => new CriticalStockAlert
                {
                    ProductCode = p.ProductCode ?? $"ED-{p.ProductID:D4}",
                    ProductName = p.ProductName,
                    CurrentStock = p.QuantityInStock,
                    ReorderLevel = p.ReorderLevel,
                    HealthStatus = p.QuantityInStock == 0 ? "Critical" : "Low"
                })
                .ToList();

            return new DashboardViewModel
            {
                TotalProducts = totalProducts,
                LowStockItems = lowStockItems,
                OutOfStockItems = outOfStockItems,
                TotalInventoryValue = totalInventoryValue,
                ProjectedRestockBudget = projectedRestockBudget,
                CategoryNames = categoryNames,
                CategoryQuantities = categoryQuantities,
                CriticalStockAlerts = criticalAlerts
            };
        }
    }
}