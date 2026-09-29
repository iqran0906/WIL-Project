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
                .Include(q => q.SalesRepresentative)
                    .ThenInclude(sr => sr.Employee)
                .Include(q => q.QuoteItems)
                    .ThenInclude(qi => qi.Product)
                .ToListAsync();
        }

        public async Task<Quote?> GetByIdAsync(int quoteId)
        {
            return await _context.Quotes
                .Include(q => q.Customer)
                .Include(q => q.SalesRepresentative)
                    .ThenInclude(sr => sr.Employee)
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
           

            var trackedEntry = _context.ChangeTracker
                .Entries<Quote>()
                .FirstOrDefault(e => e.Entity.QuoteId == quote.QuoteId);

          
            if (trackedEntry != null &&
                ReferenceEquals(trackedEntry.Entity, quote))
            {
                quote.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return quote;
            }

           

            var existingQuote = trackedEntry?.Entity;

            if (existingQuote == null)
            {
                existingQuote = await _context.Quotes
                    .Include(q => q.QuoteItems)
                    .FirstOrDefaultAsync(q => q.QuoteId == quote.QuoteId);
            }
            else
            {
                await _context.Entry(existingQuote)
                    .Collection(q => q.QuoteItems)
                    .LoadAsync();
            }

            if (existingQuote == null)
            {
                throw new KeyNotFoundException(
                    $"Quote with ID {quote.QuoteId} was not found.");
            }

            
            var submittedItems = quote.QuoteItems
                .Select(item => new QuoteItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    DiscountPercent = item.DiscountPercent,
                    VatCategory = item.VatCategory,
                    LineTotalExclVat = item.LineTotalExclVat,
                    LineTotal = item.LineTotal
                })
                .ToList();

            // ---------------------------------------------------------
            // Update quote header
            // ---------------------------------------------------------

            existingQuote.QuoteDate = quote.QuoteDate;
            existingQuote.CustomerId = quote.CustomerId;
            existingQuote.BillingAddress = quote.BillingAddress;
            existingQuote.PaymentTerms = quote.PaymentTerms;
            existingQuote.SalesRepresentativeId = quote.SalesRepresentativeId;
            existingQuote.Status = quote.Status;
            existingQuote.Subtotal = quote.Subtotal;
            existingQuote.Total = quote.Total;
            existingQuote.UpdatedAt = DateTime.UtcNow;

            

            var oldItems = existingQuote.QuoteItems.ToList();

            if (oldItems.Count > 0)
            {
                _context.QuoteItems.RemoveRange(oldItems);
            }

            existingQuote.QuoteItems.Clear();

            foreach (var submittedItem in submittedItems)
            {
                existingQuote.QuoteItems.Add(submittedItem);
            }

            await _context.SaveChangesAsync();

            return existingQuote;
        }

        public async Task<bool> DeleteAsync(int quoteId)
        {
            var quote = await _context.Quotes
                .Include(q => q.QuoteItems)
                .FirstOrDefaultAsync(q => q.QuoteId == quoteId);

            if (quote == null)
            {
                return false;
            }

            if (quote.QuoteItems.Any())
            {
                _context.QuoteItems.RemoveRange(
                    quote.QuoteItems.ToList());
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