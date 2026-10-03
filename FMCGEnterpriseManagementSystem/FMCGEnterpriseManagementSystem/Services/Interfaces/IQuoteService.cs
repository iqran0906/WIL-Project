/***************************************************************************************
*    Title: Quote Service Interface
*    Author: Sayali-St10458649
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Services/Interfaces/IQuoteService.cs
***************************************************************************************/

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