// Title: Saving Data - Entity Framework Core
// Author: Microsoft
// Date: 14-06-2025
// Code version: Entity Framework Core / .NET 10
// Availability: https://learn.microsoft.com/en-us/ef/core/saving/

using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.Repositories.Interfaces
{
    // Purpose: Defines the database operations required for sales representative management.
    public interface ISalesRepresentativeRepository
    {
        // Retrieves all sales representatives.
        Task<IEnumerable<SalesRepresentative>> GetAllAsync();

        // Retrieves a sales representative by their ID.
        Task<SalesRepresentative?> GetByIdAsync(
            int salesRepresentativeId);

        // Retrieves a sales representative using their employee ID.
        Task<SalesRepresentative?> GetByEmployeeIdAsync(
            string employeeId);

        // Retrieves a sales representative using their sales representative code.
        Task<SalesRepresentative?> GetByCodeAsync(
            string salesRepCode);

        // Retrieves employees who are eligible to become sales representatives.
        Task<IEnumerable<Employee>> GetEligibleEmployeesAsync();

        // Adds a new sales representative.
        Task AddAsync(
            SalesRepresentative salesRepresentative);

        // Marks a sales representative as modified.
        void Update(
            SalesRepresentative salesRepresentative);

        // Checks whether a sales representative code already exists.
        Task<bool> SalesRepCodeExistsAsync(
            string salesRepCode,
            int? excludeSalesRepresentativeId = null);

        // Saves pending database changes.
        Task SaveChangesAsync();
    }
}