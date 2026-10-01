using FMCGEnterpriseManagementSystem.Enums;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using FMCGEnterpriseManagementSystem.Services;
using FMCGEnterpriseManagementSystem.ViewModels;
using Moq;

namespace FMCGEnterpriseManagementSystem.Tests
{
    public class InvoiceTests
    {
        [Fact]
        public void NewInvoice_ShouldHaveDraftStatus()
        {
            // Arrange
            var invoice = new Invoice();

            // Act
            var status = invoice.Status;

            // Assert
            Assert.Equal("Draft", status);
        }

        [Fact]
        public async Task NewInvoice_ShouldHaveCorrectOutstandingBalance()
        {
            // Arrange
            var invoice = new Invoice
            {
                InvoiceId = 1,
                Total = 1000m
            };

            var paymentRepository = new Mock<IPaymentRepository>();

            paymentRepository
                .Setup(r => r.GetInvoiceByIdAsync(1))
                .ReturnsAsync(invoice);

            paymentRepository
                .Setup(r => r.GetTotalPaidForInvoiceAsync(1))
                .ReturnsAsync(0m);

            var paymentService = new PaymentService(paymentRepository.Object);

            // Act
            var outstandingBalance =
                await paymentService.GetOutstandingBalanceAsync(1);

            // Assert
            Assert.Equal(1000m, outstandingBalance);
        }
    }

    public class PaymentTests
    {
        [Fact]
        public async Task PartialPayment_ShouldReduceOutstandingBalance()
        {
            // Arrange
            var invoice = new Invoice
            {
                InvoiceId = 1,
                Total = 1000m
            };

            var paymentRepository = new Mock<IPaymentRepository>();

            paymentRepository
                .Setup(r => r.GetInvoiceByIdAsync(1))
                .ReturnsAsync(invoice);

            paymentRepository
                .Setup(r => r.GetTotalPaidForInvoiceAsync(1))
                .ReturnsAsync(300m);

            var paymentService = new PaymentService(paymentRepository.Object);

            // Act
            var outstandingBalance =
                await paymentService.GetOutstandingBalanceAsync(1);

            // Assert
            Assert.Equal(700m, outstandingBalance);
        }

        [Fact]
        public async Task FullPayment_ShouldMakeOutstandingBalanceZero()
        {
            // Arrange
            var invoice = new Invoice
            {
                InvoiceId = 2,
                Total = 1000m
            };

            var paymentRepository = new Mock<IPaymentRepository>();

            paymentRepository
                .Setup(r => r.GetInvoiceByIdAsync(2))
                .ReturnsAsync(invoice);

            paymentRepository
                .Setup(r => r.GetTotalPaidForInvoiceAsync(2))
                .ReturnsAsync(1000m);

            var paymentService = new PaymentService(paymentRepository.Object);

            // Act
            var outstandingBalance =
                await paymentService.GetOutstandingBalanceAsync(2);

            // Assert
            Assert.Equal(0m, outstandingBalance);
        }

        [Fact]
        public async Task FullPayment_ShouldChangeStatusToPaid()
        {
            // Arrange
            var invoice = new Invoice
            {
                InvoiceId = 3,
                Total = 1000m
            };

            var paymentRepository = new Mock<IPaymentRepository>();

            paymentRepository
                .Setup(r => r.GetInvoiceByIdAsync(3))
                .ReturnsAsync(invoice);

            paymentRepository
                .Setup(r => r.GetTotalPaidForInvoiceAsync(3))
                .ReturnsAsync(1000m);

            paymentRepository
                .Setup(r => r.GetByInvoiceIdAsync(3))
                .ReturnsAsync(new List<Payment>());

            var paymentService = new PaymentService(paymentRepository.Object);

            // Act
            var payment =
                await paymentService.GetPaymentFormForInvoiceAsync(3);

            // Assert
            Assert.Equal(PaymentStatus.Paid, payment.Status);
        }

