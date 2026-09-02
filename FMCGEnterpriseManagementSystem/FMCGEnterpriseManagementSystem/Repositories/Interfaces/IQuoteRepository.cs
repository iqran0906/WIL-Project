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
        Task<string> GenerateNextQuoteNumberAsync();
    }
}