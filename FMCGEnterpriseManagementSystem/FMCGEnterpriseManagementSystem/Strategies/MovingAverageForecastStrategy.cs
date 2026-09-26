using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FMCGEnterpriseManagementSystem.DTOs;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using FMCGEnterpriseManagementSystem.Strategies;

namespace FMCGEnterpriseManagementSystem.Strategies
{
    public class MovingAverageForecastStrategy : IForecastingStrategy
    {
        private readonly IProductRepository _productRepository;

        public MovingAverageForecastStrategy(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<ForecastResultDto>> GenerateForecastAsync()
        {
            var products = await _productRepository.GetAllAsync();

            // Project forecast data safely using the Inventory navigation property
            var forecasts = products.Select(p => new ForecastResultDto
            {
                ProductID = p.ProductId,
                ProductName = p.ProductName,
                CurrentStock = p.Inventory?.QuantityInStock ?? 0,
                // Predictive heuristic using the Inventory reorder level
                PredictedDemand = ((p.Inventory?.ReorderLevel ?? 0) * 1.2m),
                ForecastPeriod = "Next 30 Days"
            }).ToList();

            return forecasts;
        }
    }
}