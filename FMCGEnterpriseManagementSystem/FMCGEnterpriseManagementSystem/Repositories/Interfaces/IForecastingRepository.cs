using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.Repositories.Interfaces
{
    public interface IForecastingRepository
    {
        // Retrieves items that need restocking along with cost data
        Task<IEnumerable<Inventory>> GetLowStockForRestockForecastAsync();

        // Retrieves inventory with category data for turnover and demand projection
        Task<IEnumerable<Inventory>> GetInventoryForDemandAnalysisAsync();
    }
}