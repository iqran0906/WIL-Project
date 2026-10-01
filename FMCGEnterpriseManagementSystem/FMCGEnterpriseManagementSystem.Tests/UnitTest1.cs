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
        public class QuoteToInvoiceTests
        {
            [Fact]
            public async Task ConvertQuoteToInvoice_ShouldCreateInvoice()
            {
                // Arrange
                var quoteRepository = new Mock<IQuoteRepository>();
                var invoiceRepository = new Mock<IInvoiceRepository>();

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
                    invoiceRepository.Object);

                // Act
                var result = await quoteService.ConvertToInvoiceAsync(1);

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
                    .Callback<Invoice>(invoice => createdInvoice = invoice)
                    .ReturnsAsync((Invoice invoice) => invoice);

                var quoteService = new QuoteService(
                    quoteRepository.Object,
                    invoiceRepository.Object);

                // Act
                var result = await quoteService.ConvertToInvoiceAsync(2);

                // Assert
                Assert.True(result);
                Assert.NotNull(createdInvoice);
                Assert.Equal(2, createdInvoice!.InvoiceItems.Count);
            }
            [Fact]
            public async Task ConvertQuoteToInvoice_ShouldPreserveQuantitiesPricesAndDiscounts()
            {
                // Arrange
                var quoteRepository = new Mock<IQuoteRepository>();
                var invoiceRepository = new Mock<IInvoiceRepository>();

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
                    .Callback<Invoice>(invoice => createdInvoice = invoice)
                    .ReturnsAsync((Invoice invoice) => invoice);

                var quoteService = new QuoteService(
                    quoteRepository.Object,
                    invoiceRepository.Object);

                // Act
                var result = await quoteService.ConvertToInvoiceAsync(3);

                // Assert
                Assert.True(result);
                Assert.NotNull(createdInvoice);

                var quoteItem = quote.QuoteItems.First();
                var invoiceItem = createdInvoice!.InvoiceItems.First();

                Assert.Equal(quoteItem.Quantity, invoiceItem.Quantity);
                Assert.Equal(quoteItem.UnitPrice, invoiceItem.UnitPrice);
                Assert.Equal(quoteItem.DiscountPercent, invoiceItem.DiscountPercent);
            }
            [Fact]
            public async Task ConvertQuoteToInvoice_ShouldKeepInvoiceTotalEqualToQuoteTotal()
            {
                // Arrange
                var quoteRepository = new Mock<IQuoteRepository>();
                var invoiceRepository = new Mock<IInvoiceRepository>();

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
                    .Callback<Invoice>(invoice => createdInvoice = invoice)
                    .ReturnsAsync((Invoice invoice) => invoice);

                var quoteService = new QuoteService(
                    quoteRepository.Object,
                    invoiceRepository.Object);

                // Act
                var result = await quoteService.ConvertToInvoiceAsync(4);

                // Assert
                Assert.True(result);
                Assert.NotNull(createdInvoice);
                Assert.Equal(quote.Total, createdInvoice!.Total);
            }
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
                    var result = await forecastingService.GetFilteredForecastsAsync(
                        null,
                        null);

                    // Assert
                    var forecast = result.Forecasts.First();

                    Assert.Equal(60, forecast.EstimatedMonthlyDemand);
                    Assert.Equal(50, forecast.RecommendedReorderQuantity);
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
                    var result = await forecastingService.GetFilteredForecastsAsync(
                        null,
                        null);

                    // Assert
                    var forecast = result.Forecasts.First();

                    Assert.Equal(60, forecast.EstimatedMonthlyDemand);
                    Assert.Equal(0, forecast.RecommendedReorderQuantity);
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
                    var result = await forecastingService.GetFilteredForecastsAsync(
                        null,
                        null);

                    // Assert
                    var forecast = result.Forecasts.First();

                    Assert.Equal(90, forecast.EstimatedMonthlyDemand);
                    Assert.Equal(70, forecast.RecommendedReorderQuantity);
                }
                public class SalesRepresentativeTests
                {
                    [Fact]
                    public async Task NewSalesRepresentative_ShouldGenerateSRCode()
                    {
                        // Arrange
                        var salesRepRepository = new Mock<ISalesRepresentativeRepository>();

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
                        var result = await salesRepService.CreateAsync(model);

                        // Assert
                        Assert.True(result);

                        salesRepRepository.Verify(
                            r => r.AddAsync(It.Is<SalesRepresentative>(
                                sr => sr.SalesRepCode == "SR-001")),
                            Times.Once);
                    }
                }
                [Fact]
                public async Task ExistingSalesRepCodes_ShouldGenerateNextAvailableCode()
                {
                    // Arrange
                    var salesRepRepository = new Mock<ISalesRepresentativeRepository>();

                    var existingSalesReps = new List<SalesRepresentative>
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
                    var result = await salesRepService.CreateAsync(model);

                    // Assert
                    Assert.True(result);

                    salesRepRepository.Verify(
                        r => r.AddAsync(It.Is<SalesRepresentative>(
                            sr => sr.SalesRepCode == "SR-003")),
                        Times.Once);
                }
            }
        }
    }
}