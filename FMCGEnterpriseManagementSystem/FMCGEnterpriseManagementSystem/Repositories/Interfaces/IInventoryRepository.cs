using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.Repositories.Interfaces
{
    public interface IInventoryRepository
    {
        Task<Inventory> GetByProductIdAsync(int productId);
        Task<bool> HasSufficientStockAsync(int productId, int quantity);
        Task DeductStockAsync(int productId, int quantity);
    }
}