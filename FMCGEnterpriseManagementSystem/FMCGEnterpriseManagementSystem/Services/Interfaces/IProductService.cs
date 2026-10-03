/***************************************************************************************
*    Title: Product Service Interface
*    Author: iqran0906, Maseeha17
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Services/Interfaces/IProductService.cs
***************************************************************************************/

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