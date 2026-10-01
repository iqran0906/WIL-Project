using FMCGEnterpriseManagementSystem.Models;

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
    }
}