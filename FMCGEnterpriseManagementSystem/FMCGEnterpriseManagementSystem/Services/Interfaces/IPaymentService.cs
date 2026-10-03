/***************************************************************************************
*    Title: Payment Service Interface
*    Author: Naseeha27, Sayali-St10458649
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Services/Interfaces/IPaymentService.cs
***************************************************************************************/

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
        Task<List<InvoiceViewModel>> GetAvailableInvoicesAsync();
    }
}