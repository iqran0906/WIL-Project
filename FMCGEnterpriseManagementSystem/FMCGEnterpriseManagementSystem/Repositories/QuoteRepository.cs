// Title: Implement CRUD - ASP.NET Core MVC with Entity Framework Core
// Author: Microsoft
// Date: 10-04-2024
// Code version: ASP.NET Core 10.0
// Availability: https://learn.microsoft.com/en-us/aspnet/core/data/ef-mvc/crud

using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FMCGEnterpriseManagementSystem.Repositories
{
    // Repository responsible for managing quotes and their related quote items.
    public class QuoteRepository : IQuoteRepository
    {
        // Provides access to the application's database.
        private readonly ApplicationDbContext _context;

        // Constructor receives the database context through dependency injection.
        public QuoteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Retrieves all quotes together with their related customer, sales representative, employee, quote items and products.
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

        // Retrieves a specific quote by its ID together with all related information required to display the quote.
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

        // Adds a new quote to the database and saves the changes.
        public async Task<Quote> AddAsync(Quote quote)
        {
            _context.Quotes.Add(quote);

            await _context.SaveChangesAsync();

            return quote;
        }

        // Updates an existing quote and its associated quote items.
        public async Task<Quote> UpdateAsync(Quote quote)
        {
            // Checks whether the quote is already being tracked
            // by the current Entity Framework Core context.
            var trackedEntry = _context.ChangeTracker
                .Entries<Quote>()
                .FirstOrDefault(e => e.Entity.QuoteId == quote.QuoteId);

            // If the submitted quote is already the tracked entity, its changes can be saved directly.
            if (trackedEntry != null &&
                ReferenceEquals(trackedEntry.Entity, quote))
            {
                quote.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return quote;
            }

            // Attempts to use an already tracked quote when available.
            var existingQuote = trackedEntry?.Entity;

            // If the quote is not already tracked, retrieve it from the database together with its existing quote items.
            if (existingQuote == null)
            {
                existingQuote = await _context.Quotes
                    .Include(q => q.QuoteItems)
                    .FirstOrDefaultAsync(q => q.QuoteId == quote.QuoteId);
            }
            else
            {
                // Loads the quote items when the quote is already tracked.
                await _context.Entry(existingQuote)
                    .Collection(q => q.QuoteItems)
                    .LoadAsync();
            }

            // Prevents an update from being performed when the quote does not exist in the database.
            if (existingQuote == null)
            {
                throw new KeyNotFoundException(
                    $"Quote with ID {quote.QuoteId} was not found.");
            }

            // Creates a new collection of quote items using the values submitted for the updated quote.
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

            // Updates the main quote information with the submitted values.
            existingQuote.QuoteDate = quote.QuoteDate;
            existingQuote.CustomerId = quote.CustomerId;
            existingQuote.BillingAddress = quote.BillingAddress;
            existingQuote.PaymentTerms = quote.PaymentTerms;
            existingQuote.SalesRepresentativeId = quote.SalesRepresentativeId;
            existingQuote.Status = quote.Status;
            existingQuote.Subtotal = quote.Subtotal;
            existingQuote.Total = quote.Total;
            existingQuote.UpdatedAt = DateTime.UtcNow;

            // Stores the current quote items before replacing them.
            var oldItems = existingQuote.QuoteItems.ToList();

            // Removes the previous quote items from the database.
            if (oldItems.Count > 0)
            {
                _context.QuoteItems.RemoveRange(oldItems);
            }

            // Clears the existing quote item collection.
            existingQuote.QuoteItems.Clear();

            // Adds the newly submitted quote items to the quote.
            foreach (var submittedItem in submittedItems)
            {
                existingQuote.QuoteItems.Add(submittedItem);
            }

            // Saves the updated quote and quote items to the database.
            await _context.SaveChangesAsync();

            return existingQuote;
        }

        // Deletes a quote and its associated quote items.
        public async Task<bool> DeleteAsync(int quoteId)
        {
            // Retrieves the quote together with its quote items.
            var quote = await _context.Quotes
                .Include(q => q.QuoteItems)
                .FirstOrDefaultAsync(q => q.QuoteId == quoteId);

            // Returns false when the requested quote does not exist.
            if (quote == null)
            {
                return false;
            }

            // Removes any quote items associated with the quote.
            if (quote.QuoteItems.Any())
            {
                _context.QuoteItems.RemoveRange(
                    quote.QuoteItems.ToList());
            }

            // Removes the main quote record.
            _context.Quotes.Remove(quote);

            // Saves the deletion changes to the database.
            await _context.SaveChangesAsync();

            return true;
        }

        // Generates the next quote number using the specified prefix.
        public async Task<string> GenerateNextQuoteNumberAsync(string prefix = "ED")
        {
            // Counts the existing quotes to determine the next number.
            var count = await _context.Quotes.CountAsync();

            // Increments the count to generate the next quote number.
            var nextNumber = count + 1;

            // Formats the number using five digits and adds the prefix.
            return $"{prefix}{nextNumber:D5}";
        }
    }
}