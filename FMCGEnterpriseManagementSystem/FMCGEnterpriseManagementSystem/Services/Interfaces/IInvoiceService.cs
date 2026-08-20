using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    public interface IInvoiceService
    {
        Task<InvoiceViewModel> GetByIdAsync(int id);
        Task<IEnumerable<InvoiceViewModel>> GetAllAsync();
        Task<IEnumerable<InvoiceViewModel>> SearchAsync(string customerId, DateTime? startDate, DateTime? endDate, string keyword);
        Task<InvoiceViewModel> CreateAsync(InvoiceViewModel model);
        Task UpdateStatusAsync(int invoiceId, string newStatus);
        Task DeleteAsync(int id);
    }
}