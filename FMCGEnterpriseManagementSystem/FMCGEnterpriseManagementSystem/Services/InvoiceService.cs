// Title: Asynchronous programming with async and await
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services
{

    // Title: Dependency injection in ASP.NET Core
    // Author: Microsoft
    // Date: 2026
    // Code version: ASP.NET Core 10.0
    // Availability: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection

    // Provides business logic for creating, retrieving,
    // searching, updating and deleting invoices.
    public class InvoiceService : IInvoiceService
    {
        // Repository used to access invoice data.
        private readonly IInvoiceRepository _invoiceRepository;

        // Repository used to retrieve product information.
        private readonly IProductRepository _productRepository;

        // Repository used to check and update inventory quantities.
        private readonly IInventoryRepository _inventoryRepository;

        // Service used to retrieve application-wide settings,
        // including the configured VAT rate.
        private readonly ISettingsService _settingsService;

        // Service used to create notifications for invoice events.
        private readonly INotificationService _notificationService;

        // Constructor receives all required dependencies through
        // dependency injection.
        public InvoiceService(
            IInvoiceRepository invoiceRepository,
            IProductRepository productRepository,
            IInventoryRepository inventoryRepository,
            INotificationService notificationService,
            ISettingsService settingsService)
        {
            _invoiceRepository = invoiceRepository;
            _productRepository = productRepository;
            _inventoryRepository = inventoryRepository;
            _notificationService = notificationService;
            _settingsService = settingsService;
        }

        // Creates a new invoice from the supplied view model.

        // Title: Exception handling
        // Author: Microsoft
        // Date: 2026
        // Code version: C# / .NET 10
        // Availability: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/exceptions/
        public async Task<InvoiceViewModel> CreateAsync(InvoiceViewModel model)
        {
            // An invoice must contain at least one item.
            if (model.Items == null || !model.Items.Any())
            {
                throw new InvalidOperationException(
                    "Cannot create an invoice with no items.");
            }

            // Retrieve the current system settings.
            // The VAT rate is controlled through the application's settings.
            var settings = await _settingsService.GetAsync();

            // Convert the stored VAT percentage into a decimal value.
            var vatRate = settings.VatRatePercent / 100m;

            // Create the invoice entity using information from the view model.
            var invoice = new Invoice
            {
                // A customer must be selected before an invoice can be created.
                CustomerId = model.CustomerId
                    ?? throw new InvalidOperationException(
                        "Please select a customer."),

                // Copies the invoice date supplied by the user.
                InvoiceDate = model.InvoiceDate,

                // Copies the billing address.
                BillingAddress = model.BillingAddress,

                // Copies the optional business name.
                BusinessName = model.BusinessName,

                // Copies the selected payment terms.
                PaymentTerms = model.PaymentTerms,

                // Stores the selected sales representative.
                SalesRepresentativeId = model.SalesRepresentativeId,

                // New invoices initially have a Pending status.
                Status = "Pending",

                // Generates the next invoice number through the repository.
                InvoiceNumber =
                    await _invoiceRepository.GetNextInvoiceNumberAsync()
            };

            // Stores the total value of invoice items before VAT.
            decimal subtotal = 0;

            // Stores the accumulated VAT amount.
            decimal vatTotal = 0;

            // Processes each item included in the invoice.
            foreach (var itemVm in model.Items)
            {
                // Retrieves the product associated with the invoice item.
                var product =
                    await _productRepository.GetByIdAsync(itemVm.ProductId);

                // The invoice cannot contain a product that does not exist.
                if (product == null)
                {
                    throw new InvalidOperationException(
                        $"Product with ID {itemVm.ProductId} not found.");
                }

                // Checks whether enough stock is available for the requested quantity.
                var hasStock =
                    await _inventoryRepository.HasSufficientStockAsync(
                        product.ProductId,
                        itemVm.Quantity);

                // Prevents an invoice from being created when there is
                // insufficient stock.
                if (!hasStock)
                {
                    throw new InvalidOperationException(
                        $"Insufficient stock for product {product.ProductCode}.");
                }

                // Calculates the item value before applying the discount.
                decimal lineBeforeDiscount =
                    itemVm.Quantity * itemVm.UnitPrice;

                // Calculates the discount amount using the item's
                // configured discount percentage.
                decimal discountAmount =
                    lineBeforeDiscount *
                    (itemVm.DiscountPercent / 100m);

                // Calculates the item value after the discount.
                decimal lineAfterDiscount =
                    lineBeforeDiscount - discountAmount;

                // Calculates VAT on the discounted amount.
                // Items marked as [NONE] do not receive VAT.
                decimal lineVat =
                    itemVm.VatCategory == "[NONE]"
                        ? 0
                        : lineAfterDiscount * vatRate;

                // Calculates the final line total including VAT.
                decimal lineTotal =
                    lineAfterDiscount + lineVat;

                // Adds the processed item to the invoice.
                invoice.InvoiceItems.Add(new InvoiceItem
                {
                    ProductId = itemVm.ProductId,
                    Quantity = itemVm.Quantity,
                    UnitPrice = itemVm.UnitPrice,
                    DiscountPercent = itemVm.DiscountPercent,
                    VatCategory = itemVm.VatCategory,
                    LineTotal = lineTotal
                });

                // Adds the discounted item value to the invoice subtotal.
                subtotal += lineAfterDiscount;

                // Adds the item's VAT to the overall VAT total.
                vatTotal += lineVat;
            }

            // Stores the calculated invoice subtotal.
            invoice.Subtotal = subtotal;

            // Calculates and stores the final invoice total.
            invoice.Total = subtotal + vatTotal;

            // Saves the invoice through the repository.
            var saved = await _invoiceRepository.AddAsync(invoice);

            // Retrieves the saved invoice again so that the returned
            // view model contains the complete related information.
            var result = await GetByIdAsync(saved.InvoiceId);

            // Creates a notification informing the system that
            // a new invoice has been created.
            await _notificationService.NotifyNewInvoiceAsync(
                result.InvoiceNumber,
                result.InvoiceId,
                result.CustomerName);

            // Returns the completed invoice view model.
            return result;
        }

        // Retrieves one invoice using its database ID.
        public async Task<InvoiceViewModel> GetByIdAsync(int id)
        {
            // Retrieves the invoice through the repository.
            var invoice = await _invoiceRepository.GetByIdAsync(id);

            // Returns null when the invoice does not exist.
            if (invoice == null)
                return null;

            // Converts the entity into a view model.
            return MapToViewModel(invoice);
        }

        // Title: Enumerable.Select Method
        // Author: Microsoft
        // Date: 2026
        // Code version: .NET 10
        // Availability: https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.select

        // Retrieves all invoices.
        public async Task<IEnumerable<InvoiceViewModel>> GetAllAsync()
        {
            // Retrieves invoice entities from the repository.
            var invoices = await _invoiceRepository.GetAllAsync();

            // Converts the entities into view models.
            return invoices.Select(MapToViewModel);
        }


        // Title: LINQ (Language-Integrated Query)
        // Author: Microsoft
        // Date: 2026
        // Code version: C# / .NET 10
        // Availability: https://learn.microsoft.com/en-us/dotnet/csharp/linq/

        // Searches invoices using optional customer, date and keyword filters.
        public async Task<IEnumerable<InvoiceViewModel>> SearchAsync(
            int? customerId,
            DateTime? startDate,
            DateTime? endDate,
            string? keyword)
        {
            // Retrieves all invoices from the repository.
            var invoices = await _invoiceRepository.GetAllAsync();

            // Converts the collection to an enumerable so filters
            // can be applied conditionally.
            var filtered = invoices.AsEnumerable();

            // Filters by customer when a customer ID was supplied.
            if (customerId.HasValue)
                filtered = filtered.Where(
                    i => i.CustomerId == customerId.Value);

            // Filters invoices from the selected start date onwards.
            if (startDate.HasValue)
                filtered = filtered.Where(
                    i => i.InvoiceDate >= startDate.Value.Date);

            // Includes the entire end date rather than stopping at midnight.
            if (endDate.HasValue)
                filtered = filtered.Where(
                    i => i.InvoiceDate <
                         endDate.Value.Date.AddDays(1));

            // Applies the keyword search when a keyword was supplied.
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                // Removes unnecessary whitespace from the search term.
                keyword = keyword.Trim();

                // Searches invoice number, business name and customer name.
                filtered = filtered.Where(i =>
                    i.InvoiceNumber.Contains(
                        keyword,
                        StringComparison.OrdinalIgnoreCase) ||

                    (i.BusinessName?.Contains(
                        keyword,
                        StringComparison.OrdinalIgnoreCase) ?? false) ||

                    (i.Customer != null &&
                     $"{i.Customer.Name} {i.Customer.Surname}"
                         .Contains(
                             keyword,
                             StringComparison.OrdinalIgnoreCase)));
            }

            // Sorts invoices from newest to oldest
            // and converts them into view models.
            return filtered
                .OrderByDescending(i => i.InvoiceDate)
                .Select(MapToViewModel);
        }

        // Updates the status of an invoice.
        public async Task UpdateStatusAsync(
            int invoiceId,
            string newStatus)
        {
            // Retrieves the invoice that needs to be updated.
            var invoice =
                await _invoiceRepository.GetByIdAsync(invoiceId);

            // The invoice must exist before its status can be changed.
            if (invoice == null)
            {
                throw new InvalidOperationException(
                    "Invoice not found.");
            }

            // The only supported status change through this method
            // is approval.
            if (!string.Equals(
                    newStatus,
                    "Approved",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Invalid invoice status change.");
            }

            // Only invoices currently marked as Pending
            // can be approved.
            if (!string.Equals(
                    invoice.Status,
                    "Pending",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Only pending invoices can be approved.");
            }

            // Deducts the quantity of every invoice item from inventory.
            foreach (var item in invoice.InvoiceItems)
            {
                await _inventoryRepository.DeductStockAsync(
                    item.ProductId,
                    item.Quantity);
            }

            // Changes the invoice status to Approved.
            invoice.Status = "Approved";

            // Records when the invoice was updated.
            invoice.UpdatedAt = DateTime.UtcNow;

            // Saves the updated invoice.
            await _invoiceRepository.UpdateAsync(invoice);
        }

        // Deletes an invoice using its ID.
        public async Task DeleteAsync(int id)
        {
            // Retrieves the invoice before attempting deletion.
            var invoice = await _invoiceRepository.GetByIdAsync(id);

            // The invoice must exist.
            if (invoice == null)
            {
                throw new InvalidOperationException(
                    "Invoice not found.");
            }

            // Only pending invoices can be deleted.
            // Approved invoices are retained as completed records.
            if (!string.Equals(
                    invoice.Status,
                    "Pending",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Approved invoices cannot be deleted.");
            }

            // Deletes the invoice through the repository.
            await _invoiceRepository.DeleteAsync(id);
        }

        public async Task<bool> SendInvoiceEmailAsync(int invoiceId)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);
            if (invoice == null)
            {
                throw new InvalidOperationException("Invoice not found.");
            }

            if (invoice.Customer == null || string.IsNullOrWhiteSpace(invoice.Customer.Email))
            {
                throw new InvalidOperationException("This customer has no email address on file.");
            }

            // TODO: once the Email API project is merged in, replace this
            // with a real call (e.g. via HttpClient to the API's /email endpoint).
            // For now this is a placeholder so the UI flow can be tested end-to-end.
            await Task.Delay(300); // simulates a network call
            return true;
        }

        private static InvoiceViewModel MapToViewModel(Invoice invoice)
        {
            // Calculates VAT by subtracting the subtotal from the total.
            decimal vatTotal =
                invoice.Total - invoice.Subtotal;

            // Calculates the amount already paid against the invoice.
            decimal totalPaid =
                invoice.Payments?.Sum(p => p.AmountPaid) ?? 0m;

            // Calculates the remaining amount owed.
            decimal amountDue =
                invoice.Total - totalPaid;

            // Prevents the amount due from becoming negative,
            // for example when payments exceed the invoice total.
            if (amountDue < 0)
            {
                amountDue = 0;
            }

            // Creates the view model used by the presentation layer.
            return new InvoiceViewModel
            {
                InvoiceId = invoice.InvoiceId,
                InvoiceNumber = invoice.InvoiceNumber,
                InvoiceDate = invoice.InvoiceDate,
                QuoteId = invoice.QuoteId,
                CustomerId = invoice.CustomerId,

                // Builds the customer's full name when customer information exists.
                CustomerName = invoice.Customer != null
                    ? $"{invoice.Customer.Name} {invoice.Customer.Surname}"
                    : null,

                BillingAddress = invoice.BillingAddress,
                BusinessName = invoice.BusinessName,
                PaymentTerms = invoice.PaymentTerms,
                SalesRepresentativeId = invoice.SalesRepresentativeId,

                // Copies the calculated financial values.
                Subtotal = invoice.Subtotal,
                VatTotal = vatTotal,
                Total = invoice.Total,
                AmountDue = amountDue,

                // Copies the current invoice status.
                Status = invoice.Status,

                // Converts each invoice item into an item view model.
                Items = invoice.InvoiceItems.Select(i =>
                    new InvoiceItemViewModel
                    {
                        InvoiceItemId = i.InvoiceItemId,
                        ProductId = i.ProductId,

                        // Gets the product code when the related product exists.
                        ItemCode = i.Product?.ProductCode,

                        // Gets the product name when the related product exists.
                        Description = i.Product?.ProductName,

                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        DiscountPercent = i.DiscountPercent,
                        VatCategory = i.VatCategory,
                        LineTotal = i.LineTotal
                    }).ToList()
            };
        }
    }
}
