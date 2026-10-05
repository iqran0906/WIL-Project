// Title: Asynchronous programming with async and await
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using System.Threading.Tasks;
using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    // Defines the operations used to retrieve dashboard analytics
    // for display in the application dashboard.
    public interface IDashboardService
    {
        // Retrieves dashboard analytics for the specified number of months.
        // The default period is 12 months.
        Task<DashboardViewModel> GetDashboardAnalyticsAsync(int months = 12);
    }
}