// Purpose: Contract (interface) for quote data access.
// Authors: Sayali-St10458649 (from git history)

using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.Repositories.Interfaces
{
    public interface IQuoteRepository
    {
        Task<IEnumerable<Quote>> GetAllAsync();
        Task<Quote> GetByIdAsync(int quoteId);
        Task<Quote> AddAsync(Quote quote);
        Task<Quote> UpdateAsync(Quote quote);
        Task<bool> DeleteAsync(int quoteId);
        Task<string> GenerateNextQuoteNumberAsync(string prefix = "ED");
    }
}