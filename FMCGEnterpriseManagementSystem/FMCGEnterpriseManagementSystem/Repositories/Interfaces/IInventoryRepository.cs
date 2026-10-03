/***************************************************************************************
*    Title: Inventory Repository Interface
*    Author: Maseeha17, Sayali-St10458649
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Repositories/Interfaces/IInventoryRepository.cs
***************************************************************************************/

using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.Repositories.Interfaces
{
    public interface IInventoryRepository
    {
        Task<IEnumerable<Inventory>> GetAllAsync();
        Task<Inventory?> GetByIdAsync(int id);
        Task<Inventory?> GetByProductIdAsync(int productId);

        Task AddAsync(Inventory inventory);
        Task UpdateAsync(Inventory inventory);
        Task<bool> HasSufficientStockAsync(int productId, int quantity);
        Task DeductStockAsync(int productId, int quantity);
        Task DeleteAsync(int id);
    }
}