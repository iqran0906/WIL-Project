// Title: Grouping Data: LINQ
// Author: Maseeha17
// Date: 31-05-2024
// Code version: .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/linq/standard-query-operators/grouping-data

using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.Repositories.Interfaces
{
    // Purpose: Defines database operations used to retrieve dashboard statistics and summaries.
    public interface IDashboardRepository
    {
        // Gets the total number of products.
        Task<int> GetTotalProductsCountAsync();

        // Gets the total number of suppliers.
        Task<int> GetTotalSuppliersCountAsync();

        // Gets the total number of customers.
        Task<int> GetTotalCustomersCountAsync();

        // Calculates the total value of inventory currently held.
        Task<decimal> GetTotalInventoryValueAsync();

        // Retrieves inventory items that have reached their low-stock level.
        Task<IEnumerable<Inventory>> GetLowStockInventoriesAsync();

        // Retrieves stock summaries grouped by product category.
        Task<Dictionary<string, (int ItemCount, int TotalQty)>> GetCategoryStockSummariesAsync();
    }
}
