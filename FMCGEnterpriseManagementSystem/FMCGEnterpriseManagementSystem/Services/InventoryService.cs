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

        public InventoryService(IInventoryRepository repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
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
            int supplierIdToUse = model.SupplierId;

            if (supplierIdToUse > 0)
            {
                var existingSupplier = await _context.Suppliers.FindAsync(model.SupplierId);
                if (existingSupplier == null)
                {
                    supplierIdToUse = 0;
                }
            }

            if (supplierIdToUse == 0)
            {
                var defaultSupplier = await _context.Suppliers.FirstOrDefaultAsync();

                if (defaultSupplier == null)
                {
                    defaultSupplier = new Supplier
                    {
                        CompanyName = "Default Supplier",
                        ContactPerson = "Default Contact",
                        ContactNumber = "0000000000",
                        CreditTerms = "Net 30",
                        Email = "default@supplier.com",
                        Notes = "Auto-generated default supplier",
                        PhysicalAddress = "Default Address",
                        VATNumber = "VAT000000"
                    };
                    _context.Suppliers.Add(defaultSupplier);
                    await _context.SaveChangesAsync();
                }

                supplierIdToUse = defaultSupplier.SupplierId;
            }

            var product = new Product
            {
                ProductCode = model.ProductCode,
                ProductName = model.ProductName,
                Description = model.ProductName,
                SellingPrice = model.SellingPrice,
                Category = model.CategoryName,
                SupplierId = supplierIdToUse,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var entity = new Inventory
            {
                ProductId = product.ProductId,
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

            entity.QuantityOnHand = model.QuantityOnHand;
            entity.ReorderLevel = model.ReorderLevel;
            entity.UpdatedAt = DateTime.UtcNow;

            if (entity.Product != null)
            {
                entity.Product.ProductCode = model.ProductCode ?? entity.Product.ProductCode;
                entity.Product.ProductName = model.ProductName ?? entity.Product.ProductName;
                entity.Product.Description = model.ProductName ?? entity.Product.Description;
                entity.Product.SellingPrice = model.SellingPrice;
                entity.Product.Category = model.CategoryName ?? entity.Product.Category;
            }

            await _repository.UpdateAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteInventoryItemAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                // Delete the inventory record first to handle foreign key constraints cleanly
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
            CategoryName = item.Product?.Category ?? string.Empty,
            QuantityOnHand = item.QuantityOnHand,
            ReorderLevel = item.ReorderLevel,
            SellingPrice = item.Product?.SellingPrice ?? 0,
            BatchCount = item.StockBatches?.Count ?? 0
        };
    }
}