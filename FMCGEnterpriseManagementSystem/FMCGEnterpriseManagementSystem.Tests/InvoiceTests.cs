using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using FMCGEnterpriseManagementSystem.Services;
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
}