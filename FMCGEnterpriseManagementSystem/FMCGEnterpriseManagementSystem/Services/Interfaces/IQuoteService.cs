// Title: Asynchronous programming with async and await
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using FMCGEnterpriseManagementSystem.Enums;
using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    // Defines the operations used to manage customer quotations.
    public interface IQuoteService
    {
        // Retrieves all quotes from the system.
        Task<IEnumerable<Quote>> GetAllQuotesAsync();

        // Searches and filters quotes using customer, date range,
        // status and keyword criteria.
        Task<IEnumerable<Quote>> SearchQuotesAsync(
            int? customerId,
            DateTime? startDate,
            DateTime? endDate,
            QuoteStatus? status,
            string? keyword);

        // Retrieves a specific quote using its ID.
        Task<Quote> GetQuoteByIdAsync(int quoteId);

        // Creates a new quote.
        Task<Quote> CreateQuoteAsync(Quote quote);

        // Updates an existing quote.
        Task<Quote> UpdateQuoteAsync(Quote quote);

        // Deletes a quote using its ID and returns whether
        // the operation was successful.
        Task<bool> DeleteQuoteAsync(int quoteId);

        // Converts an existing quote into an invoice.
        // Returns whether the conversion was successful.
        Task<bool> ConvertToInvoiceAsync(int quoteId);
    }
}