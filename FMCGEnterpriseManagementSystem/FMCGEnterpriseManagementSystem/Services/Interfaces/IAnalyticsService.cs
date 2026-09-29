using System.Threading.Tasks;
using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    public interface IAnalyticsService
    {
        Task<AnalyticsViewModel> GetSalesAndInventoryTrendsAsync();
    }
}