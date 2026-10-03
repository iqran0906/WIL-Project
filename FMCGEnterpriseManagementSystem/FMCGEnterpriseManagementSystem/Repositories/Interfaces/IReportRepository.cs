/***************************************************************************************
*    Title: Report Repository Interface
*    Author: iqran0906
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Repositories/Interfaces/IReportRepository.cs
***************************************************************************************/

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