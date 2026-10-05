// Title: Asynchronous programming with async and await
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.Repositories.Interfaces
{
    // Purpose: Defines database operations used to retrieve inventory data for forecasting.
    public interface IForecastingRepository
    {
        // Retrieves low-stock items together with cost information for restocking forecasts.
        Task<IEnumerable<Inventory>> GetLowStockForRestockForecastAsync();

        // Retrieves inventory and category information for turnover and demand analysis.
        Task<IEnumerable<Inventory>> GetInventoryForDemandAnalysisAsync();
    }
}