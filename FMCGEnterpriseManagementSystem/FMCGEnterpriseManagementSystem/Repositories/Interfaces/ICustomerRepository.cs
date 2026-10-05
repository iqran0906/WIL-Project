// Title: Asynchronous programming with async and await
// Author: Microsoft
// Date: 2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.Repositories.Interfaces
{
    // Defines the database operations required for customer management.
    public interface ICustomerRepository
    {
        // Retrieves active customers.
        Task<IEnumerable<Customer>> GetAllAsync();

        // Retrieves inactive customers for the Recently Deleted page.
        Task<IEnumerable<Customer>> GetDeletedAsync();

        // Retrieves a customer by their ID.
        Task<Customer?> GetByIdAsync(int id);

        // Adds a new customer.
        Task AddAsync(Customer customer);

        // Updates an existing customer.
        Task UpdateAsync(Customer customer);

        // Soft deletes a customer.
        Task DeleteAsync(int id);

        // Restores a previously deleted customer.
        Task RestoreAsync(int id);
    }
}