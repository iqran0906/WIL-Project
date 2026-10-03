// Purpose: Business logic for invoice.
// Authors: Sayali-St10458649 (from git history)

using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IProductRepository _productRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly ISettingsService _settingsService;

        private readonly INotificationService _notificationService;

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

        public async Task<InvoiceViewModel> CreateAsync(InvoiceViewModel model)
        {
            if (model.Items == null || !model.Items.Any())
            {
                throw new InvalidOperationException("Cannot create an invoice with no items.");
            }

            // VAT rate and number prefix come from Settings
            var settings = await _settingsService.GetAsync();
            var vatRate = settings.VatRatePercent / 100m;

            var invoice = new Invoice
            {
                CustomerId = model.CustomerId ?? throw new InvalidOperationException("Please select a customer."),
                InvoiceDate = model.InvoiceDate,
                BillingAddress = model.BillingAddress,
                BusinessName = model.BusinessName,
                PaymentTerms = model.PaymentTerms,
                SalesRepresentativeId = model.SalesRepresentativeId,
                Status = "Pending",
                InvoiceNumber = await _invoiceRepository.GetNextInvoiceNumberAsync()
            };

            decimal subtotal = 0;
            decimal vatTotal = 0;

            foreach (var itemVm in model.Items)
            {
                var product = await _productRepository.GetByIdAsync(itemVm.ProductId);
                if (product == null)
                {
                    throw new InvalidOperationException($"Product with ID {itemVm.ProductId} not found.");
                }

                var hasStock = await _inventoryRepository.HasSufficientStockAsync(product.ProductId, itemVm.Quantity);
                if (!hasStock)
                {
                    throw new InvalidOperationException($"Insufficient stock for product {product.ProductCode}.");
                }

                decimal lineBeforeDiscount = itemVm.Quantity * itemVm.UnitPrice;
                decimal discountAmount = lineBeforeDiscount * (itemVm.DiscountPercent / 100m);
                decimal lineAfterDiscount = lineBeforeDiscount - discountAmount;
                decimal lineVat = itemVm.VatCategory == "[NONE]" ? 0 : lineAfterDiscount * vatRate;
                decimal lineTotal = lineAfterDiscount + lineVat;

                invoice.InvoiceItems.Add(new InvoiceItem
                {
                    ProductId = itemVm.ProductId,
                    Quantity = itemVm.Quantity,
                    UnitPrice = itemVm.UnitPrice,
                    DiscountPercent = itemVm.DiscountPercent,
                    VatCategory = itemVm.VatCategory,
                    LineTotal = lineTotal
                });

                subtotal += lineAfterDiscount;
                vatTotal += lineVat;
            }

            invoice.Subtotal = subtotal;
            invoice.Total = subtotal + vatTotal;

            var saved = await _invoiceRepository.AddAsync(invoice);

            var result = await GetByIdAsync(saved.InvoiceId);

            await _notificationService.NotifyNewInvoiceAsync(result.InvoiceNumber, result.InvoiceId, result.CustomerName);

            return result;
        }

        public async Task<InvoiceViewModel> GetByIdAsync(int id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);
            if (invoice == null) return null;

            return MapToViewModel(invoice);
        }

        public async Task<IEnumerable<InvoiceViewModel>> GetAllAsync()
        {
            var invoices = await _invoiceRepository.GetAllAsync();
            return invoices.Select(MapToViewModel);
        }

        public async Task<IEnumerable<InvoiceViewModel>> SearchAsync(int? customerId, DateTime? startDate, DateTime? endDate, string? keyword)
        {
            var invoices = await _invoiceRepository.GetAllAsync();

            var filtered = invoices.AsEnumerable();

            if (customerId.HasValue)
                filtered = filtered.Where(i => i.CustomerId == customerId.Value);

            if (startDate.HasValue)
                filtered = filtered.Where(i => i.InvoiceDate >= startDate.Value.Date);

            // Include the whole end day, not just up to midnight
            if (endDate.HasValue)
                filtered = filtered.Where(i => i.InvoiceDate < endDate.Value.Date.AddDays(1));

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();

                filtered = filtered.Where(i =>
                    i.InvoiceNumber.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    (i.BusinessName?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (i.Customer != null &&
                     $"{i.Customer.Name} {i.Customer.Surname}".Contains(keyword, StringComparison.OrdinalIgnoreCase)));
            }

            return filtered
                .OrderByDescending(i => i.InvoiceDate)
                .Select(MapToViewModel);
        }

        public async Task UpdateStatusAsync(int invoiceId, string newStatus)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);

            if (invoice == null)
            {
                throw new InvalidOperationException("Invoice not found.");
            }

            if (!string.Equals(
                    newStatus,
                    "Approved",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Invalid invoice status change.");
            }

            if (!string.Equals(
                    invoice.Status,
                    "Pending",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Only pending invoices can be approved.");
            }

            foreach (var item in invoice.InvoiceItems)
            {
                await _inventoryRepository.DeductStockAsync(
                    item.ProductId,
                    item.Quantity);
            }

            invoice.Status = "Approved";
            invoice.UpdatedAt = DateTime.UtcNow;

            await _invoiceRepository.UpdateAsync(invoice);
        }

        public async Task DeleteAsync(int id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);

            if (invoice == null)
            {
                throw new InvalidOperationException("Invoice not found.");
            }

            if (!string.Equals(
                    invoice.Status,
                    "Pending",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Approved invoices cannot be deleted.");
            }

            await _invoiceRepository.DeleteAsync(id);
        }

        private static InvoiceViewModel MapToViewModel(Invoice invoice)
        {
            decimal vatTotal = invoice.Total - invoice.Subtotal;

            return new InvoiceViewModel
            {
                InvoiceId = invoice.InvoiceId,
                InvoiceNumber = invoice.InvoiceNumber,
                InvoiceDate = invoice.InvoiceDate,
                QuoteId = invoice.QuoteId,
                CustomerId = invoice.CustomerId,
                CustomerName = invoice.Customer != null ? $"{invoice.Customer.Name} {invoice.Customer.Surname}" : null,
                BillingAddress = invoice.BillingAddress,
                BusinessName = invoice.BusinessName,
                PaymentTerms = invoice.PaymentTerms,
                SalesRepresentativeId = invoice.SalesRepresentativeId,
                Subtotal = invoice.Subtotal,
                VatTotal = vatTotal,
                Total = invoice.Total,
                AmountDue = invoice.Total,
                Status = invoice.Status,
                Items = invoice.InvoiceItems.Select(i => new InvoiceItemViewModel
                {
                    InvoiceItemId = i.InvoiceItemId,
                    ProductId = i.ProductId,
                    ItemCode = i.Product?.ProductCode,
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