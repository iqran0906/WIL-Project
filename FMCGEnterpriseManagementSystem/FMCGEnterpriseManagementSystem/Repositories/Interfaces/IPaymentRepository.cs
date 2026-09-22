using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment> AddAsync(Payment payment);
        Task<Payment?> GetByIdAsync(int paymentId);
        Task<List<Payment>> GetAllAsync();
        Task<List<Payment>> GetByInvoiceIdAsync(int invoiceId);
        Task<decimal> GetTotalPaidForInvoiceAsync(int invoiceId);
        Task<Invoice?> GetInvoiceByIdAsync(int invoiceId);
    }
}