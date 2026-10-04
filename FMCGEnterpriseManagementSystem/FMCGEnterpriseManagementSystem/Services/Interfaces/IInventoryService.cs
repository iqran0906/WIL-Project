// Title: Asynchronous programming with async and await
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    // Defines the operations used to manage inventory records and stock levels.
    public interface IInventoryService
    {
        // Retrieves all inventory records for display in the system.
        Task<IEnumerable<InventoryViewModel>> GetAllInventoryAsync();

        // Retrieves a specific inventory record using its ID.
        // Returns null when the inventory record cannot be found.
        Task<InventoryViewModel?> GetByIdAsync(int id);

        // Creates a new inventory item using the supplied view model.
        Task CreateInventoryItemAsync(InventoryViewModel model);

        // Updates an existing inventory item using the supplied view model.
        Task UpdateInventoryItemAsync(InventoryViewModel model);

        // Deletes an inventory item using its ID.
        Task DeleteInventoryItemAsync(int id); // Add this line here!

        // Adjusts the stock quantity using the supplied stock adjustment information.
        Task AdjustStockAsync(AdjustStockViewModel model);
    }
}