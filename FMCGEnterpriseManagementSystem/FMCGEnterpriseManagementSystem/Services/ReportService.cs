// Title: Asynchronous programming with async and await
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels.Reports;

namespace FMCGEnterpriseManagementSystem.Services
{
    // Service responsible for generating report data from repository records.
    public class ReportService : IReportService
    {
        // Repository used to retrieve the data required by the reports.
        private readonly IReportRepository _reportRepository;

        // Initialises the report service with the report repository.
        public ReportService(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        // Generates an invoice report for the optional date range.
        public async Task<IEnumerable<InvoiceReportViewModel>> GetInvoiceReportAsync(
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            // Retrieves invoices within the selected date range.
            var invoices = await _reportRepository
                .GetInvoicesAsync(startDate, endDate);

            // Converts invoice entities into report view models.
            return invoices.Select(invoice =>
            {
                // Calculates the total amount already paid for the invoice.
                var amountPaid = invoice.Payments.Sum(p => p.AmountPaid);

                return new InvoiceReportViewModel
                {
                    InvoiceId = invoice.InvoiceId,
                    InvoiceNumber = invoice.InvoiceNumber,
                    InvoiceDate = invoice.InvoiceDate,

                    // Combines the customer's first and last names.
                    CustomerName =
                        $"{invoice.Customer.Name} {invoice.Customer.Surname}".Trim(),

                    Status = invoice.Status,
                    Subtotal = invoice.Subtotal,
                    Total = invoice.Total,
                    AmountPaid = amountPaid,

                    // Calculates the remaining balance and prevents negative values.
                    OutstandingBalance =
                        Math.Max(0, invoice.Total - amountPaid)
                };
            });
        }

        // Generates a quotation report for the optional date range.
        public async Task<IEnumerable<QuoteReportViewModel>> GetQuoteReportAsync(
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            // Retrieves quotations from the repository.
            var quotes = await _reportRepository
                .GetQuotesAsync(startDate, endDate);

            // Converts quotation entities into report view models.
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

        // Generates an age analysis report for outstanding invoices.
        public async Task<IEnumerable<AgeAnalysisReportViewModel>>
            GetAgeAnalysisReportAsync()
        {
            // Retrieves all invoices.
            var invoices = await _reportRepository.GetInvoicesAsync();

            // Gets today's date for calculating invoice age.
            var today = DateTime.Today;

            // Calculates outstanding balances and invoice ages.
            return invoices
                .Select(invoice =>
                {
                    var paid = invoice.Payments.Sum(p => p.AmountPaid);

                    // Calculates the amount still owed.
                    var outstanding =
                        Math.Max(0, invoice.Total - paid);

                    // Calculates how many days have passed since the invoice date.
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
                // Only includes invoices that still have an outstanding balance.
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

                    // Assigns the invoice to an age category.
                    AgeBracket = GetAgeBracket(x.Age)
                })
                // Displays the oldest outstanding invoices first.
                .OrderByDescending(x => x.AgeInDays);
        }

        // Generates a report containing customer information.
        public async Task<IEnumerable<CustomerReportViewModel>>
            GetCustomerReportAsync()
        {
            // Retrieves all customers.
            var customers =
                await _reportRepository.GetCustomersAsync();

            // Converts customer entities into report view models.
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

        // Generates a report summarising sales by customer.
        public async Task<IEnumerable<CustomerSalesReportViewModel>>
            GetCustomerSalesReportAsync(
                DateTime? startDate = null,
                DateTime? endDate = null)
        {
            // Retrieves invoices within the selected period.
            var invoices = await _reportRepository
                .GetInvoicesAsync(startDate, endDate);

            // Groups invoices according to customer.
            return invoices
                .GroupBy(i => new
                {
                    i.CustomerId,
                    i.Customer.Name,
                    i.Customer.Surname
                })
                .Select(group =>
                {
                    // Calculates the customer's total sales.
                    var sales = group.Sum(i => i.Total);

                    // Calculates the total amount paid by the customer.
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

                        // Calculates the remaining customer balance.
                        OutstandingBalance =
                            Math.Max(0, sales - paid)
                    };
                })
                // Displays customers with the highest sales first.
                .OrderByDescending(x => x.TotalSales);
        }

        // Generates a report containing payment information.
        public async Task<IEnumerable<PaymentReportViewModel>>
            GetPaymentReportAsync(
                DateTime? startDate = null,
                DateTime? endDate = null)
        {
            // Retrieves payments within the selected date range.
            var payments = await _reportRepository
                .GetPaymentsAsync(startDate, endDate);

            // Converts payment entities into report view models.
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

        // Generates a report containing current inventory information.
        public async Task<IEnumerable<InventoryReportViewModel>>
            GetInventoryReportAsync()
        {
            // Retrieves inventory records.
            var inventory =
                await _reportRepository.GetInventoryAsync();

            // Converts inventory records into report view models.
            return inventory.Select(i =>
            {
                // Finds the earliest expiry date among stock batches
                // that still have available quantity.
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

                    // Calculates the value of the stock currently on hand.
                    StockValue =
                        i.QuantityOnHand * i.Product.SellingPrice,

                    // Determines whether stock has reached its reorder level.
                    IsLowStock =
                        i.QuantityOnHand <= i.ReorderLevel,

                    EarliestExpiryDate = earliestExpiry
                };
            });
        }

        // Generates a daily sales report for the selected date range.
        public async Task<IEnumerable<SalesReportViewModel>>
            GetSalesReportAsync(
                DateTime? startDate = null,
                DateTime? endDate = null)
        {
            // Retrieves invoices for the selected period.
            var invoices = await _reportRepository
                .GetInvoicesAsync(startDate, endDate);

            // Groups invoices by their invoice date.
            return invoices
                .GroupBy(i => i.InvoiceDate.Date)
                .Select(group =>
                {
                    // Calculates total sales for the day.
                    var totalSales = group.Sum(i => i.Total);

                    // Calculates total payments received for the day.
                    var totalPaid = group.Sum(i =>
                        i.Payments.Sum(p => p.AmountPaid));

                    return new SalesReportViewModel
                    {
                        SaleDate = group.Key,
                        InvoiceCount = group.Count(),
                        TotalSales = totalSales,
                        TotalPaid = totalPaid,

                        // Calculates the outstanding balance for the day.
                        OutstandingBalance =
                            Math.Max(0, totalSales - totalPaid)
                    };
                })
                // Displays the most recent sales dates first.
                .OrderByDescending(x => x.SaleDate);
        }

        // Generates a report showing the number of items purchased by each customer.
        public async Task<IEnumerable<ItemsPerCustomerReportViewModel>>
            GetItemsPerCustomerReportAsync(
                DateTime? startDate = null,
                DateTime? endDate = null)
        {
            // Retrieves invoices for the selected period.
            var invoices = await _reportRepository
                .GetInvoicesAsync(startDate, endDate);

            // Groups invoices by customer and calculates item quantities.
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

                    // Calculates the total number of items purchased.
                    TotalItemsPurchased =
                        group.SelectMany(i => i.InvoiceItems)
                             .Sum(item => item.Quantity),

                    TotalSales = group.Sum(i => i.Total)
                })
                // Displays customers with the highest item quantity first.
                .OrderByDescending(x => x.TotalItemsPurchased);
        }

