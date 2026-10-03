// Purpose: Business logic for payment.
// Authors: Naseeha27, Sayali-St10458649 (from git history)

using FMCGEnterpriseManagementSystem.Enums;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<PaymentViewModel> RecordPaymentAsync(PaymentViewModel model)
        {
            var invoice = await _paymentRepository.GetInvoiceByIdAsync(model.InvoiceId)
    ?? throw new InvalidOperationException("Invoice not found.");

            if (!string.Equals(
                    invoice.Status,
                    "Approved",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Payments can only be recorded for approved invoices.");
            }

            if (model.AmountPaid <= 0)
            {
                throw new InvalidOperationException(
                    "Payment amount must be greater than zero.");
            }
            throw new InvalidOperationException("Payment amount must be greater than zero.");

            var alreadyPaid = await _paymentRepository.GetTotalPaidForInvoiceAsync(model.InvoiceId);
            var outstanding = invoice.Total - alreadyPaid;

            if (outstanding <= 0)
            {
                throw new InvalidOperationException(
                    "This invoice has already been paid in full.");
            }

            if (model.AmountPaid > outstanding)
                throw new InvalidOperationException(
                    $"Payment of {model.AmountPaid:C} exceeds the outstanding balance of {outstanding:C}.");

            var payment = new Payment
            {
                InvoiceId = model.InvoiceId,
                PaymentDate = model.PaymentDate,
                AmountPaid = model.AmountPaid,
                PaymentMethod = model.PaymentMethod
            };

            await _paymentRepository.AddAsync(payment);

            return await BuildViewModelAsync(invoice, model.AmountPaid + alreadyPaid);
        }

        public async Task<PaymentViewModel> GetPaymentFormForInvoiceAsync(int invoiceId)
        {
            var invoice = await _paymentRepository.GetInvoiceByIdAsync(invoiceId)
                ?? throw new InvalidOperationException("Invoice not found.");

            if (!string.Equals(
                    invoice.Status,
                    "Approved",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Payments can only be recorded for approved invoices.");
            }

            var alreadyPaid =
                await _paymentRepository.GetTotalPaidForInvoiceAsync(invoiceId);

            var outstanding = invoice.Total - alreadyPaid;

            if (outstanding <= 0)
            {
                throw new InvalidOperationException(
                    "This invoice has already been paid in full.");
            }

            return await BuildViewModelAsync(invoice, alreadyPaid);
        }

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

                var totalPaid =
                    await _paymentRepository.GetTotalPaidForInvoiceAsync(
                        invoice.InvoiceId);

                var outstanding = invoice.Total - totalPaid;

                
                if (outstanding <= 0)
                {
                    continue;
                }

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

        public async Task<List<PaymentViewModel>> GetAllPaymentsAsync()
        {
            var payments = await _paymentRepository.GetAllAsync();

            var result = new List<PaymentViewModel>();

            foreach (var payment in payments)
            {
                var totalPaid = await _paymentRepository
                    .GetTotalPaidForInvoiceAsync(payment.InvoiceId);

                var invoiceTotal = payment.Invoice?.Total ?? 0m;
                var outstanding = invoiceTotal - totalPaid;

                var status = payment.Invoice != null
                    ? CalculateStatus(payment.Invoice, totalPaid, outstanding)
                    : PaymentStatus.Unpaid;

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

        public async Task<List<PaymentViewModel>> GetPaymentsForInvoiceAsync(int invoiceId)
        {
            var payments = await _paymentRepository.GetByInvoiceIdAsync(invoiceId);

            return payments.Select(p => new PaymentViewModel
            {
                PaymentId = p.PaymentId,
                InvoiceId = p.InvoiceId,
                PaymentDate = p.PaymentDate,
                AmountPaid = p.AmountPaid,
                PaymentMethod = p.PaymentMethod
            }).ToList();
        }

        public async Task<decimal> GetOutstandingBalanceAsync(int invoiceId)
        {
            var invoice = await _paymentRepository.GetInvoiceByIdAsync(invoiceId)
                ?? throw new InvalidOperationException("Invoice not found.");

            var paid = await _paymentRepository.GetTotalPaidForInvoiceAsync(invoiceId);

            return invoice.Total - paid;
        }

        public async Task<PaymentViewModel?> GetPaymentByIdAsync(int paymentId)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);

            if (payment == null)
                return null;

            var totalPaid = await _paymentRepository.GetTotalPaidForInvoiceAsync(payment.InvoiceId);
            var outstanding = payment.Invoice.Total - totalPaid;
            var status = CalculateStatus(payment.Invoice, totalPaid, outstanding);

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

        private async Task<PaymentViewModel> BuildViewModelAsync(
            Invoice invoice,
            decimal totalPaid)
        {
            var outstanding = invoice.Total - totalPaid;
            var status = CalculateStatus(invoice, totalPaid, outstanding);

            return new PaymentViewModel
            {
                InvoiceId = invoice.InvoiceId,
                InvoiceNumber = invoice.InvoiceNumber,
                InvoiceTotal = invoice.Total,
                AmountAlreadyPaid = totalPaid,
                OutstandingBalance = outstanding < 0 ? 0 : outstanding,
                Status = status,
                IsOverdue = status == PaymentStatus.Overdue,
                PaymentHistory = await GetPaymentsForInvoiceAsync(invoice.InvoiceId)
            };
        }

        private PaymentStatus CalculateStatus(
            Invoice invoice,
            decimal totalPaid,
            decimal outstanding)
        {
            if (outstanding <= 0)
                return PaymentStatus.Paid;

            return totalPaid > 0
                ? PaymentStatus.PartiallyPaid
                : PaymentStatus.Unpaid;
        }
    }
}