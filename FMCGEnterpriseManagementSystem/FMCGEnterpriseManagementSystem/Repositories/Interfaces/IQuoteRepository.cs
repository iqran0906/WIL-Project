// Title: Implement CRUD - ASP.NET Core MVC with Entity Framework Core
// Author: Microsoft
// Date: 10-04-2024
// Code version: ASP.NET Core 10.0
// Availability: https://learn.microsoft.com/en-us/aspnet/core/data/ef-mvc/crud

using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.Repositories.Interfaces
{
    // Purpose: Defines the database operations required for quotation management.
    public interface IQuoteRepository
    {
        // Retrieves all quotations.
        Task<IEnumerable<Quote>> GetAllAsync();

        // Retrieves a quotation by its ID.
        Task<Quote> GetByIdAsync(int quoteId);

        // Adds a new quotation and returns the created quotation.
        Task<Quote> AddAsync(Quote quote);

        // Updates an existing quotation and returns the updated quotation.
        Task<Quote> UpdateAsync(Quote quote);

        // Deletes a quotation using its ID and returns whether the operation succeeded.
        Task<bool> DeleteAsync(int quoteId);

        // Generates the next quotation number using the specified prefix.
        Task<string> GenerateNextQuoteNumberAsync(string prefix = "ED");
    }
}