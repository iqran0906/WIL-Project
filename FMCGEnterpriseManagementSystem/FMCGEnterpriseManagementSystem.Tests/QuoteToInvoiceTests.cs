using FMCGEnterpriseManagementSystem.Enums;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using FMCGEnterpriseManagementSystem.Services;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using Moq;

namespace FMCGEnterpriseManagementSystem.Tests
{
    public class QuoteToInvoiceTests
    {
        [Fact]
        public async Task ConvertQuoteToInvoice_ShouldCreateInvoice()
        {
            // Arrange
            var quoteRepository = new Mock<IQuoteRepository>();
            var invoiceRepository = new Mock<IInvoiceRepository>();
            var notificationService = new Mock<INotificationService>();

            var quote = new Quote
            {
                QuoteId = 1,
                QuoteNumber = "Q-001",
                QuoteDate = DateTime.Today,
                CustomerId = 1,
                BillingAddress = "Test Address",
                PaymentTerms = "30 Days",
                Status = QuoteStatus.Pending,

                QuoteItems = new List<QuoteItem>
                {
                    new QuoteItem
                    {
                        ProductId = 1,
                        Quantity = 2,
                        UnitPrice = 100m,
                        DiscountPercent = 0m,
                        VatCategory = "NONE",
                        LineTotal = 200m
                    }
                },

                Subtotal = 200m,
                Total = 200m
            };

            quoteRepository
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(quote);

            quoteRepository
                .Setup(r => r.UpdateAsync(It.IsAny<Quote>()))
                .ReturnsAsync((Quote updatedQuote) => updatedQuote);

            invoiceRepository
                .Setup(r => r.GetNextInvoiceNumberAsync("ED"))
                .ReturnsAsync("ED-0001");

            invoiceRepository
                .Setup(r => r.AddAsync(It.IsAny<Invoice>()))
                .ReturnsAsync((Invoice invoice) => invoice);

            var quoteService = new QuoteService(
                quoteRepository.Object,
                invoiceRepository.Object,
                notificationService.Object);

            // Act
            var result =
                await quoteService.ConvertToInvoiceAsync(1);

            // Assert
            Assert.True(result);

            invoiceRepository.Verify(
                r => r.AddAsync(It.IsAny<Invoice>()),
                Times.Once);
        }

        [Fact]
        public async Task ConvertQuoteToInvoice_ShouldCopyAllItems()
        {
            // Arrange
            var quoteRepository = new Mock<IQuoteRepository>();
            var invoiceRepository = new Mock<IInvoiceRepository>();
            var notificationService = new Mock<INotificationService>();

            var quote = new Quote
            {
                QuoteId = 2,
                QuoteNumber = "Q-002",
                QuoteDate = DateTime.Today,
                CustomerId = 1,
                BillingAddress = "Test Address",
                PaymentTerms = "30 Days",
                Status = QuoteStatus.Pending,

                QuoteItems = new List<QuoteItem>
                {
                    new QuoteItem
                    {
                        ProductId = 1,
                        Quantity = 2,
                        UnitPrice = 100m,
                        DiscountPercent = 0m,
                        VatCategory = "NONE",
                        LineTotal = 200m
                    },
                    new QuoteItem
                    {
                        ProductId = 2,
                        Quantity = 3,
                        UnitPrice = 50m,
                        DiscountPercent = 0m,
                        VatCategory = "NONE",
                        LineTotal = 150m
                    }
                },

                Subtotal = 350m,
                Total = 350m
            };

            quoteRepository
                .Setup(r => r.GetByIdAsync(2))
                .ReturnsAsync(quote);

            quoteRepository
                .Setup(r => r.UpdateAsync(It.IsAny<Quote>()))
                .ReturnsAsync((Quote updatedQuote) => updatedQuote);

            invoiceRepository
                .Setup(r => r.GetNextInvoiceNumberAsync("ED"))
                .ReturnsAsync("ED-0002");

            Invoice? createdInvoice = null;

            invoiceRepository
                .Setup(r => r.AddAsync(It.IsAny<Invoice>()))
                .Callback<Invoice>(
                    invoice => createdInvoice = invoice)
                .ReturnsAsync((Invoice invoice) => invoice);

            var quoteService = new QuoteService(
                quoteRepository.Object,
                invoiceRepository.Object,
                notificationService.Object);

            // Act
            var result =
                await quoteService.ConvertToInvoiceAsync(2);

            // Assert
            Assert.True(result);
            Assert.NotNull(createdInvoice);
            Assert.Equal(
                2,
                createdInvoice!.InvoiceItems.Count);
        }

