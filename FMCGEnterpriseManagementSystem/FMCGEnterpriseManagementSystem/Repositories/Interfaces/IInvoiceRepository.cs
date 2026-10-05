// Title: Implement CRUD - ASP.NET Core MVC with Entity Framework Core
// Author: Microsoft
// Date: 10-04-2024
// Code version: ASP.NET Core 10.0
// Availability: https://learn.microsoft.com/en-us/aspnet/core/data/ef-mvc/crud

using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.Repositories.Interfaces
{
    // Purpose: Defines the database operations required for invoice management.
    public interface IInvoiceRepository
    {
        // Retrieves an invoice by its ID.
        Task<Invoice> GetByIdAsync(int id);

        // Retrieves all invoices.
        Task<IEnumerable<Invoice>> GetAllAsync();

        // Adds a new invoice and returns the created invoice.
        Task<Invoice> AddAsync(Invoice invoice);

        // Updates an existing invoice.
        Task UpdateAsync(Invoice invoice);

        // Deletes an invoice using its ID.
        Task DeleteAsync(int id);

        // Generates the next invoice number using the specified prefix.
        Task<string> GetNextInvoiceNumberAsync(string prefix = "ED");
    }
}