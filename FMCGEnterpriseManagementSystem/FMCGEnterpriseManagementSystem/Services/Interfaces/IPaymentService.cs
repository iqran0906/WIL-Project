using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentViewModel> RecordPaymentAsync(PaymentViewModel model);
        Task<PaymentViewModel> GetPaymentFormForInvoiceAsync(int invoiceId);
        Task<List<PaymentViewModel>> GetAllPaymentsAsync();
        Task<List<PaymentViewModel>> GetPaymentsForInvoiceAsync(int invoiceId);
        Task<decimal> GetOutstandingBalanceAsync(int invoiceId);
        Task<PaymentViewModel?> GetPaymentByIdAsync(int paymentId);
    }
}