// Title: Asynchronous programming with async and await
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    // Defines the operations used to manage sales representatives.
    public interface ISalesRepresentativeService
    {
        // Retrieves all sales representatives.
        // An optional keyword can be used to filter the results.
        Task<IEnumerable<SalesRepresentativeViewModel>> GetAllAsync(
            string? keyword = null);

        // Retrieves a specific sales representative using their ID.
        // Returns null when the sales representative cannot be found.
        Task<SalesRepresentativeViewModel?> GetByIdAsync(
            int salesRepresentativeId);

        // Retrieves employees who are eligible to become sales representatives.
        Task<IEnumerable<Employee>> GetEligibleEmployeesAsync();

        // Creates a new sales representative and returns whether
        // the operation was successful.
        Task<bool> CreateAsync(
            SalesRepresentativeViewModel model);

        // Updates an existing sales representative and returns whether
        // the operation was successful.
        Task<bool> UpdateAsync(
            SalesRepresentativeViewModel model);

        // Deactivates a sales representative using their ID.
        // Returns whether the operation was successful.
        Task<bool> DeactivateAsync(
            int salesRepresentativeId);

        // Reactivates an inactive sales representative.
        // Returns whether the operation was successful.
        Task<bool> ReactivateAsync(
            int salesRepresentativeId);

        // Checks whether a sales representative code already exists.
        // An optional ID can be excluded when checking during an update.
        Task<bool> SalesRepCodeExistsAsync(
            string salesRepCode,
            int? excludeSalesRepresentativeId = null);


    }
}