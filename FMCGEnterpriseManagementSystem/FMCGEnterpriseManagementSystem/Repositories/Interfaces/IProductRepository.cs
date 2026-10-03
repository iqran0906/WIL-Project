/***************************************************************************************
*    Title: Product Repository Interface
*    Author: iqran0906, Maseeha17, Sayali-St10458649
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Repositories/Interfaces/IProductRepository.cs
***************************************************************************************/

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