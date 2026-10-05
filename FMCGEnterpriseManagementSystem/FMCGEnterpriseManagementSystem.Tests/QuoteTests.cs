using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using FMCGEnterpriseManagementSystem.Services;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using Moq;

namespace FMCGEnterpriseManagementSystem.Tests
{
    public class QuoteTests
    {
        [Fact]
        public async Task StandardVat_ShouldCalculate15Percent()
        {
            // Arrange
            var quoteRepository = new Mock<IQuoteRepository>();
            var invoiceRepository = new Mock<IInvoiceRepository>();
            var notificationService = new Mock<INotificationService>();

            quoteRepository
                .Setup(r => r.GenerateNextQuoteNumberAsync("ED"))
                .ReturnsAsync("Q-001");

            quoteRepository
                .Setup(r => r.AddAsync(It.IsAny<Quote>()))
                .ReturnsAsync((Quote quote) => quote);

            var quoteService = new QuoteService(
                quoteRepository.Object,
                invoiceRepository.Object,
                notificationService.Object);

            var quote = new Quote
            {
                QuoteDate = DateTime.Today,
                CustomerId = 1,
                BillingAddress = "Test Address",
                PaymentTerms = "30 Days",
                QuoteItems = new List<QuoteItem>
                {
                    new QuoteItem
                    {
                        ProductId = 1,
                        Quantity = 2,
                        UnitPrice = 100m,
                        DiscountPercent = 0m,
                        VatCategory = "STANDARD"
                    }
                }
            };

            // Act
            var result = await quoteService.CreateQuoteAsync(quote);

            // Assert
            Assert.Equal(200m, result.Subtotal);
            Assert.Equal(230m, result.Total);
            Assert.Equal(230m, result.QuoteItems.First().LineTotal);
        }

        [Fact]
        public async Task NoneVat_ShouldCalculateZeroVat()
        {
            // Arrange
            var quoteRepository = new Mock<IQuoteRepository>();
            var invoiceRepository = new Mock<IInvoiceRepository>();
            var notificationService = new Mock<INotificationService>();

            quoteRepository
                .Setup(r => r.GenerateNextQuoteNumberAsync("ED"))
                .ReturnsAsync("Q-002");

            quoteRepository
                .Setup(r => r.AddAsync(It.IsAny<Quote>()))
                .ReturnsAsync((Quote quote) => quote);

            var quoteService = new QuoteService(
                quoteRepository.Object,
                invoiceRepository.Object,
                notificationService.Object);

            var quote = new Quote
            {
                QuoteDate = DateTime.Today,
                CustomerId = 1,
                BillingAddress = "Test Address",
                PaymentTerms = "30 Days",
                QuoteItems = new List<QuoteItem>
                {
                    new QuoteItem
                    {
                        ProductId = 1,
                        Quantity = 2,
                        UnitPrice = 100m,
                        DiscountPercent = 0m,
                        VatCategory = "NONE"
                    }
                }
            };

            // Act
            var result = await quoteService.CreateQuoteAsync(quote);

            // Assert
            Assert.Equal(200m, result.Subtotal);
            Assert.Equal(200m, result.Total);
            Assert.Equal(200m, result.QuoteItems.First().LineTotal);
        }

        [Fact]
        public async Task ZeroDiscount_ShouldLeaveLineTotalUnchanged()
        {
            // Arrange
            var quoteRepository = new Mock<IQuoteRepository>();
            var invoiceRepository = new Mock<IInvoiceRepository>();
            var notificationService = new Mock<INotificationService>();

            quoteRepository
                .Setup(r => r.GenerateNextQuoteNumberAsync("ED"))
                .ReturnsAsync("Q-003");

            quoteRepository
                .Setup(r => r.AddAsync(It.IsAny<Quote>()))
                .ReturnsAsync((Quote quote) => quote);

            var quoteService = new QuoteService(
                quoteRepository.Object,
                invoiceRepository.Object,
                notificationService.Object);

            var quote = new Quote
            {
                QuoteDate = DateTime.Today,
                CustomerId = 1,
                BillingAddress = "Test Address",
                PaymentTerms = "30 Days",
                QuoteItems = new List<QuoteItem>
                {
                    new QuoteItem
                    {
                        ProductId = 1,
                        Quantity = 2,
                        UnitPrice = 100m,
                        DiscountPercent = 0m,
                        VatCategory = "NONE"
                    }
                }
            };

            // Act
            var result = await quoteService.CreateQuoteAsync(quote);

            // Assert
            Assert.Equal(
                200m,
                result.QuoteItems.First().LineTotalExclVat);

            Assert.Equal(
                200m,
                result.QuoteItems.First().LineTotal);
        }

