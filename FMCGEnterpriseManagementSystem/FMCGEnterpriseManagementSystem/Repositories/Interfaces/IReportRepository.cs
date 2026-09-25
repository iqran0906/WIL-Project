using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.Repositories.Interfaces
{
    public interface IReportRepository
    {
        Task<IEnumerable<Invoice>> GetInvoicesAsync( 
            DateTime? startDate = null,
            DateTime? endDate = null);

        Task<IEnumerable<Payment>> GetPaymentsAsync(
            DateTime? startDate = null,
            DateTime? endDate = null);

        Task<IEnumerable<Customer>> GetCustomersAsync();

        Task<IEnumerable<Quote>> GetQuotesAsync(
            DateTime? startDate = null,
            DateTime? endDate = null);

        Task<IEnumerable<SalesRepresentative>> GetSalesRepresentativesAsync();

        Task<IEnumerable<Inventory>> GetInventoryAsync();
    }
}