        // Generates a report showing sales performance for individual products.
        public async Task<IEnumerable<ItemSalesReportViewModel>>
            GetItemSalesReportAsync(
                DateTime? startDate = null,
                DateTime? endDate = null)
        {
            // Retrieves invoices for the selected period.
            var invoices = await _reportRepository
                .GetInvoicesAsync(startDate, endDate);

            // Groups invoice items by product.
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

                    // Calculates the total quantity sold for the product.
                    QuantitySold =
                        group.Sum(item => item.Quantity),

                    // Calculates the total sales generated by the product.
                    TotalSales =
                        group.Sum(item => item.LineTotal)
                })
                // Displays products with the highest sales first.
                .OrderByDescending(x => x.TotalSales);
        }

        // Generates a report showing sales performance by sales representative.
        public async Task<IEnumerable<SalesRepReportViewModel>>
            GetSalesRepReportAsync(
                DateTime? startDate = null,
                DateTime? endDate = null)
        {
            // Retrieves all sales representatives.
            var representatives =
                await _reportRepository.GetSalesRepresentativesAsync();

            // Retrieves invoices for the selected reporting period.
            var invoices = await _reportRepository
                .GetInvoicesAsync(startDate, endDate);

            // Calculates sales information for each representative.
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

                    // Estimates commission based on total sales and commission rate.
                    EstimatedCommission =
                        totalSales * (rep.CommissionRate / 100m)
                };
            })
            // Displays representatives with the highest sales first.
            .OrderByDescending(x => x.TotalSales);
        }

        // Generates a report showing VAT amounts for sales invoices.
        public async Task<IEnumerable<SalesVatReportViewModel>>
            GetSalesVatReportAsync(
                DateTime? startDate = null,
                DateTime? endDate = null)
        {
            // Retrieves invoices for the selected reporting period.
            var invoices = await _reportRepository
                .GetInvoicesAsync(startDate, endDate);

            // Converts invoice data into VAT report view models.
            return invoices.Select(i =>
                new SalesVatReportViewModel
                {
                    InvoiceNumber = i.InvoiceNumber,
                    InvoiceDate = i.InvoiceDate,

                    CustomerName =
                        $"{i.Customer.Name} {i.Customer.Surname}".Trim(),

                    Subtotal = i.Subtotal,
                    Total = i.Total,

                    // Calculates the VAT amount as the difference between
                    // the invoice total and subtotal.
                    VatAmount =
                        Math.Max(0, i.Total - i.Subtotal)
                });
        }

        // Determines the appropriate age bracket for an outstanding invoice.
        private static string GetAgeBracket(int ageInDays)
        {
            // Invoices up to 30 days old.
            if (ageInDays <= 30)
                return "0-30 Days";

            // Invoices between 31 and 60 days old.
            if (ageInDays <= 60)
                return "31-60 Days";

            // Invoices between 61 and 90 days old.
            if (ageInDays <= 90)
                return "61-90 Days";

            // Invoices older than 90 days.
            return "90+ Days";
        }
    }
}