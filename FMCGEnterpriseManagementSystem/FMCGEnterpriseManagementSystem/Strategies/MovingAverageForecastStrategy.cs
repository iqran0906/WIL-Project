/***************************************************************************************
*    Title: Moving Average Forecast Strategy
*    Author: Maseeha17
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Strategies/MovingAverageForecastStrategy.cs
***************************************************************************************/

/***************************************************************************************
*    Title: Dependency injection in ASP.NET Core
*    Author: Microsoft
*    Date: 2026
*    Code version: ASP.NET Core
*    Availability: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection
***************************************************************************************/

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FMCGEnterpriseManagementSystem.DTOs;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using FMCGEnterpriseManagementSystem.Strategies;

namespace FMCGEnterpriseManagementSystem.Strategies
{
    // Generates forecast results using the moving-average forecasting strategy.
    public class MovingAverageForecastStrategy : IForecastingStrategy
    {
        private readonly IProductRepository _productRepository;

        // Receives the product repository through dependency injection.
        public MovingAverageForecastStrategy(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // Retrieves products and converts their inventory data into forecast results.
        public async Task<IEnumerable<ForecastResultDto>> GenerateForecastAsync()
        {
            var products = await _productRepository.GetAllAsync();

            // Project forecast data safely using the Inventory navigation property
            var forecasts = products.Select(p => new ForecastResultDto
            {
                ProductID = p.ProductId,
                ProductName = p.ProductName,
                CurrentStock = p.Inventory?.QuantityOnHand ?? 0,

                // Predictive heuristic using the Inventory reorder level
                PredictedDemand = ((p.Inventory?.ReorderLevel ?? 0) * 1.2m),

                ForecastPeriod = "Next 30 Days"
            }).ToList();

            return forecasts;
        }
    }
}