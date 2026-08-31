using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FMCGEnterpriseManagementSystem.Repositories
{
    public class QuoteRepository : IQuoteRepository
    {
        private readonly ApplicationDbContext _context;

        public QuoteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Quote>> GetAllAsync()
        {
            return await _context.Quotes
                .Include(q => q.Customer)
                .Include(q => q.QuoteItems)
                .ToListAsync();
        }

        public async Task<Quote> GetByIdAsync(int quoteId)
        {
            return await _context.Quotes
                .Include(q => q.Customer)
                .Include(q => q.QuoteItems)
                    .ThenInclude(qi => qi.Product)
                .FirstOrDefaultAsync(q => q.QuoteId == quoteId);
        }

        public async Task<Quote> AddAsync(Quote quote)
        {
            _context.Quotes.Add(quote);
            await _context.SaveChangesAsync();
            return quote;
        }

        public async Task<Quote> UpdateAsync(Quote quote)
        {
            _context.Quotes.Update(quote);
            await _context.SaveChangesAsync();
            return quote;
        }

        public async Task<bool> DeleteAsync(int quoteId)
        {
            var quote = await _context.Quotes.FindAsync(quoteId);
            if (quote == null)
            {
                return false;
            }

            _context.Quotes.Remove(quote);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string> GenerateNextQuoteNumberAsync()
        {
            var count = await _context.Quotes.CountAsync();
            var nextNumber = count + 1;
            return $"ED{nextNumber:D5}";
        }
    }
}