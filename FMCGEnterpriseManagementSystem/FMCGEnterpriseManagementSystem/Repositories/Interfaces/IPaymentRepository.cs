// Title: Saving Data - Entity Framework Core
// Author: Microsoft
// Date: 14-06-2025
// Code version: Entity Framework Core / .NET 10
// Availability: https://learn.microsoft.com/en-us/ef/core/saving/

using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.Repositories.Interfaces
{
    // Purpose: Defines the database operations required for payment management.
    public interface IPaymentRepository
    {
        // Adds a new payment and returns the created payment.
        Task<Payment> AddAsync(Payment payment);

        // Retrieves a payment by its ID.
        Task<Payment?> GetByIdAsync(int paymentId);

        // Retrieves all payments.
        Task<List<Payment>> GetAllAsync();

        // Retrieves all payments belonging to a specific invoice.
        Task<List<Payment>> GetByInvoiceIdAsync(int invoiceId);

        // Calculates the total amount paid for an invoice.
        Task<decimal> GetTotalPaidForInvoiceAsync(int invoiceId);

        // Retrieves an invoice by its ID.
        Task<Invoice?> GetInvoiceByIdAsync(int invoiceId);

        // Retrieves all invoices.
        Task<List<Invoice>> GetAllInvoicesAsync();
    }
}