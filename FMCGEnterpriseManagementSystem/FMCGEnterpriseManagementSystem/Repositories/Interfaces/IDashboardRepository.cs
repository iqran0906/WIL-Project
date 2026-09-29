using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.Repositories.Interfaces
{
    public interface IDashboardRepository
    {
        Task<int> GetTotalProductsCountAsync();
        Task<int> GetTotalSuppliersCountAsync();
        Task<int> GetTotalCustomersCountAsync();
        Task<decimal> GetTotalInventoryValueAsync();
        Task<IEnumerable<Inventory>> GetLowStockInventoriesAsync();
        Task<Dictionary<string, (int ItemCount, int TotalQty)>> GetCategoryStockSummariesAsync();
    }
}