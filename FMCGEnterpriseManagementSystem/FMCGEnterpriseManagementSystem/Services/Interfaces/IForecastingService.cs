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