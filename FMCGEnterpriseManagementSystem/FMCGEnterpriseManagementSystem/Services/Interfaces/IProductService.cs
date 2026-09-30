// Purpose: Contract (interface) for the product service.
// Authors: iqran0906, Maseeha17 (from git history)

using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductViewModel>> GetAllProductsAsync();
        Task<ProductViewModel?> GetProductByIdAsync(int id);
        Task CreateProductAsync(ProductViewModel model);
        Task UpdateProductAsync(ProductViewModel model);
        Task DeleteProductAsync(int id);
        Task ActivateProductAsync(int id);
    }
}