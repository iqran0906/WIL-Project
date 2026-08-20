using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<Product> GetByIdAsync(string productId);
        Task<Product> GetByCodeAsync(string productCode);
    }
}