using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services
{
    public class InvoiceService : IInvoiceService
    {
        private const decimal VatRate = 0.15m;

        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IProductRepository _productRepository;
        private readonly IInventoryRepository _inventoryRepository;

        public InvoiceService(
            IInvoiceRepository invoiceRepository,
            IProductRepository productRepository,
            IInventoryRepository inventoryRepository)
        {
            _invoiceRepository = invoiceRepository;
            _productRepository = productRepository;
            _inventoryRepository = inventoryRepository;
        }

        public async Task<InvoiceViewModel> CreateAsync(InvoiceViewModel model)
        {
            if (model.Items == null || !model.Items.Any())
            {
                throw new InvalidOperationException("Cannot create an invoice with no items.");
            }

            var invoice = new Invoice
            {
                CustomerId = model.CustomerId,
                InvoiceDate = model.InvoiceDate,
                BillingAddress = model.BillingAddress,
                PaymentTerms = model.PaymentTerms,
                SalesRepresentativeId = model.SalesRepresentativeId,
                Status = "Draft",
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
                decimal lineVat = itemVm.VatCategory == "[NONE]" ? 0 : lineAfterDiscount * VatRate;
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

            return await GetByIdAsync(saved.InvoiceId);
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

        public async Task<IEnumerable<InvoiceViewModel>> SearchAsync(int? customerId, DateTime? startDate, DateTime? endDate, string keyword)
        {
            var invoices = await _invoiceRepository.GetAllAsync();

            var filtered = invoices.AsEnumerable();

            if (customerId.HasValue)
                filtered = filtered.Where(i => i.CustomerId == customerId.Value);

            if (startDate.HasValue)
                filtered = filtered.Where(i => i.InvoiceDate >= startDate.Value);

            if (endDate.HasValue)
                filtered = filtered.Where(i => i.InvoiceDate <= endDate.Value);

            if (!string.IsNullOrEmpty(keyword))
                filtered = filtered.Where(i => i.InvoiceNumber.Contains(keyword, StringComparison.OrdinalIgnoreCase));

            return filtered.Select(MapToViewModel);
        }

        public async Task UpdateStatusAsync(int invoiceId, string newStatus)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);
            if (invoice == null)
            {
                throw new InvalidOperationException("Invoice not found.");
            }

            var previousStatus = invoice.Status;
            invoice.Status = newStatus;

            if (newStatus == "Approved" && previousStatus != "Approved")
            {
                foreach (var item in invoice.InvoiceItems)
                {
                    await _inventoryRepository.DeductStockAsync(item.ProductId, item.Quantity);
                }
            }

            await _invoiceRepository.UpdateAsync(invoice);
        }

        public async Task DeleteAsync(int id)
        {
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