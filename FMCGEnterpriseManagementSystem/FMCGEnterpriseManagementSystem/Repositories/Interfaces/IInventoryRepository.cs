using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.Repositories.Interfaces
{
    public interface IInventoryRepository
    {
        Task<IEnumerable<Inventory>> GetAllAsync();
        Task<Inventory?> GetByIdAsync(int id);
        Task<Inventory?> GetByProductIdAsync(int productId);
        Task AddAsync(Inventory item);
        Task UpdateAsync(Inventory item);
        Task DeleteAsync(int id);
    }
}