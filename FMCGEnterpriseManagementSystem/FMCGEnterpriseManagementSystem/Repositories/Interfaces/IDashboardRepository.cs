/***************************************************************************************
*    Title: Dashboard Repository Interface
*    Author: Maseeha17
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Repositories/Interfaces/IDashboardRepository.cs
***************************************************************************************/
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