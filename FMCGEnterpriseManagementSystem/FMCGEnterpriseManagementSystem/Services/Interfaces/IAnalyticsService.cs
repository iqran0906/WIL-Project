// Title: Asynchronous programming with async and await
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using System.Threading.Tasks;
using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    // Defines the operations used to retrieve analytical information
    // about sales and inventory trends.
    public interface IAnalyticsService
    {
        // Retrieves sales and inventory trend data and returns it
        // in an AnalyticsViewModel for presentation in the application.
        Task<AnalyticsViewModel> GetSalesAndInventoryTrendsAsync();
    }
}