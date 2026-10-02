using System.Threading.Tasks;

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    public interface IEmailApiClientService
    {
        Task<(bool Success, string Message)> EmailInvoiceAsync(int invoiceId, string recipientEmail, byte[]? attachmentBytes, string? attachmentFileName);
        Task<(bool Success, string Message)> EmailQuoteAsync(int quoteId, string recipientEmail, byte[]? attachmentBytes, string? attachmentFileName);
        Task<(bool Success, string Message)> EmailPaymentAsync(int paymentId, string recipientEmail, byte[]? attachmentBytes, string? attachmentFileName);
    }
}