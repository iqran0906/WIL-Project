using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    public interface ISalesRepresentativeService
    {
        Task<IEnumerable<SalesRepresentativeViewModel>> GetAllAsync(
            string? keyword = null);

        Task<SalesRepresentativeViewModel?> GetByIdAsync(
            int salesRepresentativeId);

        Task<IEnumerable<Employee>> GetEligibleEmployeesAsync();

        Task<bool> CreateAsync(
            SalesRepresentativeViewModel model);

        Task<bool> UpdateAsync(
            SalesRepresentativeViewModel model);

        Task<bool> DeactivateAsync(
            int salesRepresentativeId);

        Task<bool> SalesRepCodeExistsAsync(
            string salesRepCode,
            int? excludeSalesRepresentativeId = null);
    }
}