        [Fact]
        public async Task ConvertQuoteToInvoice_ShouldPreserveQuantitiesPricesAndDiscounts()
        {
            // Arrange
            var quoteRepository = new Mock<IQuoteRepository>();
            var invoiceRepository = new Mock<IInvoiceRepository>();
            var notificationService = new Mock<INotificationService>();

            var quote = new Quote
            {
                QuoteId = 3,
                QuoteNumber = "Q-003",
                QuoteDate = DateTime.Today,
                CustomerId = 1,
                BillingAddress = "Test Address",
                PaymentTerms = "30 Days",
                Status = QuoteStatus.Pending,

                QuoteItems = new List<QuoteItem>
                {
                    new QuoteItem
                    {
                        ProductId = 1,
                        Quantity = 5,
                        UnitPrice = 250m,
                        DiscountPercent = 10m,
                        VatCategory = "STANDARD",
                        LineTotal = 1237.50m
                    }
                },

                Subtotal = 1125m,
                Total = 1237.50m
            };

            quoteRepository
                .Setup(r => r.GetByIdAsync(3))
                .ReturnsAsync(quote);

            quoteRepository
                .Setup(r => r.UpdateAsync(It.IsAny<Quote>()))
                .ReturnsAsync((Quote updatedQuote) => updatedQuote);

            invoiceRepository
                .Setup(r => r.GetNextInvoiceNumberAsync("ED"))
                .ReturnsAsync("ED-0003");

            Invoice? createdInvoice = null;

            invoiceRepository
                .Setup(r => r.AddAsync(It.IsAny<Invoice>()))
                .Callback<Invoice>(
                    invoice => createdInvoice = invoice)
                .ReturnsAsync((Invoice invoice) => invoice);

            var quoteService = new QuoteService(
                quoteRepository.Object,
                invoiceRepository.Object,
                notificationService.Object);

            // Act
            var result =
                await quoteService.ConvertToInvoiceAsync(3);

            // Assert
            Assert.True(result);
            Assert.NotNull(createdInvoice);

            var quoteItem = quote.QuoteItems.First();
            var invoiceItem =
                createdInvoice!.InvoiceItems.First();

            Assert.Equal(
                quoteItem.Quantity,
                invoiceItem.Quantity);

            Assert.Equal(
                quoteItem.UnitPrice,
                invoiceItem.UnitPrice);

            Assert.Equal(
                quoteItem.DiscountPercent,
                invoiceItem.DiscountPercent);
        }

        [Fact]
        public async Task ConvertQuoteToInvoice_ShouldKeepInvoiceTotalEqualToQuoteTotal()
        {
            // Arrange
            var quoteRepository = new Mock<IQuoteRepository>();
            var invoiceRepository = new Mock<IInvoiceRepository>();
            var notificationService = new Mock<INotificationService>();

            var quote = new Quote
            {
                QuoteId = 4,
                QuoteNumber = "Q-004",
                QuoteDate = DateTime.Today,
                CustomerId = 1,
                BillingAddress = "Test Address",
                PaymentTerms = "30 Days",
                Status = QuoteStatus.Pending,

                QuoteItems = new List<QuoteItem>
                {
                    new QuoteItem
                    {
                        ProductId = 1,
                        Quantity = 2,
                        UnitPrice = 500m,
                        DiscountPercent = 0m,
                        VatCategory = "STANDARD",
                        LineTotal = 1150m
                    }
                },

                Subtotal = 1000m,
                Total = 1150m
            };

            quoteRepository
                .Setup(r => r.GetByIdAsync(4))
                .ReturnsAsync(quote);

            quoteRepository
                .Setup(r => r.UpdateAsync(It.IsAny<Quote>()))
                .ReturnsAsync((Quote updatedQuote) => updatedQuote);

            invoiceRepository
                .Setup(r => r.GetNextInvoiceNumberAsync("ED"))
                .ReturnsAsync("ED-0004");

            Invoice? createdInvoice = null;

            invoiceRepository
                .Setup(r => r.AddAsync(It.IsAny<Invoice>()))
                .Callback<Invoice>(
                    invoice => createdInvoice = invoice)
                .ReturnsAsync((Invoice invoice) => invoice);

            var quoteService = new QuoteService(
                quoteRepository.Object,
                invoiceRepository.Object,
                notificationService.Object);

            // Act
            var result =
                await quoteService.ConvertToInvoiceAsync(4);

            // Assert
            Assert.True(result);
            Assert.NotNull(createdInvoice);

            Assert.Equal(
                quote.Total,
                createdInvoice!.Total);
        }
    }
}