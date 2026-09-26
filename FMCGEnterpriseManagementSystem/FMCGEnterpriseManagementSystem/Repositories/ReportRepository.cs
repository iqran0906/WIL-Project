using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FMCGEnterpriseManagementSystem.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly ApplicationDbContext _context;

        public ReportRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Invoice>> GetInvoicesAsync(
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var query = _context.Invoices
                .AsNoTracking()
                .Include(i => i.Customer)
                .Include(i => i.Payments)
                .Include(i => i.InvoiceItems)
                .ThenInclude(ii => ii.Product)
                .AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(i =>
                    i.InvoiceDate >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                var exclusiveEndDate = endDate.Value.Date.AddDays(1);

                query = query.Where(i =>
                    i.InvoiceDate < exclusiveEndDate);
            }

            return await query
                .OrderByDescending(i => i.InvoiceDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetPaymentsAsync(
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var query = _context.Payments
                .AsNoTracking()
                .Include(p => p.Invoice)
                    .ThenInclude(i => i.Customer)
                .AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(p =>
                    p.PaymentDate >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                var exclusiveEndDate = endDate.Value.Date.AddDays(1);

                query = query.Where(p =>
                    p.PaymentDate < exclusiveEndDate);
            }

            return await query
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Customer>> GetCustomersAsync()
        {
            return await _context.Customers
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ThenBy(c => c.Surname)
                .ToListAsync();
        }

        public async Task<IEnumerable<Quote>> GetQuotesAsync(
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var query = _context.Quotes
                .AsNoTracking()
                .Include(q => q.Customer)
                .Include(q => q.QuoteItems)
                    .ThenInclude(qi => qi.Product)
                .AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(q =>
                    q.QuoteDate >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                var exclusiveEndDate = endDate.Value.Date.AddDays(1);

                query = query.Where(q =>
                    q.QuoteDate < exclusiveEndDate);
            }

            return await query
                .OrderByDescending(q => q.QuoteDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<SalesRepresentative>>
            GetSalesRepresentativesAsync()
        {
            return await _context.SalesRepresentatives
                .AsNoTracking()
                .Include(sr => sr.Employee)
                .OrderBy(sr => sr.SalesRepCode)
                .ToListAsync();
        }

        public async Task<IEnumerable<Inventory>> GetInventoryAsync()
        {
            return await _context.Inventories
                .AsNoTracking()
                .Include(i => i.Product)
                .Include(i => i.StockBatches)
                .OrderBy(i => i.Product.ProductName)
                .ToListAsync();
        }
    }
}