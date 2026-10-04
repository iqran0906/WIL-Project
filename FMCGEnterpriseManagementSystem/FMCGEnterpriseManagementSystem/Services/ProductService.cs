// Title: Asynchronous programming with async and await
// Author: Maseeha17
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;
using System.Text.RegularExpressions;

namespace FMCGEnterpriseManagementSystem.Services
{
    // Service responsible for product-related business operations.
    public class ProductService : IProductService
    {
        // Repository used to access and modify product data.
        private readonly IProductRepository _repository;

        // Service used to retrieve system settings such as the VAT rate.
        private readonly ISettingsService _settingsService;

        // Initialises the product service with its required dependencies.
        public ProductService(IProductRepository repository, ISettingsService settingsService)
        {
            _repository = repository;
            _settingsService = settingsService;
        }

        // Retrieves all products and converts them into view models.
        public async Task<IEnumerable<ProductViewModel>> GetAllProductsAsync()
        {
            var list = await _repository.GetAllAsync();
            return list.Select(MapToViewModel);
        }

        // Retrieves a product by its ID and maps it to a view model.
        public async Task<ProductViewModel?> GetProductByIdAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            return product == null ? null : MapToViewModel(product);
        }

        // Creates a new product and automatically generates its product code.
        public async Task CreateProductAsync(ProductViewModel model)
        {
            // Retrieves existing products so the next available product number can be determined.
            var existingProducts = await _repository.GetAllAsync();

            int nextNumber = 1;

            // Extracts the numeric portions from existing ED product codes.
            var existingNumbers = existingProducts
                .Where(p => !string.IsNullOrEmpty(p.ProductCode) && p.ProductCode.StartsWith("ED", StringComparison.OrdinalIgnoreCase))
                .Select(p => {
                    var match = Regex.Match(p.ProductCode, @"\d+");
                    return match.Success && int.TryParse(match.Value, out int num) ? num : 0;
                })
                .ToList();

            // Starts the new product number after the highest existing product number.
            if (existingNumbers.Any())
            {
                nextNumber = existingNumbers.Max() + 1;
            }

            // Generates the product code using the ED prefix and four-digit numbering format.
            string generatedCode = $"ED{nextNumber:D4}";

            // Uses the supplied VAT-inclusive cost or calculates it from the VAT-exclusive cost.
            decimal costIncVat = model.CostIncVat > 0
                ? model.CostIncVat
                : Math.Round(model.CostExVat * (1 + await _settingsService.GetVatRateAsync()), 2);

            // Creates the product entity that will be stored in the database.
            var entity = new Product
            {
                SupplierId = model.SupplierId,
                ProductCode = generatedCode,
                ProductName = model.ProductName,
                Description = model.Description ?? string.Empty,
                Category = model.Category,
                CostExVat = model.CostExVat,
                CostIncVat = costIncVat,
                SellingPrice = model.SellingPrice,
                IsActive = model.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            // Adds the product to the repository and saves the changes.
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
        }

        // Updates an existing product using values supplied through the view model.
        public async Task UpdateProductAsync(ProductViewModel model)
        {
            // Retrieves the existing database entity before applying changes.
            var existingEntity = await _repository.GetByIdAsync(model.ProductId);
            if (existingEntity == null) return;

            // Uses the supplied VAT-inclusive cost or calculates it from the VAT-exclusive cost.
            decimal costIncVat = model.CostIncVat > 0
                ? model.CostIncVat
                : Math.Round(model.CostExVat * (1 + await _settingsService.GetVatRateAsync()), 2);

            // Updates the product properties.
            existingEntity.SupplierId = model.SupplierId;
            existingEntity.ProductName = model.ProductName;
            existingEntity.Description = model.Description ?? string.Empty;
            existingEntity.Category = model.Category;
            existingEntity.CostExVat = model.CostExVat;
            existingEntity.CostIncVat = costIncVat;
            existingEntity.SellingPrice = model.SellingPrice;
            existingEntity.IsActive = model.IsActive;
            existingEntity.UpdatedAt = DateTime.UtcNow;

            // Updates the product and persists the changes.
            await _repository.UpdateAsync(existingEntity);
            await _repository.SaveChangesAsync();
        }

        // Soft-deletes a product by marking it as inactive rather than removing it.
        public async Task DeleteProductAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);

            if (product == null)
                return;

            // Marks the product as inactive and records when the change occurred.
            product.IsActive = false;
            product.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(product);
            await _repository.SaveChangesAsync();
        }

        // Reactivates a previously deactivated product.
        public async Task ActivateProductAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);

            if (product == null)
                return;

            // Marks the product as active and records the update time.
            product.IsActive = true;
            product.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(product);
            await _repository.SaveChangesAsync();
        }

        // Converts a Product entity into a ProductViewModel for use by the presentation layer.
        private static ProductViewModel MapToViewModel(Product p) => new()
        {
            ProductId = p.ProductId,
            SupplierId = p.SupplierId,
            SupplierName = p.Supplier?.CompanyName ?? "Not Available",
            ProductCode = p.ProductCode,
            ProductName = p.ProductName,
            Description = p.Description,
            Category = p.Category,
            CostExVat = p.CostExVat,
            CostIncVat = p.CostIncVat,
            SellingPrice = p.SellingPrice,
            IsActive = p.IsActive,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        };
    }
}