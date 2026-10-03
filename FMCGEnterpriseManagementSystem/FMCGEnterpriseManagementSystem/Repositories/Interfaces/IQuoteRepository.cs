/***************************************************************************************
*    Title: Quote Repository Interface
*    Author: Sayali-St10458649
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Repositories/Interfaces/IQuoteRepository.cs
***************************************************************************************/
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