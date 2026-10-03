/***************************************************************************************
*    Title: Forecasting Service Interface
*    Author: Maseeha17
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Services/Interfaces/IForecastingService.cs
***************************************************************************************/

using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    public interface IForecastingService
    {
        Task<ForecastFilterViewModel> GetFilteredForecastsAsync(string? category, string? statusFilter);
        Task<TriggerReorderViewModel?> GetReorderModelAsync(int productId);
        Task<bool> ProcessReorderAsync(TriggerReorderViewModel model);
        Task<byte[]> ExportForecastCsvAsync(string? category, string? statusFilter);
    }
}