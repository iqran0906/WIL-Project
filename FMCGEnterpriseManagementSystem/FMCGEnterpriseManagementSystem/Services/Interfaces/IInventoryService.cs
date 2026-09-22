using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    public interface IInventoryService
    {
        Task<IEnumerable<InventoryViewModel>> GetAllInventoryAsync();
        Task<InventoryViewModel?> GetByIdAsync(int id);
        Task CreateInventoryItemAsync(InventoryViewModel model);
        Task UpdateInventoryItemAsync(InventoryViewModel model);
        Task AdjustStockAsync(AdjustStockViewModel model);
    }
}