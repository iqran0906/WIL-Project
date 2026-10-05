using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using FMCGEnterpriseManagementSystem.Services;
using FMCGEnterpriseManagementSystem.ViewModels;
using Moq;

namespace FMCGEnterpriseManagementSystem.Tests
{
    public class SalesRepresentativeTests
    {
        [Fact]
        public async Task NewSalesRepresentative_ShouldGenerateSRCode()
        {
            // Arrange
            var salesRepRepository =
                new Mock<ISalesRepresentativeRepository>();

            salesRepRepository
                .Setup(r => r.GetByEmployeeIdAsync("EMP001"))
                .ReturnsAsync((SalesRepresentative?)null);

            salesRepRepository
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<SalesRepresentative>());

            salesRepRepository
                .Setup(r => r.SalesRepCodeExistsAsync(
                    It.IsAny<string>(),
                    It.IsAny<int?>()))
                .ReturnsAsync(false);

            var salesRepService =
                new SalesRepresentativeService(
                    salesRepRepository.Object);

            var model = new SalesRepresentativeViewModel
            {
                EmployeeID = "EMP001",
                Area = "Durban",
                Salary = 15000m,
                CommissionRate = 5m,
                SalesTarget = 100000m
            };

            // Act
            var result =
                await salesRepService.CreateAsync(model);

            // Assert
            Assert.True(result);

            salesRepRepository.Verify(
                r => r.AddAsync(
                    It.Is<SalesRepresentative>(
                        sr => sr.SalesRepCode == "SR-001")),
                Times.Once);
        }

        [Fact]
        public async Task ExistingSalesRepCodes_ShouldGenerateNextAvailableCode()
        {
            // Arrange
            var salesRepRepository =
                new Mock<ISalesRepresentativeRepository>();

            var existingSalesReps =
                new List<SalesRepresentative>
                {
                    new SalesRepresentative
                    {
                        SalesRepresentativeId = 1,
                        EmployeeID = "EMP001",
                        SalesRepCode = "SR-001"
                    },
                    new SalesRepresentative
                    {
                        SalesRepresentativeId = 2,
                        EmployeeID = "EMP002",
                        SalesRepCode = "SR-002"
                    }
                };

            salesRepRepository
                .Setup(r => r.GetByEmployeeIdAsync("EMP003"))
                .ReturnsAsync((SalesRepresentative?)null);

            salesRepRepository
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(existingSalesReps);

            salesRepRepository
                .Setup(r => r.SalesRepCodeExistsAsync(
                    It.IsAny<string>(),
                    It.IsAny<int?>()))
                .ReturnsAsync(false);

            var salesRepService =
                new SalesRepresentativeService(
                    salesRepRepository.Object);

            var model = new SalesRepresentativeViewModel
            {
                EmployeeID = "EMP003",
                Area = "Durban",
                Salary = 15000m,
                CommissionRate = 5m,
                SalesTarget = 100000m
            };

            // Act
            var result =
                await salesRepService.CreateAsync(model);

            // Assert
            Assert.True(result);

            salesRepRepository.Verify(
                r => r.AddAsync(
                    It.Is<SalesRepresentative>(
                        sr => sr.SalesRepCode == "SR-003")),
                Times.Once);
        }
    }
}