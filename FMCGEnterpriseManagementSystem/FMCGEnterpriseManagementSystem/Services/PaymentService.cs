// Title: Asynchronous programming with async and await
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using FMCGEnterpriseManagementSystem.Enums;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services
{
    // Service responsible for payment-related business logic and validation.
    public class PaymentService : IPaymentService
    {
        // Repository used to retrieve and store payment and invoice information.
        private readonly IPaymentRepository _paymentRepository;

        // Initialises the payment service with its required repository dependency.
        public PaymentService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        // Records a payment against an approved invoice after performing validation.
        public async Task<PaymentViewModel> RecordPaymentAsync(PaymentViewModel model)
        {
            // Retrieves the invoice associated with the payment.
            var invoice = await _paymentRepository.GetInvoiceByIdAsync(model.InvoiceId)
    ?? throw new InvalidOperationException("Invoice not found.");

            // Ensures that payments can only be recorded against approved invoices.
            if (!string.Equals(
                    invoice.Status,
                    "Approved",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Payments can only be recorded for approved invoices.");
            }

            // Prevents zero or negative payment amounts from being recorded.
            if (model.AmountPaid <= 0)
            {
                throw new InvalidOperationException(
                    "Payment amount must be greater than zero.");
            }

            // Calculates how much has already been paid and determines the outstanding balance.
            var alreadyPaid = await _paymentRepository.GetTotalPaidForInvoiceAsync(model.InvoiceId);
            var outstanding = invoice.Total - alreadyPaid;

            // Prevents additional payments when the invoice has already been fully paid.
            if (outstanding <= 0)
            {
                throw new InvalidOperationException(
                    "This invoice has already been paid in full.");
            }

            // Prevents a payment from exceeding the remaining invoice balance.
            if (model.AmountPaid > outstanding)
                throw new InvalidOperationException(
                    $"Payment of {model.AmountPaid:C} exceeds the outstanding balance of {outstanding:C}.");

            // Creates a new payment entity using the supplied payment details.
            var payment = new Payment
            {
                InvoiceId = model.InvoiceId,
                PaymentDate = model.PaymentDate,
                AmountPaid = model.AmountPaid,
                PaymentMethod = model.PaymentMethod
            };

            // Adds the payment to the repository.
            await _paymentRepository.AddAsync(payment);

            // Builds the payment view model using the updated total amount paid.
            return await BuildViewModelAsync(invoice, model.AmountPaid + alreadyPaid);
        }

        // Prepares payment information for a specific invoice before a payment is recorded.
        public async Task<PaymentViewModel> GetPaymentFormForInvoiceAsync(int invoiceId)
        {
            // Retrieves the selected invoice.
            var invoice = await _paymentRepository.GetInvoiceByIdAsync(invoiceId)
                ?? throw new InvalidOperationException("Invoice not found.");

            // Only approved invoices can receive payments.
            if (!string.Equals(
                    invoice.Status,
                    "Approved",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Payments can only be recorded for approved invoices.");
            }

            // Calculates the amount already paid against the invoice.
            var alreadyPaid =
                await _paymentRepository.GetTotalPaidForInvoiceAsync(invoiceId);

            var outstanding = invoice.Total - alreadyPaid;

            // Prevents payments from being made against fully paid invoices.
            if (outstanding <= 0)
            {
                throw new InvalidOperationException(
                    "This invoice has already been paid in full.");
            }

            // Returns the invoice payment information for the payment form.
            return await BuildViewModelAsync(invoice, alreadyPaid);
        }

        // Retrieves all invoices that are eligible to receive payments.
        public async Task<List<InvoiceViewModel>> GetAvailableInvoicesAsync()
        {
            var invoices = await _paymentRepository.GetAllInvoicesAsync();

            var result = new List<InvoiceViewModel>();

            foreach (var invoice in invoices)
            {
                // Only approved invoices may receive payments.
                if (!string.Equals(
                        invoice.Status,
                        "Approved",
                        StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                // Calculates the total amount already paid for the invoice.
                var totalPaid =
                    await _paymentRepository.GetTotalPaidForInvoiceAsync(
                        invoice.InvoiceId);

                var outstanding = invoice.Total - totalPaid;

                // Fully paid invoices are excluded from the available list.
                if (outstanding <= 0)
                {
                    continue;
                }

                // Adds the outstanding invoice information to the result list.
                result.Add(new InvoiceViewModel
                {
                    InvoiceId = invoice.InvoiceId,
                    InvoiceNumber = invoice.InvoiceNumber,
                    InvoiceDate = invoice.InvoiceDate,

                    CustomerName = invoice.Customer != null
                        ? $"{invoice.Customer.Name} {invoice.Customer.Surname}"
                        : null,

                    Total = invoice.Total,
                    AmountDue = outstanding,
                    Status = invoice.Status
                });
            }

            return result;
        }

        // Retrieves all recorded payments and calculates their current payment status.
        public async Task<List<PaymentViewModel>> GetAllPaymentsAsync()
        {
            var payments = await _paymentRepository.GetAllAsync();

            var result = new List<PaymentViewModel>();

            foreach (var payment in payments)
            {
                // Calculates the total amount paid against the related invoice.
                var totalPaid = await _paymentRepository
                    .GetTotalPaidForInvoiceAsync(payment.InvoiceId);

                var invoiceTotal = payment.Invoice?.Total ?? 0m;
                var outstanding = invoiceTotal - totalPaid;

                // Determines the payment status when the related invoice exists.
                var status = payment.Invoice != null
                    ? CalculateStatus(payment.Invoice, totalPaid, outstanding)
                    : PaymentStatus.Unpaid;

                // Maps the payment entity and calculated values to a view model.
                result.Add(new PaymentViewModel
                {
                    PaymentId = payment.PaymentId,
                    InvoiceId = payment.InvoiceId,
                    InvoiceNumber = payment.Invoice?.InvoiceNumber,
                    CustomerName = payment.Invoice?.Customer != null
    ? $"{payment.Invoice.Customer.Name} {payment.Invoice.Customer.Surname}"
    : null,

                    InvoiceTotal = invoiceTotal,
                    AmountAlreadyPaid = totalPaid,
                    OutstandingBalance = outstanding < 0 ? 0 : outstanding,

                    PaymentDate = payment.PaymentDate,
                    AmountPaid = payment.AmountPaid,
                    PaymentMethod = payment.PaymentMethod,

                    Status = status,
                    IsOverdue = status == PaymentStatus.Overdue
                });
            }

            return result;
        }

        // Retrieves all payments associated with a specific invoice.
        public async Task<List<PaymentViewModel>> GetPaymentsForInvoiceAsync(int invoiceId)
        {
            var payments = await _paymentRepository.GetByInvoiceIdAsync(invoiceId);

            // Maps each payment entity to a payment view model.
            return payments.Select(p => new PaymentViewModel
            {
                PaymentId = p.PaymentId,
                InvoiceId = p.InvoiceId,
                PaymentDate = p.PaymentDate,
                AmountPaid = p.AmountPaid,
                PaymentMethod = p.PaymentMethod
            }).ToList();
        }

        // Calculates the outstanding balance for a specific invoice.
        public async Task<decimal> GetOutstandingBalanceAsync(int invoiceId)
        {
            // Retrieves the invoice used to determine the total amount due.
            var invoice = await _paymentRepository.GetInvoiceByIdAsync(invoiceId)
                ?? throw new InvalidOperationException("Invoice not found.");

            // Retrieves the total amount already paid.
            var paid = await _paymentRepository.GetTotalPaidForInvoiceAsync(invoiceId);

            // Returns the remaining amount owed on the invoice.
            return invoice.Total - paid;
        }

        // Retrieves a specific payment and builds its detailed view model.
        public async Task<PaymentViewModel?> GetPaymentByIdAsync(int paymentId)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);

            // Returns null when the requested payment does not exist.
            if (payment == null)
                return null;

            // Calculates the current payment totals and status.
            var totalPaid = await _paymentRepository.GetTotalPaidForInvoiceAsync(payment.InvoiceId);
            var outstanding = payment.Invoice.Total - totalPaid;
            var status = CalculateStatus(payment.Invoice, totalPaid, outstanding);

            // Maps the payment information into a view model.
            return new PaymentViewModel
            {
                PaymentId = payment.PaymentId,
                InvoiceId = payment.InvoiceId,
                InvoiceNumber = payment.Invoice?.InvoiceNumber,
                CustomerName = payment.Invoice?.Customer != null
    ? $"{payment.Invoice.Customer.Name} {payment.Invoice.Customer.Surname}"
    : null,
                InvoiceTotal = payment.Invoice.Total,
                AmountAlreadyPaid = totalPaid,
                OutstandingBalance = outstanding < 0 ? 0 : outstanding,
                PaymentDate = payment.PaymentDate,
                PaymentMethod = payment.PaymentMethod,
                AmountPaid = payment.AmountPaid,
                Status = status,
                IsOverdue = status == PaymentStatus.Overdue
            };
        }

        // Builds a payment view model containing invoice totals, status and payment history.
        private async Task<PaymentViewModel> BuildViewModelAsync(
         Invoice invoice,
         decimal totalPaid)
        {
            // Calculates the outstanding balance and current payment status.
            var outstanding = invoice.Total - totalPaid;
            var status = CalculateStatus(invoice, totalPaid, outstanding);

            return new PaymentViewModel
            {
                InvoiceId = invoice.InvoiceId,
                InvoiceNumber = invoice.InvoiceNumber,

                CustomerName = invoice.Customer != null
                    ? $"{invoice.Customer.Name} {invoice.Customer.Surname}"
                    : null,

                InvoiceTotal = invoice.Total,
                AmountAlreadyPaid = totalPaid,
                OutstandingBalance = outstanding < 0 ? 0 : outstanding,

                Status = status,
                IsOverdue = status == PaymentStatus.Overdue,

                // Includes the payment history for the selected invoice.
                PaymentHistory = await GetPaymentsForInvoiceAsync(invoice.InvoiceId)
            };
        }

        // Determines the payment status based on the invoice balance and amount already paid.
        private PaymentStatus CalculateStatus(
            Invoice invoice,
            decimal totalPaid,
            decimal outstanding)
        {
            // An invoice with no outstanding balance is considered fully paid.
            if (outstanding <= 0)
                return PaymentStatus.Paid;

            // An invoice with some payment made but an outstanding balance is partially paid.
            return totalPaid > 0
                ? PaymentStatus.PartiallyPaid
                : PaymentStatus.Unpaid;
        }
    }
}