using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    public interface ISupplierService
    {
        Task<IEnumerable<SupplierViewModel>> GetAllSuppliersAsync();
        Task<SupplierViewModel?> GetSupplierByIdAsync(string id);
        Task CreateSupplierAsync(SupplierViewModel model);
        Task UpdateSupplierAsync(SupplierViewModel model);
        Task DeleteSupplierAsync(string id);
    }
}