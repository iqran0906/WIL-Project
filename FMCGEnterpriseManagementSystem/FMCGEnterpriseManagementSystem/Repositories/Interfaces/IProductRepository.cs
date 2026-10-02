// Purpose: Contract (interface) for product data access.
// Authors: iqran0906, Maseeha17, Sayali-St10458649 (from git history)

using System.Collections.Generic;
using System.Threading.Tasks;
using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task<Product?> GetByCodeAsync(string productCode);

        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
        Task<bool> SaveChangesAsync();
    }
}