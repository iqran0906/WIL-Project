// Title: Asynchronous programming with async and await
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    // Defines the operations used to manage invoice records.
    public interface IInvoiceService
    {
        // Retrieves a specific invoice using its ID.
        Task<InvoiceViewModel> GetByIdAsync(int id);

        // Retrieves all invoices in the system.
        Task<IEnumerable<InvoiceViewModel>> GetAllAsync();

        // Searches and filters invoices using customer, date range
        // and keyword criteria.
        Task<IEnumerable<InvoiceViewModel>> SearchAsync(int? customerId, DateTime? startDate, DateTime? endDate, string? keyword); Task<InvoiceViewModel> CreateAsync(InvoiceViewModel model);

        // Updates the status of an existing invoice.
        Task UpdateStatusAsync(int invoiceId, string newStatus);

        // Deletes an invoice using its ID.
        Task DeleteAsync(int id);
    }
}