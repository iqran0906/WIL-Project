using System.Text;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services
{
    public class ForecastingService : IForecastingService
    {
        private readonly IInventoryRepository _inventoryRepository;

        public ForecastingService(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        public async Task<ForecastFilterViewModel> GetFilteredForecastsAsync(string? category, string? statusFilter)
        {
            var inventoryList = await _inventoryRepository.GetAllAsync();
            var forecasts = new List<DemandForecastViewModel>();

            foreach (var item in inventoryList)
            {
                int estimatedDemand = item.ReorderLevel * 3;
                int recommendedOrder = Math.Max(0, estimatedDemand - item.QuantityOnHand);

                string status = item.QuantityOnHand switch
                {
                    0 => "Critical",
                    var q when q <= item.ReorderLevel => "Warning",
                    _ => "Optimal"
                };

                forecasts.Add(new DemandForecastViewModel
                {
                    ProductId = item.ProductId,
                    ProductCode = item.Product?.ProductCode ?? string.Empty,
                    ProductName = item.Product?.ProductName ?? string.Empty,
                    Category = item.Product?.Category ?? "Uncategorized",
                    CurrentStock = item.QuantityOnHand,
                    ReorderLevel = item.ReorderLevel,
                    EstimatedMonthlyDemand = estimatedDemand,
                    RecommendedReorderQuantity = recommendedOrder,
                    ForecastStatus = status,
                    UnitPrice = item.Product?.UnitPrice ?? 0m
                });
            }

            var categories = forecasts.Select(f => f.Category).Distinct().OrderBy(c => c).ToList();

            if (!string.IsNullOrEmpty(category))
            {
                forecasts = forecasts.Where(f => f.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                forecasts = forecasts.Where(f => f.ForecastStatus.Equals(statusFilter, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return new ForecastFilterViewModel
            {
                SelectedCategory = category,
                StatusFilter = statusFilter,
                Forecasts = forecasts,
                Categories = categories,
                TotalProjectedReorderCost = forecasts.Sum(f => f.EstimatedReorderCost)
            };
        }

        public async Task<TriggerReorderViewModel?> GetReorderModelAsync(int productId)
        {
            var inventory = await _inventoryRepository.GetByProductIdAsync(productId);
            if (inventory == null) return null;

            int estimatedDemand = inventory.ReorderLevel * 3;
            int recommendedOrder = Math.Max(1, estimatedDemand - inventory.QuantityOnHand);

            return new TriggerReorderViewModel
            {
                ProductId = inventory.ProductId,
                ProductCode = inventory.Product?.ProductCode ?? string.Empty,
                ProductName = inventory.Product?.ProductName ?? string.Empty,
                ReorderQuantity = recommendedOrder
            };
        }

        public async Task<bool> ProcessReorderAsync(TriggerReorderViewModel model)
        {
            var inventory = await _inventoryRepository.GetByProductIdAsync(model.ProductId);
            if (inventory == null) return false;

            inventory.QuantityOnHand += model.ReorderQuantity;
            inventory.UpdatedAt = DateTime.UtcNow;

            await _inventoryRepository.UpdateAsync(inventory);
            return true;
        }

        public async Task<byte[]> ExportForecastCsvAsync(string? category, string? statusFilter)
        {
            var filterResult = await GetFilteredForecastsAsync(category, statusFilter);
            var builder = new StringBuilder();

            builder.AppendLine("Product Code,Product Name,Category,Current Stock,Reorder Level,Est Monthly Demand,Recommended Order,Status,Estimated Cost");

            foreach (var item in filterResult.Forecasts)
            {
                builder.AppendLine($"\"{item.ProductCode}\",\"{item.ProductName}\",\"{item.Category}\",{item.CurrentStock},{item.ReorderLevel},{item.EstimatedMonthlyDemand},{item.RecommendedReorderQuantity},\"{item.ForecastStatus}\",{item.EstimatedReorderCost}");
            }

            return Encoding.UTF8.GetBytes(builder.ToString());
        }
    }
}