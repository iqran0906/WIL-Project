// Title: Implement CRUD - ASP.NET Core MVC with Entity Framework Core
// Author: Microsoft
// Date: 10-04-2024
// Code version: ASP.NET Core 10.0
// Availability: https://learn.microsoft.com/en-us/aspnet/core/data/ef-mvc/crud

using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.Repositories.Interfaces
{
    // Purpose: Defines the database operations required for supplier management.
    public interface ISupplierRepository
    {
        // Retrieves all suppliers.
        Task<IEnumerable<Supplier>> GetAllAsync();

        // Retrieves a supplier by its ID.
        Task<Supplier?> GetByIdAsync(int id);

        // Adds a new supplier.
        Task AddAsync(Supplier supplier);

        // Updates an existing supplier.
        Task UpdateAsync(Supplier supplier);

        // Deletes a supplier using its ID.
        Task DeleteAsync(int id);
    }
}
