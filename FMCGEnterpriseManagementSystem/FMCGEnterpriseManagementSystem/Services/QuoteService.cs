using FMCGEnterpriseManagementSystem.Enums;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using FMCGEnterpriseManagementSystem.Services.Interfaces;

namespace FMCGEnterpriseManagementSystem.Services
{
    public class QuoteService : IQuoteService
    {
        private readonly IQuoteRepository _quoteRepository;

        // Temporary flat VAT rate until VatHelper/VatSettings is merged from Invoices branch
        private const decimal VatRate = 0.15m;

        public QuoteService(IQuoteRepository quoteRepository)
        {
            _quoteRepository = quoteRepository;
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
            quote.Status = QuoteStatus.Draft;

            CalculateTotals(quote);

            return await _quoteRepository.AddAsync(quote);
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

            // TODO: implement once Invoice module is merged into this branch
            quote.Status = QuoteStatus.Converted;
            await _quoteRepository.UpdateAsync(quote);

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

                var vatAmount = item.VatCategory == "[NONE]" ? 0 : lineAfterDiscount * VatRate;

                item.LineTotal = lineAfterDiscount + vatAmount;
                subtotal += lineAfterDiscount;
            }

            quote.Subtotal = subtotal;
            quote.Total = quote.QuoteItems.Sum(i => i.LineTotal);
        }
    }
}