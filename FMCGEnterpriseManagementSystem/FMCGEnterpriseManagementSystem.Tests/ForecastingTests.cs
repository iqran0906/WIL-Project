using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using FMCGEnterpriseManagementSystem.Services;
using Moq;

namespace FMCGEnterpriseManagementSystem.Tests
{
    public class ForecastingTests
    {
        [Fact]
        public async Task LowStock_ShouldGiveReorderQuantity()
        {
            // Arrange
            var inventoryRepository = new Mock<IInventoryRepository>();

            var inventory = new Inventory
            {
                InventoryId = 1,
                ProductId = 1,
                QuantityOnHand = 10,
                ReorderLevel = 20,
                Product = new Product
                {
                    ProductId = 1,
                    ProductCode = "P001",
                    ProductName = "Test Product",
                    Category = "Test",
                    SellingPrice = 100m
                }
            };

            inventoryRepository
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Inventory> { inventory });

            var forecastingService =
                new ForecastingService(inventoryRepository.Object);

            // Act
            var result =
                await forecastingService.GetFilteredForecastsAsync(
                    null,
                    null);

            // Assert
            var forecast = result.Forecasts.First();

            Assert.Equal(
                60,
                forecast.EstimatedMonthlyDemand);

            Assert.Equal(
                50,
                forecast.RecommendedReorderQuantity);
        }

        [Fact]
        public async Task AdequateStock_ShouldGiveNoReorder()
        {
            // Arrange
            var inventoryRepository = new Mock<IInventoryRepository>();

            var inventory = new Inventory
            {
                InventoryId = 2,
                ProductId = 2,
                QuantityOnHand = 100,
                ReorderLevel = 20,
                Product = new Product
                {
                    ProductId = 2,
                    ProductCode = "P002",
                    ProductName = "Test Product 2",
                    Category = "Test",
                    SellingPrice = 100m
                }
            };

            inventoryRepository
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Inventory> { inventory });

            var forecastingService =
                new ForecastingService(inventoryRepository.Object);

            // Act
            var result =
                await forecastingService.GetFilteredForecastsAsync(
                    null,
                    null);

            // Assert
            var forecast = result.Forecasts.First();

            Assert.Equal(
                60,
                forecast.EstimatedMonthlyDemand);

            Assert.Equal(
                0,
                forecast.RecommendedReorderQuantity);
        }

        [Fact]
        public async Task RecommendedQuantity_ShouldEqualDemandMinusAvailableStock()
        {
            // Arrange
            var inventoryRepository = new Mock<IInventoryRepository>();

            var inventory = new Inventory
            {
                InventoryId = 3,
                ProductId = 3,
                QuantityOnHand = 20,
                ReorderLevel = 30,
                Product = new Product
                {
                    ProductId = 3,
                    ProductCode = "P003",
                    ProductName = "Test Product 3",
                    Category = "Test",
                    SellingPrice = 100m
                }
            };

            inventoryRepository
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Inventory> { inventory });

            var forecastingService =
                new ForecastingService(inventoryRepository.Object);

            // Act
            var result =
                await forecastingService.GetFilteredForecastsAsync(
                    null,
                    null);

            // Assert
            var forecast = result.Forecasts.First();

            Assert.Equal(
                90,
                forecast.EstimatedMonthlyDemand);

            Assert.Equal(
                70,
                forecast.RecommendedReorderQuantity);
        }
    }
}