        [Fact]
        public async Task Overpayment_ShouldBeRejected()
        {
            // Arrange
            var invoice = new Invoice
            {
                InvoiceId = 4,
                Total = 1000m
            };

            var paymentRepository = new Mock<IPaymentRepository>();

            paymentRepository
                .Setup(r => r.GetInvoiceByIdAsync(4))
                .ReturnsAsync(invoice);

            paymentRepository
                .Setup(r => r.GetTotalPaidForInvoiceAsync(4))
                .ReturnsAsync(0m);

            var paymentService = new PaymentService(paymentRepository.Object);

            var payment = new PaymentViewModel
            {
                InvoiceId = 4,
                AmountPaid = 1200m,
                PaymentDate = DateTime.UtcNow,
                PaymentMethod = "Cash"
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => paymentService.RecordPaymentAsync(payment));
        }
    }

    public class QuoteTests
    {
        [Fact]
        public async Task StandardVat_ShouldCalculate15Percent()
        {
            // Arrange
            var quoteRepository = new Mock<IQuoteRepository>();
            var invoiceRepository = new Mock<IInvoiceRepository>();

            quoteRepository
                .Setup(r => r.GenerateNextQuoteNumberAsync("ED"))
                .ReturnsAsync("Q-001");

            quoteRepository
                .Setup(r => r.AddAsync(It.IsAny<Quote>()))
                .ReturnsAsync((Quote quote) => quote);

            var quoteService = new QuoteService(
                quoteRepository.Object,
                invoiceRepository.Object);

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

            quoteRepository
                .Setup(r => r.GenerateNextQuoteNumberAsync("ED"))
                .ReturnsAsync("Q-002");

            quoteRepository
                .Setup(r => r.AddAsync(It.IsAny<Quote>()))
                .ReturnsAsync((Quote quote) => quote);

            var quoteService = new QuoteService(
                quoteRepository.Object,
                invoiceRepository.Object);

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

            quoteRepository
                .Setup(r => r.GenerateNextQuoteNumberAsync("ED"))
                .ReturnsAsync("Q-003");

            quoteRepository
                .Setup(r => r.AddAsync(It.IsAny<Quote>()))
                .ReturnsAsync((Quote quote) => quote);

            var quoteService = new QuoteService(
                quoteRepository.Object,
                invoiceRepository.Object);

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
            Assert.Equal(200m, result.QuoteItems.First().LineTotalExclVat);
            Assert.Equal(200m, result.QuoteItems.First().LineTotal);
        }
        [Fact]
        public async Task Discount_ShouldReduceLineTotal()
        {
            // Arrange
            var quoteRepository = new Mock<IQuoteRepository>();
            var invoiceRepository = new Mock<IInvoiceRepository>();

            quoteRepository
                .Setup(r => r.GenerateNextQuoteNumberAsync("ED"))
                .ReturnsAsync("Q-004");

            quoteRepository
                .Setup(r => r.AddAsync(It.IsAny<Quote>()))
                .ReturnsAsync((Quote quote) => quote);

            var quoteService = new QuoteService(
                quoteRepository.Object,
                invoiceRepository.Object);

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
            Assert.Equal(180m, result.QuoteItems.First().LineTotalExclVat);
            Assert.Equal(180m, result.QuoteItems.First().LineTotal);
        }
        [Fact]
        public async Task MultipleProducts_ShouldCalculateCorrectSubtotal()
        {
            // Arrange
            var quoteRepository = new Mock<IQuoteRepository>();
            var invoiceRepository = new Mock<IInvoiceRepository>();

            quoteRepository
                .Setup(r => r.GenerateNextQuoteNumberAsync("ED"))
                .ReturnsAsync("Q-005");

            quoteRepository
                .Setup(r => r.AddAsync(It.IsAny<Quote>()))
                .ReturnsAsync((Quote quote) => quote);

            var quoteService = new QuoteService(
                quoteRepository.Object,
                invoiceRepository.Object);

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

            quoteRepository
                .Setup(r => r.GenerateNextQuoteNumberAsync("ED"))
                .ReturnsAsync("Q-006");

            quoteRepository
                .Setup(r => r.AddAsync(It.IsAny<Quote>()))
                .ReturnsAsync((Quote quote) => quote);

            var quoteService = new QuoteService(
                quoteRepository.Object,
                invoiceRepository.Object);

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