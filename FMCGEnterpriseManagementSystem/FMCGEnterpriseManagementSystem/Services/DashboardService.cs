using System;
using System.Linq;
using System.Threading.Tasks;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IProductRepository _productRepository;

        public DashboardService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<DashboardViewModel> GetDashboardAnalyticsAsync()
        {
            var products = (await _productRepository.GetAllAsync()).ToList();

            int totalProducts = products.Count;

            // Access stock quantities and reorder levels safely through the Inventory navigation property
            int lowStockItems = products.Count(p => p.Inventory != null && p.Inventory.QuantityInStock > 0 && p.Inventory.QuantityInStock <= p.Inventory.ReorderLevel);
            int outOfStockItems = products.Count(p => p.Inventory == null || p.Inventory.QuantityInStock <= 0);

            // Calculate total inventory value using SellingPrice and Inventory QuantityInStock
            decimal totalInventoryValue = products.Sum(p => (p.Inventory?.QuantityInStock ?? 0) * p.SellingPrice);

            // Calculate restock budget using CostIncVat
            decimal projectedRestockBudget = products
                .Where(p => p.Inventory != null && p.Inventory.QuantityInStock <= p.Inventory.ReorderLevel)
                .Sum(p => (Math.Max(p.Inventory.ReorderLevel * 2, 10) - p.Inventory.QuantityInStock) * p.CostIncVat);

            // Group by Category property on Product
            var categoryGroupings = products
                .GroupBy(p => string.IsNullOrEmpty(p.Category) ? "Unassigned" : p.Category)
                .Select(g => new
                {
                    CategoryName = g.Key,
                    TotalQuantity = g.Sum(p => p.Inventory?.QuantityInStock ?? 0)
                })
                .OrderByDescending(g => g.TotalQuantity)
                .ToList();

            string[] categoryNames = categoryGroupings.Select(g => g.CategoryName).ToArray();
            int[] categoryQuantities = categoryGroupings.Select(g => g.TotalQuantity).ToArray();

            var criticalAlerts = products
                .Where(p => p.Inventory != null && p.Inventory.QuantityInStock <= p.Inventory.ReorderLevel)
                .OrderBy(p => p.Inventory.QuantityInStock)
                .Take(10)
                .Select(p => new CriticalStockAlert
                {
                    ProductCode = p.ProductCode ?? $"ED-{p.ProductId:D4}",
                    ProductName = p.ProductName,
                    CurrentStock = p.Inventory.QuantityInStock,
                    ReorderLevel = p.Inventory.ReorderLevel,
                    HealthStatus = p.Inventory.QuantityInStock == 0 ? "Critical" : "Low"
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