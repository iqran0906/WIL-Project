// Title: Asynchronous programming with async and await
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    // Defines the operations used to manage customer records.
    public interface ICustomerService
    {
        // Retrieves all customers, with an optional keyword used to filter
        // the returned customer records.
        Task<IEnumerable<CustomerViewModel>> GetAllCustomersAsync(string? searchKeyword = null);

        // Retrieves a specific customer using their ID.
        // Returns null when the customer cannot be found.
        Task<CustomerViewModel?> GetCustomerByIdAsync(int id);

        // Creates and stores a new customer using the supplied view model.
        Task CreateCustomerAsync(CustomerViewModel model);

        // Updates an existing customer using the supplied view model.
        Task UpdateCustomerAsync(CustomerViewModel model);

        // Deletes a customer using their ID.
        Task DeleteCustomerAsync(int id);
    }
}