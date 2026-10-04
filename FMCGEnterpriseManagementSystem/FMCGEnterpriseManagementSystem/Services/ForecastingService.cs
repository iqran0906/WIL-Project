// Title: Asynchronous programming with async and await
// Author: Maseeha17
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using System.Text;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services
{
    // Service responsible for demand forecasting and inventory reorder calculations.
    public class ForecastingService : IForecastingService
    {
        // Repository used to retrieve and update inventory information.
        private readonly IInventoryRepository _inventoryRepository;

        // Initialises the forecasting service with the inventory repository dependency.
        public ForecastingService(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        // Retrieves inventory data and creates demand forecasts based on stock levels.
        public async Task<ForecastFilterViewModel> GetFilteredForecastsAsync(string? category, string? statusFilter)
        {
            // Retrieves all inventory records from the repository.
            var inventoryList = await _inventoryRepository.GetAllAsync();

            // Creates a collection to store the calculated demand forecasts.
            var forecasts = new List<DemandForecastViewModel>();

            // Processes each inventory item to calculate its forecast values.
            foreach (var item in inventoryList)
            {
                // Estimates monthly demand as three times the configured reorder level.
                int estimatedDemand = item.ReorderLevel * 3;

                // Calculates the recommended reorder quantity while preventing a negative value.
                int recommendedOrder = Math.Max(0, estimatedDemand - item.QuantityOnHand);

                // Determines the stock status based on the current quantity and reorder level.
                string status = item.QuantityOnHand switch
                {
                    // No stock available is considered critical.
                    0 => "Critical",

                    // Stock at or below the reorder level requires attention.
                    var q when q <= item.ReorderLevel => "Warning",

                    // Stock above the reorder level is considered optimal.
                    _ => "Optimal"
                };

                // Creates a forecast view model containing the calculated inventory information.
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
                    UnitPrice = item.Product?.SellingPrice ?? 0m
                });
            }

            // Creates a sorted list of unique product categories for filtering.
            var categories = forecasts.Select(f => f.Category).Distinct().OrderBy(c => c).ToList();

            // Filters the forecasts by category when a category has been selected.
            if (!string.IsNullOrEmpty(category))
            {
                forecasts = forecasts.Where(f => f.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Filters the forecasts by stock status when a status has been selected.
            if (!string.IsNullOrEmpty(statusFilter))
            {
                forecasts = forecasts.Where(f => f.ForecastStatus.Equals(statusFilter, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Returns the filtered forecasts together with the available categories and projected reorder cost.
            return new ForecastFilterViewModel
            {
                SelectedCategory = category,
                StatusFilter = statusFilter,
                Forecasts = forecasts,
                Categories = categories,
                TotalProjectedReorderCost = forecasts.Sum(f => f.EstimatedReorderCost)
            };
        }

        // Creates the model required to display and process a reorder for a specific product.
        public async Task<TriggerReorderViewModel?> GetReorderModelAsync(int productId)
        {
            // Retrieves inventory information using the product ID.
            var inventory = await _inventoryRepository.GetByProductIdAsync(productId);

            // Returns null when no inventory record exists for the requested product.
            if (inventory == null) return null;

            // Estimates demand using three times the configured reorder level.
            int estimatedDemand = inventory.ReorderLevel * 3;

            // Calculates the recommended reorder quantity and ensures at least one item is recommended.
            int recommendedOrder = Math.Max(1, estimatedDemand - inventory.QuantityOnHand);

            // Returns the product and reorder information required by the reorder view.
            return new TriggerReorderViewModel
            {
                ProductId = inventory.ProductId,
                ProductCode = inventory.Product?.ProductCode ?? string.Empty,
                ProductName = inventory.Product?.ProductName ?? string.Empty,
                ReorderQuantity = recommendedOrder
            };
        }

        // Processes a reorder by increasing the quantity currently held in inventory.
        public async Task<bool> ProcessReorderAsync(TriggerReorderViewModel model)
        {
            // Retrieves the inventory record associated with the selected product.
            var inventory = await _inventoryRepository.GetByProductIdAsync(model.ProductId);

            // Returns false when the product does not have an inventory record.
            if (inventory == null) return false;

            // Adds the reordered quantity to the current stock level.
            inventory.QuantityOnHand += model.ReorderQuantity;

            // Records the date and time when the inventory was updated.
            inventory.UpdatedAt = DateTime.UtcNow;

            // Updates the inventory record in the repository.
            await _inventoryRepository.UpdateAsync(inventory);

            // Indicates that the reorder was processed successfully.
            return true;
        }

        // Exports the filtered demand forecast information as a CSV file.
        public async Task<byte[]> ExportForecastCsvAsync(string? category, string? statusFilter)
        {
            // Retrieves the forecasts using the same category and status filters.
            var filterResult = await GetFilteredForecastsAsync(category, statusFilter);

            // Creates a StringBuilder to efficiently construct the CSV content.
            var builder = new StringBuilder();

            // Adds the CSV column headings.
            builder.AppendLine("Product Code,Product Name,Category,Current Stock,Reorder Level,Est Monthly Demand,Recommended Order,Status,Estimated Cost");

            // Adds each forecast as a row in the CSV output.
            foreach (var item in filterResult.Forecasts)
            {
                builder.AppendLine($"\"{item.ProductCode}\",\"{item.ProductName}\",\"{item.Category}\",{item.CurrentStock},{item.ReorderLevel},{item.EstimatedMonthlyDemand},{item.RecommendedReorderQuantity},\"{item.ForecastStatus}\",{item.EstimatedReorderCost}");
            }

            // Converts the generated CSV text into UTF-8 encoded bytes for download or storage.
            return Encoding.UTF8.GetBytes(builder.ToString());
        }
    }
}