using FMCGEnterpriseManagementSystem.Enums;
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
                PaymentTerms = model.PaymentTerms,
                SalesPersonId = model.SalesPersonId,
                Status = InvoiceStatus.Draft,
                InvoiceNumber = await _invoiceRepository.GetNextInvoiceNumberAsync()
            };

            decimal subtotal = 0;
            decimal vatTotal = 0;

            foreach (var itemVm in model.Items)
            {
                var product = await _productRepository.GetByCodeAsync(itemVm.ItemCode);
                if (product == null)
                {
                    throw new InvalidOperationException($"Product with code {itemVm.ItemCode} not found.");
                }

                var hasStock = await _inventoryRepository.HasSufficientStockAsync(product.ProductID, itemVm.Quantity);
                if (!hasStock)
                {
                    throw new InvalidOperationException($"Insufficient stock for product {itemVm.ItemCode}.");
                }

                decimal lineSubtotal = itemVm.Quantity * itemVm.UnitPrice;
                decimal discountAmount = lineSubtotal * (itemVm.DiscountPercent / 100m);
                decimal lineAfterDiscount = lineSubtotal - discountAmount;
                decimal lineVat = VatHelper.CalculateVat(lineAfterDiscount, itemVm.VatPercent);
                decimal lineTotal = lineAfterDiscount + lineVat;

                invoice.Items.Add(new InvoiceItem
                {
                    ItemCode = itemVm.ItemCode,
                    Description = itemVm.Description,
                    Quantity = itemVm.Quantity,
                    UnitPrice = itemVm.UnitPrice,
                    DiscountPercent = itemVm.DiscountPercent,
                    VatPercent = itemVm.VatPercent,
                    LineTotal = lineTotal
                });

                subtotal += lineAfterDiscount;
                vatTotal += lineVat;
            }

            invoice.Subtotal = subtotal;
            invoice.VatTotal = vatTotal;
            invoice.Total = subtotal + vatTotal;

            var saved = await _invoiceRepository.AddAsync(invoice);

            return MapToViewModel(saved);
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

        public async Task<IEnumerable<InvoiceViewModel>> SearchAsync(string customerId, DateTime? startDate, DateTime? endDate, string keyword)
        {
            var invoices = await _invoiceRepository.GetAllAsync();

            var filtered = invoices.AsEnumerable();

            if (!string.IsNullOrEmpty(customerId))
                filtered = filtered.Where(i => i.CustomerId == customerId);

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

            if (!Enum.TryParse<InvoiceStatus>(newStatus, out var statusEnum))
            {
                throw new InvalidOperationException("Invalid status value.");
            }

            var previousStatus = invoice.Status;
            invoice.Status = statusEnum;

            if (statusEnum == InvoiceStatus.Approved && previousStatus != InvoiceStatus.Approved)
            {
                foreach (var item in invoice.Items)
                {
                    var product = await _productRepository.GetByCodeAsync(item.ItemCode);
                    if (product != null)
                    {
                        await _inventoryRepository.DeductStockAsync(product.ProductID, item.Quantity);
                    }
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
            return new InvoiceViewModel
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                InvoiceDate = invoice.InvoiceDate,
                CustomerId = invoice.CustomerId,
                CustomerName = invoice.Customer?.CompanyName,
                PaymentTerms = invoice.PaymentTerms,
                SalesPersonId = invoice.SalesPersonId,
                Subtotal = invoice.Subtotal,
                VatTotal = invoice.VatTotal,
                Total = invoice.Total,
                AmountDue = invoice.Total,
                Status = invoice.Status.ToString(),
                Items = invoice.Items.Select(i => new InvoiceItemViewModel
                {
                    Id = i.Id,
                    ItemCode = i.ItemCode,
                    Description = i.Description,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    DiscountPercent = i.DiscountPercent,
                    VatPercent = i.VatPercent,
                    LineTotal = i.LineTotal
                }).ToList()
            };
        }
    }
}