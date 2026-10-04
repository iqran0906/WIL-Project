// Title: Asynchronous programming with async and await
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using System.Globalization;
using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FMCGEnterpriseManagementSystem.Services
{
    // Service responsible for inventory-related business operations.
    // It coordinates repository operations, product updates,
    // stock adjustments, batches, and low-stock notifications.
    public class InventoryService : IInventoryService
    {
        // Repository used to perform CRUD operations on inventory records.
        private readonly IInventoryRepository _repository;

        // Database context used when inventory operations also require
        // direct access to products or stock batches.
        private readonly ApplicationDbContext _context;

        // Notification service used to notify the system when stock
        // reaches or falls below its configured reorder level.
        private readonly INotificationService _notificationService;

        // Constructor receives the required dependencies through
        // dependency injection.
        public InventoryService(
       IInventoryRepository repository,
       ApplicationDbContext context,
       INotificationService notificationService)
        {
            _repository = repository;
            _context = context;
            _notificationService = notificationService;
        }

        // Retrieves all inventory records from the repository
        // and converts each entity into an InventoryViewModel.
        public async Task<IEnumerable<InventoryViewModel>> GetAllInventoryAsync()
        {
            var items = await _repository.GetAllAsync();
            return items.Select(MapToViewModel);
        }

        // Retrieves a single inventory record by its ID.
        // Returns null when no matching record exists.
        public async Task<InventoryViewModel?> GetByIdAsync(int id)
        {
            var item = await _repository.GetByIdAsync(id);
            return item == null ? null : MapToViewModel(item);
        }

        // Creates a new inventory record for an existing product.
        public async Task CreateInventoryItemAsync(InventoryViewModel model)
        {
            // Get the existing product selected from the dropdown.
            // This ensures the inventory record references a valid product.
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == model.ProductId);

            // Stop the operation if the selected product does not exist.
            if (product == null)
            {
                throw new Exception("Selected product was not found.");
            }

            // Map the view model values into a new Inventory entity.
            var entity = new Inventory
            {
                ProductId = product.ProductId,
                QuantityOnHand = model.QuantityOnHand,
                ReorderLevel = model.ReorderLevel,
                Notes = model.Notes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Persist the new inventory record through the repository.
            await _repository.AddAsync(entity);

            // If an expiry date was chosen, create a linked StockBatch.
            if (model.ExpiryDate.HasValue)
            {
                // Creates a batch record containing the stock quantity
                // and expiry information for traceability.
                var stockBatch = new StockBatch
                {
                    InventoryId = entity.InventoryId,
                    BatchNumber = $"BATCH-{DateTime.UtcNow:yyyyMMddHHmmss}",
                    Quantity = model.QuantityOnHand,
                    ReceivedDate = DateTime.UtcNow,
                    ExpiryDate = model.ExpiryDate.Value,
                    CreatedAt = DateTime.UtcNow
                };

                // Adds the batch to the current database context.
                _context.StockBatches.Add(stockBatch);

                // Saves the newly created stock batch.
                await _context.SaveChangesAsync();
            }
        }

        // Updates the quantity, reorder level, notes, and associated
        // product information for an existing inventory item.
        public async Task UpdateInventoryItemAsync(InventoryViewModel model)
        {
            // Retrieves the inventory entity that is being edited.
            var entity = await _repository.GetByIdAsync(model.InventoryId);

            // If the inventory record does not exist, there is nothing to update.
            if (entity == null)
                return;

            // Update the inventory quantity and reorder threshold.
            entity.QuantityOnHand = model.QuantityOnHand;
            entity.ReorderLevel = model.ReorderLevel;
            entity.Notes = model.Notes;
            entity.UpdatedAt = DateTime.UtcNow;

            // Attempts to convert the selling price supplied by the view
            // into a decimal using invariant culture.
            decimal.TryParse(
                model.SellingPrice,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out var parsedSellingPrice
            );

            // Update related product information when the product navigation
            // property is available.
            if (entity.Product != null)
            {
                entity.Product.ProductCode =
                    model.ProductCode ?? entity.Product.ProductCode;

                entity.Product.ProductName =
                    model.ProductName ?? entity.Product.ProductName;

                entity.Product.Description =
     model.Description ?? entity.Product.Description;

                // Stores the parsed selling price on the product.
                entity.Product.SellingPrice = parsedSellingPrice;

                // Updates the product category when a value is supplied.
                entity.Product.Category =
                    model.CategoryName ?? entity.Product.Category;
            }

            // Persists the inventory changes through the repository.
            await _repository.UpdateAsync(entity);

            // Saves related product changes tracked by the DbContext.
            await _context.SaveChangesAsync();
        }

        // Deletes an inventory item and its associated product.
        public async Task DeleteInventoryItemAsync(int id)
        {
            // Retrieves the inventory entity before attempting deletion.
            var entity = await _repository.GetByIdAsync(id);

            // Only performs deletion when the inventory record exists.
            if (entity != null)
            {
                // Deletes the inventory record through the repository.
                await _repository.DeleteAsync(id);

                // If a related product exists, remove that product as well.
                if (entity.Product != null)
                {
                    _context.Products.Remove(entity.Product);

                    // Persist the product deletion.
                    await _context.SaveChangesAsync();
                }
            }
        }

        // Adjusts stock by either adding or deducting a specified quantity.
        public async Task AdjustStockAsync(AdjustStockViewModel model)
        {
            // Retrieves the inventory record being adjusted.
            var entity = await _repository.GetByIdAsync(model.InventoryId);

            // Stop if the inventory item cannot be found.
            if (entity == null)
                return;

            // Add stock to the current quantity when the adjustment type is "Add".
            if (model.AdjustmentType == "Add")
            {
                entity.QuantityOnHand += model.AdjustmentAmount;
            }

            // Deduct stock when the adjustment type is "Deduct".
            else if (model.AdjustmentType == "Deduct")
            {
                // Math.Max prevents the stock quantity from becoming negative.
                entity.QuantityOnHand = Math.Max(
                    0,
                    entity.QuantityOnHand - model.AdjustmentAmount
                );
            }

            // Records when the inventory was last modified.
            entity.UpdatedAt = DateTime.UtcNow;

            // Saves the adjusted inventory through the repository.
            await _repository.UpdateAsync(entity);

            // Check whether the remaining stock has reached or fallen below
            // the configured reorder level.
            if (entity.QuantityOnHand <= entity.ReorderLevel)
            {
                // Sends a low-stock notification containing the product name,
                // current quantity, and reorder threshold.
                await _notificationService.NotifyLowStockAsync(
                    entity.Product.ProductName,
                    entity.QuantityOnHand,
                    entity.ReorderLevel);
            }
        }

        // Converts an Inventory entity into the view model used by the UI.
        // This keeps database entities separate from presentation models.
        private static InventoryViewModel MapToViewModel(Inventory item) => new()
        {
            // Maps the inventory identifier.
            InventoryId = item.InventoryId,

            // Maps the associated product identifier.
            ProductId = item.ProductId,

            // Maps product information while providing empty strings
            // when the related product data is unavailable.
            ProductCode = item.Product?.ProductCode ?? string.Empty,
            ProductName = item.Product?.ProductName ?? string.Empty,
            Description = item.Product?.Description ?? string.Empty,
            CategoryName = item.Product?.Category ?? string.Empty,

            // Maps the current stock and reorder threshold.
            QuantityOnHand = item.QuantityOnHand,
            ReorderLevel = item.ReorderLevel,

            // Converts the selling price to a culture-independent string
            // for consistent display and processing.
            SellingPrice = item.Product?.SellingPrice
                .ToString(CultureInfo.InvariantCulture) ?? "0",

            // Counts the stock batches associated with the inventory item.
            BatchCount = item.StockBatches?.Count ?? 0,

            // Retrieves the earliest expiry date from the available batches.
            ExpiryDate = item.StockBatches?
                .OrderBy(b => b.ExpiryDate)
                .FirstOrDefault()?
                .ExpiryDate,

            // Maps any inventory notes.
            Notes = item.Notes
        };
    }
}
