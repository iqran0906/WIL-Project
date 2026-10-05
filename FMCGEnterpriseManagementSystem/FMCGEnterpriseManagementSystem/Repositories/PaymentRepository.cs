// Title: Saving Data - Entity Framework Core
// Author: Microsoft
// Date: 14-06-2025
// Code version: Entity Framework Core / .NET 10
// Availability: [https://learn.microsoft.com/en-us/ef/core/saving/]


using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FMCGEnterpriseManagementSystem.Repositories
{
    // Repository responsible for handling database operations related to payments and retrieving invoice information required by the payment functionality.
    public class PaymentRepository : IPaymentRepository
    {
        // Provides access to the application's database through Entity Framework Core.
        private readonly ApplicationDbContext _context;

        // Constructor receives the database context through dependency injection.
        public PaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Adds a new payment to the database and saves the changes.
        public async Task<Payment> AddAsync(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        // Retrieves a specific payment using its payment ID.
        // Related invoice and customer information are also loaded.
        public async Task<Payment?> GetByIdAsync(int paymentId)
        {
            return await _context.Payments
                .Include(p => p.Invoice)
                .ThenInclude(i => i.Customer)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);
        }

        // Retrieves all payments from the database.
        // Results are ordered with the most recent payment first.
        public async Task<List<Payment>> GetAllAsync()
        {
            return await _context.Payments
                .Include(p => p.Invoice)
                    .ThenInclude(i => i.Customer)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        // Retrieves all payments associated with a specific invoice.
        // Payments are ordered from newest to oldest.
        public async Task<List<Payment>> GetByInvoiceIdAsync(int invoiceId)
        {
            return await _context.Payments
                .Where(p => p.InvoiceId == invoiceId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        // Calculates the total amount that has been paid towards an invoice.
        // Returns zero when no payments exist for the specified invoice.
        public async Task<decimal> GetTotalPaidForInvoiceAsync(int invoiceId)
        {
            return await _context.Payments
                .Where(p => p.InvoiceId == invoiceId)
                .SumAsync(p => (decimal?)p.AmountPaid) ?? 0m;
        }

        // Retrieves an invoice by its ID together with the related customer.
        public async Task<Invoice?> GetInvoiceByIdAsync(int invoiceId)
        {
            return await _context.Invoices
                .Include(i => i.Customer)
                .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);
        }

        // Retrieves all invoices and their related customers.
        // Invoices are displayed with the most recent invoice first.
        public async Task<List<Invoice>> GetAllInvoicesAsync()
        {
            return await _context.Invoices
                .Include(i => i.Customer)
                .OrderByDescending(i => i.InvoiceDate)
                .ToListAsync();
        }
    }
}