/***************************************************************************************
*    Title: Payment Repository Interface
*    Author: Sayali-St10458649
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Repositories/Interfaces/IPaymentRepository.cs
***************************************************************************************/
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
        Task<List<Invoice>> GetAllInvoicesAsync();
    }
}