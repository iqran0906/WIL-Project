using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _repository;

        public InventoryService(IInventoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<InventoryViewModel>> GetAllInventoryAsync()
        {
            var items = await _repository.GetAllAsync();
            return items.Select(MapToViewModel);
        }

        public async Task<InventoryViewModel?> GetByIdAsync(int id)
        {
            var item = await _repository.GetByIdAsync(id);
            return item == null ? null : MapToViewModel(item);
        }

        public async Task CreateInventoryItemAsync(InventoryViewModel model)
        {
            var entity = new Inventory
            {
                ProductId = model.ProductId,
                QuantityOnHand = model.QuantityOnHand,
                ReorderLevel = model.ReorderLevel,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(entity);
        }

        public async Task UpdateInventoryItemAsync(InventoryViewModel model)
        {
            var entity = await _repository.GetByIdAsync(model.InventoryId);
            if (entity == null) return;

            entity.ProductId = model.ProductId;
            entity.QuantityOnHand = model.QuantityOnHand;
            entity.ReorderLevel = model.ReorderLevel;
            entity.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(entity);
        }

        public async Task AdjustStockAsync(AdjustStockViewModel model)
        {
            var entity = await _repository.GetByIdAsync(model.InventoryId);
            if (entity == null) return;

            if (model.AdjustmentType == "Add")
            {
                entity.QuantityOnHand += model.AdjustmentAmount;
            }
            else if (model.AdjustmentType == "Deduct")
            {
                entity.QuantityOnHand = Math.Max(0, entity.QuantityOnHand - model.AdjustmentAmount);
            }

            entity.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(entity);
        }

        private static InventoryViewModel MapToViewModel(Inventory item) => new()
        {
            InventoryId = item.InventoryId,
            ProductId = item.ProductId,
            ProductCode = item.Product?.ProductCode ?? string.Empty,
            ProductName = item.Product?.ProductName ?? string.Empty,
            QuantityOnHand = item.QuantityOnHand,
            ReorderLevel = item.ReorderLevel,
            BatchCount = item.StockBatches?.Count ?? 0
        };
    }
}