// Title: Asynchronous programming with async and await
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    // Defines the operations used for product forecasting
    // and inventory reorder management.
    public interface IForecastingService
    {
        // Retrieves forecasts based on the selected product category
        // and forecast status.
        Task<ForecastFilterViewModel> GetFilteredForecastsAsync(string? category, string? statusFilter);

        // Retrieves the information required to display
        // the reorder form for a specific product.
        Task<TriggerReorderViewModel?> GetReorderModelAsync(int productId);

        // Processes a stock reorder using the supplied reorder information.
        // Returns whether the operation was successful.
        Task<bool> ProcessReorderAsync(TriggerReorderViewModel model);

        // Generates forecast information in CSV format for export.
        // Returns the generated file as a byte array.
        Task<byte[]> ExportForecastCsvAsync(string? category, string? statusFilter);
    }
}