// Purpose: Contract (interface) for the invoice service.
// Authors: Sayali-St10458649 (from git history)

using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    public interface IInvoiceService
    {
        Task<InvoiceViewModel> GetByIdAsync(int id);
        Task<IEnumerable<InvoiceViewModel>> GetAllAsync();
        Task<IEnumerable<InvoiceViewModel>> SearchAsync(int? customerId, DateTime? startDate, DateTime? endDate, string? keyword); Task<InvoiceViewModel> CreateAsync(InvoiceViewModel model);
        Task UpdateStatusAsync(int invoiceId, string newStatus);
        Task DeleteAsync(int id);

        Task<bool> SendInvoiceEmailAsync(int invoiceId);
    }
}