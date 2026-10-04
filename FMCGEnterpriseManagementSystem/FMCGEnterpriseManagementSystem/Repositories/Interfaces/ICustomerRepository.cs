// Title: Asynchronous programming with async and await
// Author: Microsoft
// Date: 2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using FMCGEnterpriseManagementSystem.Models;

namespace FMCGEnterpriseManagementSystem.Repositories.Interfaces
{
    // Purpose: Defines the database operations required for customer management.
    public interface ICustomerRepository
    {
        // Retrieves all customers.
        Task<IEnumerable<Customer>> GetAllAsync();

        // Retrieves a customer by their ID.
        Task<Customer?> GetByIdAsync(int id);

        // Adds a new customer to the database.
        Task AddAsync(Customer customer);

        // Updates an existing customer.
        Task UpdateAsync(Customer customer);

        // Deletes a customer using their ID.
        Task DeleteAsync(int id);
    }
}