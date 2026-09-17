using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;

        // Injects only IDashboardRepository to remain independent of unmerged repositories
        public DashboardService(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<DashboardViewModel> GetDashboardAnalyticsAsync()
        {
            // Execute parallel aggregation queries
            var totalProductsTask = _dashboardRepository.GetTotalProductsCountAsync();
            var totalSuppliersTask = _dashboardRepository.GetTotalSuppliersCountAsync();
            var totalCustomersTask = _dashboardRepository.GetTotalCustomersCountAsync();
            var totalInventoryValueTask = _dashboardRepository.GetTotalInventoryValueAsync();
            var lowStockInventoriesTask = _dashboardRepository.GetLowStockInventoriesAsync();
            var categorySummariesTask = _dashboardRepository.GetCategoryStockSummariesAsync();

            await Task.WhenAll(
                totalProductsTask,
                totalSuppliersTask,
                totalCustomersTask,
                totalInventoryValueTask,
                lowStockInventoriesTask,
                categorySummariesTask
            );

            var lowStockList = await lowStockInventoriesTask;
            var categoryDict = await categorySummariesTask;

            // Map repository outputs directly into DashboardViewModel
            var viewModel = new DashboardViewModel
            {
                TotalProducts = await totalProductsTask,
                TotalSuppliers = await totalSuppliersTask,
                TotalCustomers = await totalCustomersTask,
                TotalInventoryValue = await totalInventoryValueTask,
                LowStockItems = lowStockList.Count(i => i.QuantityOnHand > 0),
                OutOfStockItems = lowStockList.Count(i => i.QuantityOnHand == 0),

                // Critical Stock Alerts
                CriticalStockAlerts = lowStockList.Select(i => new LowStockAlertItem
                {
                    ProductCode = i.Product?.ProductCode ?? "N/A",
                    ProductName = i.Product?.ProductName ?? "Unknown Item",
                    CurrentStock = i.QuantityOnHand,
                    ReorderLevel = i.ReorderLevel,
                    HealthStatus = i.QuantityOnHand == 0 ? "Critical" : "Warning"
                }).ToList(),

                // Category Summaries
                CategorySummaries = categoryDict.Select(kvp => new CategoryStockSummary
                {
                    CategoryName = kvp.Key,
                    ItemCount = kvp.Value.ItemCount,
                    TotalQuantity = kvp.Value.TotalQty
                }).ToList(),

                // Restock Budget Projection: (Target Buffer - Current Stock) * Cost
                ProjectedRestockBudget = lowStockList.Sum(i =>
                    ((i.ReorderLevel * 3) - i.QuantityOnHand) * (i.Product?.CostIncVat ?? 0))
            };

            return viewModel;
        }
    }
}