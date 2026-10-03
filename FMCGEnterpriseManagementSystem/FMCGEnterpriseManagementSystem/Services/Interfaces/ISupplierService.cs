/***************************************************************************************
*    Title: Supplier Service Interface
*    Author: iqran0906, Naseeha27, Maseeha17
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Services/Interfaces/ISupplierService.cs
***************************************************************************************/

using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    public interface ISupplierService
    {
        Task<IEnumerable<SupplierViewModel>> GetAllSuppliersAsync();

        Task<SupplierViewModel?> GetSupplierByIdAsync(int id);

        Task CreateSupplierAsync(SupplierViewModel model);

        Task UpdateSupplierAsync(SupplierViewModel model);

        Task DeleteSupplierAsync(int id);
        Task<bool> ActivateSupplierAsync(int id);
        Task<bool> DeactivateSupplierAsync(int id);
    }
}