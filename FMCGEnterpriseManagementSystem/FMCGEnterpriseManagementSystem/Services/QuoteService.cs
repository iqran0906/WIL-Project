using FMCGEnterpriseManagementSystem.Enums;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using FMCGEnterpriseManagementSystem.Services.Interfaces;

namespace FMCGEnterpriseManagementSystem.Services
{
    public class QuoteService : IQuoteService
    {
        private readonly IQuoteRepository _quoteRepository;
        private readonly IInvoiceRepository _invoiceRepository;

        private readonly INotificationService _notificationService;

        private const decimal VatRate = 0.15m;

        public QuoteService(IQuoteRepository quoteRepository, IInvoiceRepository invoiceRepository, INotificationService notificationService)
        {
            _quoteRepository = quoteRepository;
            _invoiceRepository = invoiceRepository;
            _notificationService = notificationService;
        }

        public async Task<IEnumerable<Quote>> GetAllQuotesAsync()
        {
            return await _quoteRepository.GetAllAsync();
        }

        public async Task<Quote> GetQuoteByIdAsync(int quoteId)
        {
            return await _quoteRepository.GetByIdAsync(quoteId);
        }

        public async Task<Quote> CreateQuoteAsync(Quote quote)
        {
            quote.QuoteNumber = await _quoteRepository.GenerateNextQuoteNumberAsync();
            quote.Status = QuoteStatus.Pending;

            CalculateTotals(quote);

            var saved = await _quoteRepository.AddAsync(quote);

            await _notificationService.NotifyNewQuoteAsync(saved.QuoteNumber, saved.QuoteId, saved.CustomerId.ToString());

            return saved;
        }

        public async Task<Quote> UpdateQuoteAsync(Quote quote)
        {
            CalculateTotals(quote);
            return await _quoteRepository.UpdateAsync(quote);
        }

        public async Task<bool> DeleteQuoteAsync(int quoteId)
        {
            return await _quoteRepository.DeleteAsync(quoteId);
        }
        public async Task<bool> ConvertToInvoiceAsync(int quoteId)
        {
            var quote = await _quoteRepository.GetByIdAsync(quoteId);
            if (quote == null)
            {
                return false;
            }

            var invoice = new Invoice
            {
                InvoiceNumber = await _invoiceRepository.GetNextInvoiceNumberAsync(),
                InvoiceDate = DateTime.Today,
                QuoteId = quote.QuoteId,
                CustomerId = quote.CustomerId,
                BillingAddress = quote.BillingAddress,
                PaymentTerms = quote.PaymentTerms,
                SalesRepresentativeId = quote.SalesRepresentativeId,
                Status = "Draft",
                Subtotal = quote.Subtotal,
                Total = quote.Total,
                InvoiceItems = quote.QuoteItems.Select(qi => new InvoiceItem
                {
                    ProductId = qi.ProductId,
                    Quantity = qi.Quantity,
                    UnitPrice = qi.UnitPrice,
                    DiscountPercent = qi.DiscountPercent,
                    VatCategory = qi.VatCategory,
                    LineTotal = qi.LineTotal
                }).ToList()
            };

            await _invoiceRepository.AddAsync(invoice);

            quote.Status = QuoteStatus.Invoiced;
            await _quoteRepository.UpdateAsync(quote);

            await _notificationService.NotifyNewInvoiceAsync(invoice.InvoiceNumber, invoice.InvoiceId, quote.CustomerId.ToString());

            return true;
        }

        private void CalculateTotals(Quote quote)
        {
            decimal subtotal = 0;

            foreach (var item in quote.QuoteItems)
            {
                var lineBeforeDiscount = item.Quantity * item.UnitPrice;
                var discountAmount = lineBeforeDiscount * (item.DiscountPercent / 100);
                var lineAfterDiscount = lineBeforeDiscount - discountAmount;

                var vatAmount = item.VatCategory == "STANDARD"
    ? lineAfterDiscount * VatRate
    : 0;

                item.LineTotalExclVat = lineAfterDiscount;
                item.LineTotal = lineAfterDiscount + vatAmount;
                subtotal += lineAfterDiscount;
            }

            quote.Subtotal = subtotal;
            quote.Total = quote.QuoteItems.Sum(i => i.LineTotal);
        }
    }
}