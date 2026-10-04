// Title: Asynchronous programming with async and await
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using FMCGEnterpriseManagementSystem.ViewModels.Reports;

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    // Defines the operations used to generate different reports
    // from the system's business and transactional data.
    public interface IReportService
    {
        // Generates a report containing invoice information
        // within an optional date range.
        Task<IEnumerable<InvoiceReportViewModel>> GetInvoiceReportAsync(
            DateTime? startDate = null, DateTime? endDate = null);

        // Generates a report containing quote information
        // within an optional date range.
        Task<IEnumerable<QuoteReportViewModel>> GetQuoteReportAsync(
            DateTime? startDate = null, DateTime? endDate = null);

        // Generates an age analysis report showing outstanding
        // customer balances according to their age.
        Task<IEnumerable<AgeAnalysisReportViewModel>> GetAgeAnalysisReportAsync();

        // Generates a report containing customer information.
        Task<IEnumerable<CustomerReportViewModel>> GetCustomerReportAsync();

        // Generates a report showing customer sales within
        // an optional date range.
        Task<IEnumerable<CustomerSalesReportViewModel>> GetCustomerSalesReportAsync(
            DateTime? startDate = null, DateTime? endDate = null);

        // Generates a payment report within an optional date range.
        Task<IEnumerable<PaymentReportViewModel>> GetPaymentReportAsync(
            DateTime? startDate = null, DateTime? endDate = null);

        // Generates a report containing current inventory information.
        Task<IEnumerable<InventoryReportViewModel>> GetInventoryReportAsync();

        // Generates a sales report within an optional date range.
        Task<IEnumerable<SalesReportViewModel>> GetSalesReportAsync(
            DateTime? startDate = null, DateTime? endDate = null);

        // Generates a report showing items purchased by customers
        // within an optional date range.
        Task<IEnumerable<ItemsPerCustomerReportViewModel>> GetItemsPerCustomerReportAsync(
            DateTime? startDate = null, DateTime? endDate = null);

        // Generates a report containing sales information for individual items
        // within an optional date range.
        Task<IEnumerable<ItemSalesReportViewModel>> GetItemSalesReportAsync(
            DateTime? startDate = null, DateTime? endDate = null);

        // Generates a report containing sales representative performance
        // within an optional date range.
        Task<IEnumerable<SalesRepReportViewModel>> GetSalesRepReportAsync(
            DateTime? startDate = null, DateTime? endDate = null);

        // Generates a report containing sales and VAT information
        // within an optional date range.
        Task<IEnumerable<SalesVatReportViewModel>> GetSalesVatReportAsync(
            DateTime? startDate = null, DateTime? endDate = null);
    }
}