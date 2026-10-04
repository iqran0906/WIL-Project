// Title: Querying Data with Entity Framework Core
// Author: Microsoft
// Date: 11-03-2021
// Code version: Entity Framework Core / .NET 10
// Availability: https://learn.microsoft.com/en-us/ef/core/querying/

using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FMCGEnterpriseManagementSystem.Repositories
{
    // Repository responsible for retrieving data used by the reporting functionality.
    public class ReportRepository : IReportRepository
    {
        // Provides access to the application's database.
        private readonly ApplicationDbContext _context;

        // Retrieves invoices within an optional date range.
        // Related customers, payments, invoice items and products are included.
        public async Task<IEnumerable<Invoice>> GetInvoicesAsync(
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            // Creates a query for invoices without tracking them, since the data is being retrieved for reporting purposes.
            var query = _context.Invoices
                .AsNoTracking()
                .Include(i => i.Customer)
                .Include(i => i.Payments)
                .Include(i => i.InvoiceItems)
                .ThenInclude(ii => ii.Product)
                .AsQueryable();

            // Applies the start date filter when a start date is provided.
            if (startDate.HasValue)
            {
                query = query.Where(i =>
                    i.InvoiceDate >= startDate.Value.Date);
            }

            // Applies the end date filter when an end date is provided.
            // The following day is used as an exclusive boundary so that the complete selected end date is included.
            if (endDate.HasValue)
            {
                var exclusiveEndDate = endDate.Value.Date.AddDays(1);

                query = query.Where(i =>
                    i.InvoiceDate < exclusiveEndDate);
            }

            // Returns invoices with the most recent invoice first.
            return await query
                .OrderByDescending(i => i.InvoiceDate)
                .ToListAsync();
        }

        // Retrieves payments within an optional date range.
        public async Task<IEnumerable<Payment>> GetPaymentsAsync(
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            // Retrieves payments without tracking and includes the related invoice and customer information.
            var query = _context.Payments
                .AsNoTracking()
                .Include(p => p.Invoice)
                    .ThenInclude(i => i.Customer)
                .AsQueryable();

            // Applies the start date filter when provided.
            if (startDate.HasValue)
            {
                query = query.Where(p =>
                    p.PaymentDate >= startDate.Value.Date);
            }

            // Applies the end date filter when provided.
            // The following day is used as an exclusive boundary to include the full selected end date.
            if (endDate.HasValue)
            {
                var exclusiveEndDate = endDate.Value.Date.AddDays(1);

                query = query.Where(p =>
                    p.PaymentDate < exclusiveEndDate);
            }

            // Returns payments with the newest payment first.
            return await query
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        // Retrieves all customers for use in reports.
        public async Task<IEnumerable<Customer>> GetCustomersAsync()
        {
            // Retrieves customers without tracking and sorts them alphabetically by name and surname.
            return await _context.Customers
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ThenBy(c => c.Surname)
                .ToListAsync();
        }

        // Retrieves quotes within an optional date range.
        // Related customers, quote items and products are included.
        public async Task<IEnumerable<Quote>> GetQuotesAsync(
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            // Creates a query for quotes without tracking.
            var query = _context.Quotes
                .AsNoTracking()
                .Include(q => q.Customer)
                .Include(q => q.QuoteItems)
                    .ThenInclude(qi => qi.Product)
                .AsQueryable();

            // Applies the start date filter when provided.
            if (startDate.HasValue)
            {
                query = query.Where(q =>
                    q.QuoteDate >= startDate.Value.Date);
            }

            // Applies the end date filter when provided. The following day is used as an exclusive boundary to include the complete end date.
            if (endDate.HasValue)
            {
                var exclusiveEndDate = endDate.Value.Date.AddDays(1);

                query = query.Where(q =>
                    q.QuoteDate < exclusiveEndDate);
            }

            // Returns quotes with the newest quote first.
            return await query
                .OrderByDescending(q => q.QuoteDate)
                .ToListAsync();
        }

        // Retrieves all sales representatives and their associated employees.
        public async Task<IEnumerable<SalesRepresentative>>
            GetSalesRepresentativesAsync()
        {
            // Retrieves sales representatives without tracking and orders them by their sales representative code.
            return await _context.SalesRepresentatives
                .AsNoTracking()
                .Include(sr => sr.Employee)
                .OrderBy(sr => sr.SalesRepCode)
                .ToListAsync();
        }

        // Retrieves inventory records together with their products and associated stock batches.
        public async Task<IEnumerable<Inventory>> GetInventoryAsync()
        {
            // Retrieves inventory without tracking and orders the results alphabetically according to the product name.
            return await _context.Inventories
                .AsNoTracking()
                .Include(i => i.Product)
                .Include(i => i.StockBatches)
                .OrderBy(i => i.Product.ProductName)
                .ToListAsync();
        }
    }
}