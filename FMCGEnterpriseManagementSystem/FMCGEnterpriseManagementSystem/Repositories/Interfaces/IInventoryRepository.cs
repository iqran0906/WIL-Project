using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.Repositories.Interfaces
{
    public interface IInventoryRepository
    {
        Task<Inventory> GetByProductIdAsync(string productId);
        Task<bool> HasSufficientStockAsync(string productId, int quantity);
        Task DeductStockAsync(string productId, int quantity);
    }
}