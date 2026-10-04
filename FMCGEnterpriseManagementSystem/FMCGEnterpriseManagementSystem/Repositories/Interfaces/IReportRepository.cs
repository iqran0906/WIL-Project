// Title: Querying Data with Entity Framework Core
// Author: Microsoft
// Date: 11-03-2021
// Code version: Entity Framework Core / .NET 10
// Availability: https://learn.microsoft.com/en-us/ef/core/querying/

using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.Repositories.Interfaces
{
    // Purpose: Defines database operations for retrieving data used in reports.
    public interface IReportRepository
    {
        // Retrieves invoices within an optional date range.
        Task<IEnumerable<Invoice>> GetInvoicesAsync(
            DateTime? startDate = null,
            DateTime? endDate = null);

        // Retrieves payments within an optional date range.
        Task<IEnumerable<Payment>> GetPaymentsAsync(
            DateTime? startDate = null,
            DateTime? endDate = null);

        // Retrieves all customers for reporting.
        Task<IEnumerable<Customer>> GetCustomersAsync();

        // Retrieves quotations within an optional date range.
        Task<IEnumerable<Quote>> GetQuotesAsync(
            DateTime? startDate = null,
            DateTime? endDate = null);

        // Retrieves all sales representatives for reporting.
        Task<IEnumerable<SalesRepresentative>> GetSalesRepresentativesAsync();

        // Retrieves inventory data for reporting.
        Task<IEnumerable<Inventory>> GetInventoryAsync();
    }
}