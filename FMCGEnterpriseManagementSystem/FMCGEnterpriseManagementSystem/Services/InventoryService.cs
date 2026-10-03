/***************************************************************************************
*    Title: Inventory Service
*    Author: Maseeha17
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Services/InventoryService.cs
***************************************************************************************/
using System.Globalization;
using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FMCGEnterpriseManagementSystem.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _repository;
        private readonly ApplicationDbContext _context;

        private readonly INotificationService _notificationService;

        public InventoryService(
       IInventoryRepository repository,
       ApplicationDbContext context,
       INotificationService notificationService)
        {
            _repository = repository;
            _context = context;
            _notificationService = notificationService;
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
            // Get the existing product selected from the dropdown
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == model.ProductId);

            if (product == null)
            {
                throw new Exception("Selected product was not found.");
            }

            var entity = new Inventory
            {
                ProductId = product.ProductId,
                QuantityOnHand = model.QuantityOnHand,
                ReorderLevel = model.ReorderLevel,
                Notes = model.Notes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(entity);

            // If an expiry date was chosen, create a linked StockBatch
            if (model.ExpiryDate.HasValue)
            {
                var stockBatch = new StockBatch
                {
                    InventoryId = entity.InventoryId,
                    BatchNumber = $"BATCH-{DateTime.UtcNow:yyyyMMddHHmmss}",
                    Quantity = model.QuantityOnHand,
                    ReceivedDate = DateTime.UtcNow,
                    ExpiryDate = model.ExpiryDate.Value,
                    CreatedAt = DateTime.UtcNow
                };

                _context.StockBatches.Add(stockBatch);
                await _context.SaveChangesAsync();
            }
        }
        public async Task UpdateInventoryItemAsync(InventoryViewModel model)
        {
            var entity = await _repository.GetByIdAsync(model.InventoryId);

            if (entity == null)
                return;

            entity.QuantityOnHand = model.QuantityOnHand;
            entity.ReorderLevel = model.ReorderLevel;
            entity.Notes = model.Notes;
            entity.UpdatedAt = DateTime.UtcNow;

            decimal.TryParse(
                model.SellingPrice,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out var parsedSellingPrice
            );

            if (entity.Product != null)
            {
                entity.Product.ProductCode =
                    model.ProductCode ?? entity.Product.ProductCode;

                entity.Product.ProductName =
                    model.ProductName ?? entity.Product.ProductName;

                entity.Product.Description =
     model.Description ?? entity.Product.Description;

                entity.Product.SellingPrice = parsedSellingPrice;

                entity.Product.Category =
                    model.CategoryName ?? entity.Product.Category;
            }

            await _repository.UpdateAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteInventoryItemAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);

            if (entity != null)
            {
                await _repository.DeleteAsync(id);

                if (entity.Product != null)
                {
                    _context.Products.Remove(entity.Product);
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task AdjustStockAsync(AdjustStockViewModel model)
        {
            var entity = await _repository.GetByIdAsync(model.InventoryId);

            if (entity == null)
                return;

            if (model.AdjustmentType == "Add")
            {
                entity.QuantityOnHand += model.AdjustmentAmount;
            }
            else if (model.AdjustmentType == "Deduct")
            {
                entity.QuantityOnHand = Math.Max(
                    0,
                    entity.QuantityOnHand - model.AdjustmentAmount
                );
            }

            entity.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(entity);

            if (entity.QuantityOnHand <= entity.ReorderLevel)
            {
                await _notificationService.NotifyLowStockAsync(
                    entity.Product.ProductName,
                    entity.QuantityOnHand,
                    entity.ReorderLevel);
            }
        }

        private static InventoryViewModel MapToViewModel(Inventory item) => new()
        {
            InventoryId = item.InventoryId,
            ProductId = item.ProductId,
            ProductCode = item.Product?.ProductCode ?? string.Empty,
            ProductName = item.Product?.ProductName ?? string.Empty,
            Description = item.Product?.Description ?? string.Empty,
            CategoryName = item.Product?.Category ?? string.Empty,
            QuantityOnHand = item.QuantityOnHand,
            ReorderLevel = item.ReorderLevel,
            SellingPrice = item.Product?.SellingPrice
                .ToString(CultureInfo.InvariantCulture) ?? "0",

            BatchCount = item.StockBatches?.Count ?? 0,

            ExpiryDate = item.StockBatches?
                .OrderBy(b => b.ExpiryDate)
                .FirstOrDefault()?
                .ExpiryDate,

            Notes = item.Notes
        };
    }
}