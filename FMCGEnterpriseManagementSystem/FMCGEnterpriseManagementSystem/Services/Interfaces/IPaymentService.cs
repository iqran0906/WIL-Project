// Title: Asynchronous programming with async and await
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    // Defines the operations used to record and manage customer payments.
    public interface IPaymentService
    {
        // Records a payment using the supplied payment information.
        Task<PaymentViewModel> RecordPaymentAsync(PaymentViewModel model);

        // Retrieves the payment form information for a specific invoice.
        Task<PaymentViewModel> GetPaymentFormForInvoiceAsync(int invoiceId);

        // Retrieves all recorded payments.
        Task<List<PaymentViewModel>> GetAllPaymentsAsync();

        // Retrieves all payments associated with a specific invoice.
        Task<List<PaymentViewModel>> GetPaymentsForInvoiceAsync(int invoiceId);

        // Calculates the outstanding balance remaining on an invoice.
        Task<decimal> GetOutstandingBalanceAsync(int invoiceId);

        // Retrieves a specific payment using its payment ID.
        // Returns null when the payment cannot be found.
        Task<PaymentViewModel?> GetPaymentByIdAsync(int paymentId);

        // Retrieves invoices that are available for recording payments.
        Task<List<InvoiceViewModel>> GetAvailableInvoicesAsync();
    }
}