        [Fact]
        public async Task Discount_ShouldReduceLineTotal()
        {
            // Arrange
            var quoteRepository = new Mock<IQuoteRepository>();
            var invoiceRepository = new Mock<IInvoiceRepository>();
            var notificationService = new Mock<INotificationService>();

            quoteRepository
                .Setup(r => r.GenerateNextQuoteNumberAsync("ED"))
                .ReturnsAsync("Q-004");

            quoteRepository
                .Setup(r => r.AddAsync(It.IsAny<Quote>()))
                .ReturnsAsync((Quote quote) => quote);

            var quoteService = new QuoteService(
                quoteRepository.Object,
                invoiceRepository.Object,
                notificationService.Object);

            var quote = new Quote
            {
                QuoteDate = DateTime.Today,
                CustomerId = 1,
                BillingAddress = "Test Address",
                PaymentTerms = "30 Days",
                QuoteItems = new List<QuoteItem>
                {
                    new QuoteItem
                    {
                        ProductId = 1,
                        Quantity = 2,
                        UnitPrice = 100m,
                        DiscountPercent = 10m,
                        VatCategory = "NONE"
                    }
                }
            };

            // Act
            var result = await quoteService.CreateQuoteAsync(quote);

            // Assert
            Assert.Equal(
                180m,
                result.QuoteItems.First().LineTotalExclVat);

            Assert.Equal(
                180m,
                result.QuoteItems.First().LineTotal);
        }

        [Fact]
        public async Task MultipleProducts_ShouldCalculateCorrectSubtotal()
        {
            // Arrange
            var quoteRepository = new Mock<IQuoteRepository>();
            var invoiceRepository = new Mock<IInvoiceRepository>();
            var notificationService = new Mock<INotificationService>();

            quoteRepository
                .Setup(r => r.GenerateNextQuoteNumberAsync("ED"))
                .ReturnsAsync("Q-005");

            quoteRepository
                .Setup(r => r.AddAsync(It.IsAny<Quote>()))
                .ReturnsAsync((Quote quote) => quote);

            var quoteService = new QuoteService(
                quoteRepository.Object,
                invoiceRepository.Object,
                notificationService.Object);

            var quote = new Quote
            {
                QuoteDate = DateTime.Today,
                CustomerId = 1,
                BillingAddress = "Test Address",
                PaymentTerms = "30 Days",
                QuoteItems = new List<QuoteItem>
                {
                    new QuoteItem
                    {
                        ProductId = 1,
                        Quantity = 2,
                        UnitPrice = 100m,
                        DiscountPercent = 0m,
                        VatCategory = "NONE"
                    },
                    new QuoteItem
                    {
                        ProductId = 2,
                        Quantity = 3,
                        UnitPrice = 50m,
                        DiscountPercent = 0m,
                        VatCategory = "NONE"
                    }
                }
            };

            // Act
            var result = await quoteService.CreateQuoteAsync(quote);

            // Assert
            Assert.Equal(350m, result.Subtotal);
            Assert.Equal(350m, result.Total);
        }

        [Fact]
        public async Task GrandTotal_ShouldEqualSubtotalPlusVat()
        {
            // Arrange
            var quoteRepository = new Mock<IQuoteRepository>();
            var invoiceRepository = new Mock<IInvoiceRepository>();
            var notificationService = new Mock<INotificationService>();

            quoteRepository
                .Setup(r => r.GenerateNextQuoteNumberAsync("ED"))
                .ReturnsAsync("Q-006");

            quoteRepository
                .Setup(r => r.AddAsync(It.IsAny<Quote>()))
                .ReturnsAsync((Quote quote) => quote);

            var quoteService = new QuoteService(
                quoteRepository.Object,
                invoiceRepository.Object,
                notificationService.Object);

            var quote = new Quote
            {
                QuoteDate = DateTime.Today,
                CustomerId = 1,
                BillingAddress = "Test Address",
                PaymentTerms = "30 Days",
                QuoteItems = new List<QuoteItem>
                {
                    new QuoteItem
                    {
                        ProductId = 1,
                        Quantity = 2,
                        UnitPrice = 100m,
                        DiscountPercent = 0m,
                        VatCategory = "STANDARD"
                    }
                }
            };

            // Act
            var result = await quoteService.CreateQuoteAsync(quote);

            // Assert
            Assert.Equal(200m, result.Subtotal);
            Assert.Equal(230m, result.Total);
            Assert.Equal(
                result.Subtotal * 1.15m,
                result.Total);
        }
    }
}