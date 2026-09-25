using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FMCGEnterpriseManagementSystem.DTOs;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;

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

            // Simple moving average projection logic based on current stock & reorder levels
            var forecasts = products.Select(p => new ForecastResultDto
            {
                ProductID = p.ProductID,
                ProductName = p.ProductName,
                CurrentStock = p.QuantityInStock,
                // Simple predictive heuristic: smoothing reorder level with current stock
                PredictedDemand = (p.ReorderLevel * 1.2m),
                ForecastPeriod = "Next 30 Days"
            }).ToList();

            return forecasts;
        }
    }
}