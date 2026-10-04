// Title: Implement CRUD - ASP.NET Core MVC with Entity Framework Core
// Author: Microsoft
// Date: 10-04-2024
// Code version: ASP.NET Core 10.0
// Availability: https://learn.microsoft.com/en-us/aspnet/core/data/ef-mvc/crud

using Microsoft.EntityFrameworkCore;
using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;

namespace FMCGEnterpriseManagementSystem.Repositories
{
    // Purpose: Provides database operations for invoice management.
    public class InvoiceRepository : IInvoiceRepository
    {
        // Provides access to the application's database.
        private readonly ApplicationDbContext _context;

        // Receives the database context through dependency injection.
        public InvoiceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Retrieves an invoice with its items, products, customer, and payments.
        public async Task<Invoice> GetByIdAsync(int id)
        {
            return await _context.Invoices
                .Include(i => i.InvoiceItems)
                    .ThenInclude(ii => ii.Product)
                .Include(i => i.Customer)
                .Include(i => i.Payments)
                .FirstOrDefaultAsync(i => i.InvoiceId == id);
        }

        // Retrieves all invoices with their related data.
        public async Task<IEnumerable<Invoice>> GetAllAsync()
        {
            return await _context.Invoices
                .Include(i => i.InvoiceItems)
                    .ThenInclude(ii => ii.Product)
                .Include(i => i.Customer)
                .Include(i => i.Payments)
                .ToListAsync();
        }

        // Adds a new invoice and saves the changes.
        public async Task<Invoice> AddAsync(Invoice invoice)
        {
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();
            return invoice;
        }

        // Updates an existing invoice and saves the changes.
        public async Task UpdateAsync(Invoice invoice)
        {
            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();
        }

        // Finds and deletes an invoice when it exists.
        public async Task DeleteAsync(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice != null)
            {
                _context.Invoices.Remove(invoice);
                await _context.SaveChangesAsync();
            }
        }

        // Generates the next invoice number using the specified prefix.
        public async Task<string> GetNextInvoiceNumberAsync(string prefix = "ED")
        {
            var count = await _context.Invoices.CountAsync();
            return $"{prefix}{(count + 1).ToString("D5")}";
        }
    }
}