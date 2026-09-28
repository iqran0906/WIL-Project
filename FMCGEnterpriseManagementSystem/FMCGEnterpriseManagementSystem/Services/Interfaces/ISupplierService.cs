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