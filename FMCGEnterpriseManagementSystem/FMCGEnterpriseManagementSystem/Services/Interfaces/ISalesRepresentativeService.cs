/***************************************************************************************
*    Title: Sales Representative Service Interface
*    Author: iqran0906
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Services/Interfaces/ISalesRepresentativeService.cs
***************************************************************************************/

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