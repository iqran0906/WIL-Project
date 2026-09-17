using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly IProductRepository _productRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IInventoryRepository _inventoryRepository;

        public DashboardService(
            IProductRepository productRepository,
            ISupplierRepository supplierRepository,
            ICustomerRepository customerRepository,
            IInventoryRepository inventoryRepository)
        {
            _productRepository = productRepository;
            _supplierRepository = supplierRepository;
            _customerRepository = customerRepository;
            _inventoryRepository = inventoryRepository;
        }

        public async Task<DashboardViewModel> GetDashboardAnalyticsAsync()
        {
            var products = await _productRepository.GetAllAsync();
            var suppliers = await _supplierRepository.GetAllAsync();
            var customers = await _customerRepository.GetAllAsync();
            var inventories = await _inventoryRepository.GetAllAsync();

            var viewModel = new DashboardViewModel
            {
                TotalProducts = products.Count(),
                TotalSuppliers = suppliers.Count(),
                TotalCustomers = customers.Count(),

                // Inventory calculations using SellingPrice/CostIncVat
                TotalInventoryValue = inventories.Sum(i => i.QuantityOnHand * (i.Product?.SellingPrice ?? 0)),
                LowStockItems = inventories.Count(i => i.QuantityOnHand <= i.ReorderLevel && i.QuantityOnHand > 0),
                OutOfStockItems = inventories.Count(i => i.QuantityOnHand == 0),

                // Critical alert list
                CriticalStockAlerts = inventories
                    .Where(i => i.QuantityOnHand <= i.ReorderLevel)
                    .Select(i => new LowStockAlertItem
                    {
                        ProductCode = i.Product?.ProductCode ?? "N/A",
                        ProductName = i.Product?.ProductName ?? "Unknown",
                        CurrentStock = i.QuantityOnHand,
                        ReorderLevel = i.ReorderLevel,
                        HealthStatus = i.QuantityOnHand == 0 ? "Critical" : "Warning"
                    }).ToList(),

                // Category summary aggregations
                CategorySummaries = products
                    .GroupBy(p => p.Category)
                    .Select(g => new CategoryStockSummary
                    {
                        CategoryName = string.IsNullOrEmpty(g.Key) ? "Uncategorized" : g.Key,
                        ItemCount = g.Count(),
                        TotalQuantity = g.Sum(p => p.Inventory?.QuantityOnHand ?? 0)
                    }).ToList()
            };

            // Calculate restock budget (Target Stock - Current Stock) * Cost
            viewModel.ProjectedRestockBudget = inventories
                .Where(i => i.QuantityOnHand <= i.ReorderLevel)
                .Sum(i => ((i.ReorderLevel * 3) - i.QuantityOnHand) * (i.Product?.CostIncVat ?? 0));

            return viewModel;
        }
    }
}