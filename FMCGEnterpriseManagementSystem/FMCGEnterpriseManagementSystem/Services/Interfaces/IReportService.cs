using FMCGEnterpriseManagementSystem.ViewModels.Reports;

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    public interface IReportService
    {
        Task<IEnumerable<InvoiceReportViewModel>> GetInvoiceReportAsync(
            DateTime? startDate = null, DateTime? endDate = null);

        Task<IEnumerable<QuoteReportViewModel>> GetQuoteReportAsync(
            DateTime? startDate = null, DateTime? endDate = null);

        Task<IEnumerable<AgeAnalysisReportViewModel>> GetAgeAnalysisReportAsync();

        Task<IEnumerable<CustomerReportViewModel>> GetCustomerReportAsync();

        Task<IEnumerable<CustomerSalesReportViewModel>> GetCustomerSalesReportAsync(
            DateTime? startDate = null, DateTime? endDate = null);

        Task<IEnumerable<PaymentReportViewModel>> GetPaymentReportAsync(
            DateTime? startDate = null, DateTime? endDate = null);

        Task<IEnumerable<InventoryReportViewModel>> GetInventoryReportAsync();

        Task<IEnumerable<SalesReportViewModel>> GetSalesReportAsync(
            DateTime? startDate = null, DateTime? endDate = null);

        Task<IEnumerable<ItemsPerCustomerReportViewModel>> GetItemsPerCustomerReportAsync(
            DateTime? startDate = null, DateTime? endDate = null);

        Task<IEnumerable<ItemSalesReportViewModel>> GetItemSalesReportAsync(
            DateTime? startDate = null, DateTime? endDate = null);

        Task<IEnumerable<SalesRepReportViewModel>> GetSalesRepReportAsync(
            DateTime? startDate = null, DateTime? endDate = null);

        Task<IEnumerable<SalesVatReportViewModel>> GetSalesVatReportAsync(
            DateTime? startDate = null, DateTime? endDate = null);
    }
}