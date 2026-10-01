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
            var payment = await paymentService.GetPaymentFormForInvoiceAsync(3);

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
}