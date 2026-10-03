/***************************************************************************************
*    Title: Analytics Service Interface
*    Author: ST10068525, Maseeha17
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Services/Interfaces/IAnalyticsService.cs
***************************************************************************************/
using System.Threading.Tasks;
using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    public interface IAnalyticsService
    {
        Task<AnalyticsViewModel> GetSalesAndInventoryTrendsAsync();
    }
}