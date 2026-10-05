// Title: Asynchronous programming with async and await
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using FMCGEnterpriseManagementSystem.Enums;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using FMCGEnterpriseManagementSystem.Services.Interfaces;

namespace FMCGEnterpriseManagementSystem.Services
{
    // Service responsible for quotation-related business operations.
    public class QuoteService : IQuoteService
    {
        // Repository used to manage quotation records.
        private readonly IQuoteRepository _quoteRepository;

        // Repository used to create and manage invoices.
        private readonly IInvoiceRepository _invoiceRepository;

        // Service used to create notifications for quotation and invoice events.
        private readonly INotificationService _notificationService;

        // Standard VAT rate used when calculating quotation totals.
        private const decimal VatRate = 0.15m;

        // Initialises the quotation service with the required dependencies.
        public QuoteService(IQuoteRepository quoteRepository, IInvoiceRepository invoiceRepository, INotificationService notificationService)
        {
            _quoteRepository = quoteRepository;
            _invoiceRepository = invoiceRepository;
            _notificationService = notificationService;
        }

        // Retrieves all quotations.
        public async Task<IEnumerable<Quote>> GetAllQuotesAsync()
        {
            return await _quoteRepository.GetAllAsync();
        }

        // Searches quotations using customer, date, status, and keyword filters.
        public async Task<IEnumerable<Quote>> SearchQuotesAsync(
            int? customerId,
            DateTime? startDate,
            DateTime? endDate,
            QuoteStatus? status,
            string? keyword)
        {
            // Retrieves all quotations and prepares them for filtering.
            var quotes = (await _quoteRepository.GetAllAsync()).AsEnumerable();

            // Filters quotations by customer when a customer ID is provided.
            if (customerId.HasValue)
                quotes = quotes.Where(q => q.CustomerId == customerId.Value);

            // Filters quotations from the selected start date.
            if (startDate.HasValue)
                quotes = quotes.Where(q => q.QuoteDate >= startDate.Value.Date);

            // Include the whole end day, not just up to midnight.
            if (endDate.HasValue)
                quotes = quotes.Where(q => q.QuoteDate < endDate.Value.Date.AddDays(1));

            // Filters quotations by their status.
            if (status.HasValue)
                quotes = quotes.Where(q => q.Status == status.Value);

            // Applies keyword searching when a keyword is provided.
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                // Removes unnecessary spaces from the keyword.
                keyword = keyword.Trim();

                // Searches by quote number or customer name.
                quotes = quotes.Where(q =>
                    (q.QuoteNumber?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (q.Customer != null &&
                     $"{q.Customer.Name} {q.Customer.Surname}".Contains(keyword, StringComparison.OrdinalIgnoreCase)));
            }

            // Sorts quotations from newest to oldest and returns them as a list.
            return quotes
                .OrderByDescending(q => q.QuoteDate)
                .ToList();
        }

        // Retrieves a quotation using its unique ID.
        public async Task<Quote> GetQuoteByIdAsync(int quoteId)
        {
            return await _quoteRepository.GetByIdAsync(quoteId);
        }

        // Creates a new quotation and calculates its totals.
        public async Task<Quote> CreateQuoteAsync(Quote quote)
        {
            // Generates the next quotation number.
            quote.QuoteNumber = await _quoteRepository.GenerateNextQuoteNumberAsync();

            // Newly created quotations start with a pending status.
            quote.Status = QuoteStatus.Pending;

            // Calculates the quotation subtotal, VAT, and total.
            CalculateTotals(quote, VatRate);

            // Saves the quotation through the repository.
            var saved = await _quoteRepository.AddAsync(quote);

            // Creates a notification after the quotation has been created.
            await _notificationService.NotifyNewQuoteAsync(saved.QuoteNumber, saved.QuoteId, saved.CustomerId.ToString());

            return saved;
        }

        // Updates an existing quotation after recalculating its totals.
        public async Task<Quote> UpdateQuoteAsync(Quote quote)
        {
            // Recalculates the quotation totals before saving the update.
            CalculateTotals(quote, VatRate);

            // Updates the quotation through the repository.
            return await _quoteRepository.UpdateAsync(quote);
        }

        // Deletes a quotation using its unique ID.
        public async Task<bool> DeleteQuoteAsync(int quoteId)
        {
            return await _quoteRepository.DeleteAsync(quoteId);
        }

        // Converts an existing quotation into an invoice.
        public async Task<bool> ConvertToInvoiceAsync(int quoteId)
        {
            // Retrieves the quotation that will be converted.
            var quote = await _quoteRepository.GetByIdAsync(quoteId);

            // Stops the conversion when the quotation cannot be found.
            if (quote == null)
            {
                return false;
            }

            // Creates a new invoice using information from the quotation.
            var invoice = new Invoice
            {
                // Generates the next invoice number.
                InvoiceNumber = await _invoiceRepository.GetNextInvoiceNumberAsync(),

                InvoiceDate = DateTime.Today,

                // Links the invoice back to the original quotation.
                QuoteId = quote.QuoteId,

                CustomerId = quote.CustomerId,
                BillingAddress = quote.BillingAddress,
                PaymentTerms = quote.PaymentTerms,
                SalesRepresentativeId = quote.SalesRepresentativeId,

                // New invoices created from quotations start as drafts.
                Status = "Draft",

                Subtotal = quote.Subtotal,
                Total = quote.Total,

                // Converts each quotation item into an invoice item.
                InvoiceItems = quote.QuoteItems.Select(qi => new InvoiceItem
                {
                    ProductId = qi.ProductId,
                    Quantity = qi.Quantity,
                    UnitPrice = qi.UnitPrice,
                    DiscountPercent = qi.DiscountPercent,
                    VatCategory = qi.VatCategory,
                    LineTotal = qi.LineTotal
                }).ToList()
            };

            // Saves the newly created invoice.
            await _invoiceRepository.AddAsync(invoice);

            // Marks the original quotation as invoiced.
            quote.Status = QuoteStatus.Invoiced;
            await _quoteRepository.UpdateAsync(quote);

            // Creates a notification for the newly created invoice.
            await _notificationService.NotifyNewInvoiceAsync(invoice.InvoiceNumber, invoice.InvoiceId, quote.CustomerId.ToString());

            return true;
        }

        // Calculates the subtotal and total amount for a quotation.
        private static void CalculateTotals(Quote quote, decimal vatRate)
        {
            // Stores the running subtotal before VAT.
            decimal subtotal = 0;

            // Calculates the total for each quotation item.
            foreach (var item in quote.QuoteItems)
            {
                // Calculates the line amount before applying the discount.
                var lineBeforeDiscount = item.Quantity * item.UnitPrice;

                // Calculates the discount amount for the item.
                var discountAmount = lineBeforeDiscount * (item.DiscountPercent / 100);

                // Calculates the line amount after the discount.
                var lineAfterDiscount = lineBeforeDiscount - discountAmount;

                // Applies VAT only when the item uses the STANDARD VAT category.
                var vatAmount = item.VatCategory == "STANDARD"
    ? lineAfterDiscount * VatRate
    : 0;

                // Stores the line amount excluding VAT.
                item.LineTotalExclVat = lineAfterDiscount;

                // Stores the final line total including VAT.
                item.LineTotal = lineAfterDiscount + vatAmount;

                // Adds the discounted amount to the quotation subtotal.
                subtotal += lineAfterDiscount;
            }

            // Stores the calculated quotation subtotal.
            quote.Subtotal = subtotal;

            // Calculates the final quotation total from all item line totals.
            quote.Total = quote.QuoteItems.Sum(i => i.LineTotal);
        }
    }
}