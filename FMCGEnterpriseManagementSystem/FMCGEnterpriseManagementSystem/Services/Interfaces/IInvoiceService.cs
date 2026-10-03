/***************************************************************************************
*    Title: Invoice Service Interface
*    Author: Sayali-St10458649
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Services/Interfaces/IInvoiceService.cs
***************************************************************************************/

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
    }
}