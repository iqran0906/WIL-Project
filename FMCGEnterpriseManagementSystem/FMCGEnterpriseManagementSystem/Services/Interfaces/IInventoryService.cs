// Purpose: Contract (interface) for the inventory service.
// Authors: Maseeha17 (from git history)

using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    public interface IInventoryService
    {
        Task<IEnumerable<InventoryViewModel>> GetAllInventoryAsync();
        Task<InventoryViewModel?> GetByIdAsync(int id);
        Task CreateInventoryItemAsync(InventoryViewModel model);
        Task UpdateInventoryItemAsync(InventoryViewModel model);
        Task DeleteInventoryItemAsync(int id); // Add this line here!
        Task AdjustStockAsync(AdjustStockViewModel model);
    }
}