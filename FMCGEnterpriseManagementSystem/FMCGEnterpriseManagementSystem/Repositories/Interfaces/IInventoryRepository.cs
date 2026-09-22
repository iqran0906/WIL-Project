using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.Repositories.Interfaces
{
    public interface IInventoryRepository
    {
        Task<IEnumerable<Inventory>> GetAllAsync();
        Task<Inventory?> GetByIdAsync(int id);
        Task<Inventory?> GetByProductIdAsync(int productId);

        Task<bool> HasSufficientStockAsync(int productId, int quantity);
        Task DeductStockAsync(int productId, int quantity);

        Task AddAsync(Inventory item);
        Task UpdateAsync(Inventory item);
        Task DeleteAsync(int id);
    }
}