// Purpose: Contract (interface) for the quote service.
// Authors: Sayali-St10458649 (from git history)

using FMCGEnterpriseManagementSystem.Enums;
using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    public interface IQuoteService
    {
        Task<IEnumerable<Quote>> GetAllQuotesAsync();
        Task<IEnumerable<Quote>> SearchQuotesAsync(
            int? customerId,
            DateTime? startDate,
            DateTime? endDate,
            QuoteStatus? status,
            string? keyword);
        Task<Quote> GetQuoteByIdAsync(int quoteId);
        Task<Quote> CreateQuoteAsync(Quote quote);
        Task<Quote> UpdateQuoteAsync(Quote quote);
        Task<bool> DeleteQuoteAsync(int quoteId);
        Task<bool> ConvertToInvoiceAsync(int quoteId);
    }
}