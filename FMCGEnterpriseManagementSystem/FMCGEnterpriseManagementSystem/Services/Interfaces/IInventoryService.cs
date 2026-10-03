/***************************************************************************************
*    Title: Inventory Service Interface
*    Author: Maseeha17
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Services/Interfaces/IInventoryService.cs
***************************************************************************************/

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