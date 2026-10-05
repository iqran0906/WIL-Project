// Title: Asynchronous programming with async and await
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    // Defines the contract for supplier-related business operations.
    public interface ISupplierService
    {
        // Retrieves all suppliers and returns them as supplier view models.
        Task<IEnumerable<SupplierViewModel>> GetAllSuppliersAsync();

        // Retrieves a supplier using its unique identifier.
        // Returns null when the requested supplier cannot be found.
        Task<SupplierViewModel?> GetSupplierByIdAsync(int id);

        // Creates a new supplier using the supplied view model.
        Task CreateSupplierAsync(SupplierViewModel model);

        // Updates an existing supplier using the supplied view model.
        Task UpdateSupplierAsync(SupplierViewModel model);

        // Deletes a supplier using its unique identifier.
        Task DeleteSupplierAsync(int id);

        // Activates a supplier account or record using its unique identifier.
        Task<bool> ActivateSupplierAsync(int id);

        // Deactivates a supplier account or record using its unique identifier.
        Task<bool> DeactivateSupplierAsync(int id);
    }
}