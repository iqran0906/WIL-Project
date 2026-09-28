// Purpose: Contract (interface) for inventory data access.
// Authors: Maseeha17, Sayali-St10458649 (from git history)

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