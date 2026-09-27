using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels.Reports;

namespace FMCGEnterpriseManagementSystem.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;

        public ReportService(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<IEnumerable<InvoiceReportViewModel>> GetInvoiceReportAsync(
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var invoices = await _reportRepository
                .GetInvoicesAsync(startDate, endDate);

            return invoices.Select(invoice =>
            {
                var amountPaid = invoice.Payments.Sum(p => p.AmountPaid);

                return new InvoiceReportViewModel
                {
                    InvoiceId = invoice.InvoiceId,
                    InvoiceNumber = invoice.InvoiceNumber,
                    InvoiceDate = invoice.InvoiceDate,
                    CustomerName =
                        $"{invoice.Customer.Name} {invoice.Customer.Surname}".Trim(),
                    Status = invoice.Status,
                    Subtotal = invoice.Subtotal,
                    Total = invoice.Total,
                    AmountPaid = amountPaid,
                    OutstandingBalance =
                        Math.Max(0, invoice.Total - amountPaid)
                };
            });
        }

        public async Task<IEnumerable<QuoteReportViewModel>> GetQuoteReportAsync(
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var quotes = await _reportRepository
                .GetQuotesAsync(startDate, endDate);

            return quotes.Select(q => new QuoteReportViewModel
            {
                QuoteId = q.QuoteId,
                QuoteNumber = q.QuoteNumber,
                QuoteDate = q.QuoteDate,
                CustomerName =
                    $"{q.Customer.Name} {q.Customer.Surname}".Trim(),
                Status = q.Status.ToString(),
                Subtotal = q.Subtotal,
                Total = q.Total
            });
        }

        public async Task<IEnumerable<AgeAnalysisReportViewModel>>
            GetAgeAnalysisReportAsync()
        {
            var invoices = await _reportRepository.GetInvoicesAsync();

            var today = DateTime.Today;

            return invoices
                .Select(invoice =>
                {
                    var paid = invoice.Payments.Sum(p => p.AmountPaid);

                    var outstanding =
                        Math.Max(0, invoice.Total - paid);

                    var age = Math.Max(
                        0,
                        (today - invoice.InvoiceDate.Date).Days);

                    return new
                    {
                        Invoice = invoice,
                        Paid = paid,
                        Outstanding = outstanding,
                        Age = age
                    };
                })
                .Where(x => x.Outstanding > 0)
                .Select(x => new AgeAnalysisReportViewModel
                {
                    InvoiceNumber = x.Invoice.InvoiceNumber,

                    CustomerName =
                        $"{x.Invoice.Customer.Name} " +
                        $"{x.Invoice.Customer.Surname}".Trim(),

                    InvoiceDate = x.Invoice.InvoiceDate,

                    AgeInDays = x.Age,

                    InvoiceTotal = x.Invoice.Total,

                    AmountPaid = x.Paid,

                    OutstandingBalance = x.Outstanding,

                    AgeBracket = GetAgeBracket(x.Age)
                })
                .OrderByDescending(x => x.AgeInDays);
        }

        public async Task<IEnumerable<CustomerReportViewModel>>
            GetCustomerReportAsync()
        {
            var customers =
                await _reportRepository.GetCustomersAsync();

            return customers.Select(c => new CustomerReportViewModel
            {
                CustomerId = c.CustomerId,
                CustomerName = $"{c.Name} {c.Surname}".Trim(),
                Email = c.Email,
                TelephoneNumber = c.TelephoneNumber,
                CustomerGroup = c.CustomerGroup,
                PaymentTerms = c.PaymentTerms,
                IsActive = c.IsActive
            });
        }

        public async Task<IEnumerable<CustomerSalesReportViewModel>>
            GetCustomerSalesReportAsync(
                DateTime? startDate = null,
                DateTime? endDate = null)
        {
            var invoices = await _reportRepository
                .GetInvoicesAsync(startDate, endDate);

            return invoices
                .GroupBy(i => new
                {
                    i.CustomerId,
                    i.Customer.Name,
                    i.Customer.Surname
                })
                .Select(group =>
                {
                    var sales = group.Sum(i => i.Total);

                    var paid = group.Sum(i =>
                        i.Payments.Sum(p => p.AmountPaid));

                    return new CustomerSalesReportViewModel
                    {
                        CustomerId = group.Key.CustomerId,

                        CustomerName =
                            $"{group.Key.Name} {group.Key.Surname}".Trim(),

                        InvoiceCount = group.Count(),

                        TotalSales = sales,

                        TotalPaid = paid,

                        OutstandingBalance =
                            Math.Max(0, sales - paid)
                    };
                })
                .OrderByDescending(x => x.TotalSales);
        }

        public async Task<IEnumerable<PaymentReportViewModel>>
            GetPaymentReportAsync(
                DateTime? startDate = null,
                DateTime? endDate = null)
        {
            var payments = await _reportRepository
                .GetPaymentsAsync(startDate, endDate);

            return payments.Select(p => new PaymentReportViewModel
            {
                PaymentId = p.PaymentId,
                PaymentDate = p.PaymentDate,
                InvoiceNumber = p.Invoice.InvoiceNumber,

                CustomerName =
                    $"{p.Invoice.Customer.Name} " +
                    $"{p.Invoice.Customer.Surname}".Trim(),

                PaymentMethod = p.PaymentMethod,
                AmountPaid = p.AmountPaid
            });
        }

        public async Task<IEnumerable<InventoryReportViewModel>>
            GetInventoryReportAsync()
        {
            var inventory =
                await _reportRepository.GetInventoryAsync();

            return inventory.Select(i =>
            {
                var earliestExpiry = i.StockBatches
                    .Where(b => b.Quantity > 0)
                    .Select(b => (DateTime?)b.ExpiryDate)
                    .Min();

                return new InventoryReportViewModel
                {
                    ProductId = i.ProductId,
                    ProductCode = i.Product.ProductCode,
                    ProductName = i.Product.ProductName,
                    Category = i.Product.Category,
                    QuantityOnHand = i.QuantityOnHand,
                    ReorderLevel = i.ReorderLevel,
                    SellingPrice = i.Product.SellingPrice,

                    StockValue =
                        i.QuantityOnHand * i.Product.SellingPrice,

                    IsLowStock =
                        i.QuantityOnHand <= i.ReorderLevel,

                    EarliestExpiryDate = earliestExpiry
                };
            });
        }

        public async Task<IEnumerable<SalesReportViewModel>>
            GetSalesReportAsync(
                DateTime? startDate = null,
                DateTime? endDate = null)
        {
            var invoices = await _reportRepository
                .GetInvoicesAsync(startDate, endDate);

            return invoices
                .GroupBy(i => i.InvoiceDate.Date)
                .Select(group =>
                {
                    var totalSales = group.Sum(i => i.Total);

                    var totalPaid = group.Sum(i =>
                        i.Payments.Sum(p => p.AmountPaid));

                    return new SalesReportViewModel
                    {
                        SaleDate = group.Key,
                        InvoiceCount = group.Count(),
                        TotalSales = totalSales,
                        TotalPaid = totalPaid,

                        OutstandingBalance =
                            Math.Max(0, totalSales - totalPaid)
                    };
                })
                .OrderByDescending(x => x.SaleDate);
        }

        public async Task<IEnumerable<ItemsPerCustomerReportViewModel>>
            GetItemsPerCustomerReportAsync(
                DateTime? startDate = null,
                DateTime? endDate = null)
        {
            var invoices = await _reportRepository
                .GetInvoicesAsync(startDate, endDate);

            return invoices
                .GroupBy(i => new
                {
                    i.CustomerId,
                    i.Customer.Name,
                    i.Customer.Surname
                })
                .Select(group => new ItemsPerCustomerReportViewModel
                {
                    CustomerId = group.Key.CustomerId,

                    CustomerName =
                        $"{group.Key.Name} {group.Key.Surname}".Trim(),

                    TotalItemsPurchased =
                        group.SelectMany(i => i.InvoiceItems)
                             .Sum(item => item.Quantity),

                    TotalSales = group.Sum(i => i.Total)
                })
                .OrderByDescending(x => x.TotalItemsPurchased);
        }

        public async Task<IEnumerable<ItemSalesReportViewModel>>
            GetItemSalesReportAsync(
                DateTime? startDate = null,
                DateTime? endDate = null)
        {
            var invoices = await _reportRepository
                .GetInvoicesAsync(startDate, endDate);

            return invoices
                .SelectMany(i => i.InvoiceItems)
                .GroupBy(item => new
                {
                    item.ProductId,
                    item.Product.ProductCode,
                    item.Product.ProductName
                })
                .Select(group => new ItemSalesReportViewModel
                {
                    ProductId = group.Key.ProductId,
                    ProductCode = group.Key.ProductCode,
                    ProductName = group.Key.ProductName,

                    QuantitySold =
                        group.Sum(item => item.Quantity),

                    TotalSales =
                        group.Sum(item => item.LineTotal)
                })
                .OrderByDescending(x => x.TotalSales);
        }

        public async Task<IEnumerable<SalesRepReportViewModel>>
            GetSalesRepReportAsync(
                DateTime? startDate = null,
                DateTime? endDate = null)
        {
            var representatives =
                await _reportRepository.GetSalesRepresentativesAsync();

            var invoices = await _reportRepository
                .GetInvoicesAsync(startDate, endDate);

            return representatives.Select(rep =>
            {
                var totalSales = invoices
                    .Where(i =>
                        i.SalesRepresentativeId ==
                        rep.SalesRepresentativeId)
                    .Sum(i => i.Total);

                return new SalesRepReportViewModel
                {
                    SalesRepresentativeId =
                        rep.SalesRepresentativeId,

                    SalesRepCode = rep.SalesRepCode,

                    EmployeeName =
                        $"{rep.Employee.FirstName} " +
                        $"{rep.Employee.LastName}".Trim(),

                    Area = rep.Area ?? string.Empty,

                    SalesTarget = rep.SalesTarget,

                    TotalSales = totalSales,

                    CommissionRate = rep.CommissionRate,

                    EstimatedCommission =
                        totalSales * (rep.CommissionRate / 100m)
                };
            })
            .OrderByDescending(x => x.TotalSales);
        }

        public async Task<IEnumerable<SalesVatReportViewModel>>
            GetSalesVatReportAsync(
                DateTime? startDate = null,
                DateTime? endDate = null)
        {
            var invoices = await _reportRepository
                .GetInvoicesAsync(startDate, endDate);

            return invoices.Select(i =>
                new SalesVatReportViewModel
                {
                    InvoiceNumber = i.InvoiceNumber,
                    InvoiceDate = i.InvoiceDate,

                    CustomerName =
                        $"{i.Customer.Name} {i.Customer.Surname}".Trim(),

                    Subtotal = i.Subtotal,
                    Total = i.Total,

                    VatAmount =
                        Math.Max(0, i.Total - i.Subtotal)
                });
        }

        private static string GetAgeBracket(int ageInDays)
        {
            if (ageInDays <= 30)
                return "0-30 Days";

            if (ageInDays <= 60)
                return "31-60 Days";

            if (ageInDays <= 90)
                return "61-90 Days";

            return "90+ Days";
        }
    }
}