using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;
using System.Text.RegularExpressions;

namespace FMCGEnterpriseManagementSystem.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private const decimal VatRate = 0.15m;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ProductViewModel>> GetAllProductsAsync()
        {
            var list = await _repository.GetAllAsync();
            return list.Select(MapToViewModel);
        }

        public async Task<ProductViewModel?> GetProductByIdAsync(string id)
        {
            var product = await _repository.GetByIdAsync(id);
            return product == null ? null : MapToViewModel(product);
        }

        public async Task CreateProductAsync(ProductViewModel model)
        {
            var existingProducts = await _repository.GetAllAsync();

            int nextNumber = 1;
            var existingNumbers = existingProducts
                .Where(p => !string.IsNullOrEmpty(p.ProductCode) && p.ProductCode.StartsWith("ED", StringComparison.OrdinalIgnoreCase))
                .Select(p => {
                    var match = Regex.Match(p.ProductCode, @"\d+");
                    return match.Success && int.TryParse(match.Value, out int num) ? num : 0;
                })
                .ToList();

            if (existingNumbers.Any())
            {
                nextNumber = existingNumbers.Max() + 1;
            }

            string generatedCode = $"ED{nextNumber:D4}";

            decimal costIncVat = model.CostIncVat > 0
                ? model.CostIncVat
                : Math.Round(model.CostExVat * (1 + VatRate), 2);

            var entity = new Product
            {
                ProductId = string.IsNullOrEmpty(model.ProductId) ? Guid.NewGuid().ToString() : model.ProductId,
                SupplierId = model.SupplierId,
                ProductCode = generatedCode,
                ProductName = model.ProductName,
                Description = model.Description ?? string.Empty,
                Category = model.Category,
                CostExVat = model.CostExVat,
                CostIncVat = costIncVat,
                SellingPrice = model.SellingPrice
            };

            await _repository.AddAsync(entity);
        }

        public async Task UpdateProductAsync(ProductViewModel model)
        {
            decimal costIncVat = model.CostIncVat > 0
                ? model.CostIncVat
                : Math.Round(model.CostExVat * (1 + VatRate), 2);

            var entity = new Product
            {
                ProductId = model.ProductId,
                SupplierId = model.SupplierId,
                ProductCode = model.ProductCode ?? string.Empty,
                ProductName = model.ProductName,
                Description = model.Description ?? string.Empty,
                Category = model.Category,
                CostExVat = model.CostExVat,
                CostIncVat = costIncVat,
                SellingPrice = model.SellingPrice
            };

            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteProductAsync(string id)
        {
            await _repository.DeleteAsync(id);
        }

        private static ProductViewModel MapToViewModel(Product p) => new()
        {
            ProductId = p.ProductId,
            SupplierId = p.SupplierId,
            ProductCode = p.ProductCode,
            ProductName = p.ProductName,
            Description = p.Description,
            Category = p.Category,
            CostExVat = p.CostExVat,
            CostIncVat = p.CostIncVat,
            SellingPrice = p.SellingPrice
        };
    }
}