// Purpose: Business logic for quote.
// Authors: Sayali-St10458649 (from git history)

using FMCGEnterpriseManagementSystem.Enums;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using FMCGEnterpriseManagementSystem.Services.Interfaces;

namespace FMCGEnterpriseManagementSystem.Services
{
    public class QuoteService : IQuoteService
    {
        private readonly IQuoteRepository _quoteRepository;

        private readonly ISettingsService _settingsService;

        public QuoteService(IQuoteRepository quoteRepository, ISettingsService settingsService)
        {
            _quoteRepository = quoteRepository;
            _settingsService = settingsService;
        }

        public async Task<IEnumerable<Quote>> GetAllQuotesAsync()
        {
            return await _quoteRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Quote>> SearchQuotesAsync(
            int? customerId,
            DateTime? startDate,
            DateTime? endDate,
            QuoteStatus? status,
            string? keyword)
        {
            var quotes = (await _quoteRepository.GetAllAsync()).AsEnumerable();

            if (customerId.HasValue)
                quotes = quotes.Where(q => q.CustomerId == customerId.Value);

            if (startDate.HasValue)
                quotes = quotes.Where(q => q.QuoteDate >= startDate.Value.Date);

            // Include the whole end day, not just up to midnight
            if (endDate.HasValue)
                quotes = quotes.Where(q => q.QuoteDate < endDate.Value.Date.AddDays(1));

            if (status.HasValue)
                quotes = quotes.Where(q => q.Status == status.Value);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();

                quotes = quotes.Where(q =>
                    (q.QuoteNumber?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (q.Customer != null &&
                     $"{q.Customer.Name} {q.Customer.Surname}".Contains(keyword, StringComparison.OrdinalIgnoreCase)));
            }

            return quotes
                .OrderByDescending(q => q.QuoteDate)
                .ToList();
        }

        public async Task<Quote> GetQuoteByIdAsync(int quoteId)
        {
            return await _quoteRepository.GetByIdAsync(quoteId);
        }

        public async Task<Quote> CreateQuoteAsync(Quote quote)
        {
            var settings = await _settingsService.GetAsync();

            quote.QuoteNumber = await _quoteRepository.GenerateNextQuoteNumberAsync(settings.QuotePrefix);
            quote.Status = QuoteStatus.Draft;

            CalculateTotals(quote, settings.VatRatePercent / 100m);

            return await _quoteRepository.AddAsync(quote);
        }

        public async Task<Quote> UpdateQuoteAsync(Quote quote)
        {
            CalculateTotals(quote, await _settingsService.GetVatRateAsync());
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

        private static void CalculateTotals(Quote quote, decimal vatRate)
        {
            decimal subtotal = 0;

            foreach (var item in quote.QuoteItems)
            {
                var lineBeforeDiscount = item.Quantity * item.UnitPrice;
                var discountAmount = lineBeforeDiscount * (item.DiscountPercent / 100);
                var lineAfterDiscount = lineBeforeDiscount - discountAmount;

                var vatAmount = item.VatCategory == "[NONE]" ? 0 : lineAfterDiscount * vatRate;

                item.LineTotalExclVat = lineAfterDiscount;
                item.LineTotal = lineAfterDiscount + vatAmount;
                subtotal += lineAfterDiscount;
            }

            quote.Subtotal = subtotal;
            quote.Total = quote.QuoteItems.Sum(i => i.LineTotal);
        }
    }
}