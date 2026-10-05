// Title: Asynchronous programming with async and await
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    // Defines the operations used to manage products.
    public interface IProductService
    {
        // Retrieves all products in the system.
        Task<IEnumerable<ProductViewModel>> GetAllProductsAsync();

        // Retrieves a specific product using its ID.
        // Returns null when the product cannot be found.
        Task<ProductViewModel?> GetProductByIdAsync(int id);

        // Creates a new product using the supplied view model.
        Task CreateProductAsync(ProductViewModel model);

        // Updates an existing product using the supplied view model.
        Task UpdateProductAsync(ProductViewModel model);

        // Deletes a product using its ID.
        Task DeleteProductAsync(int id);

        // Activates a product using its ID.
        Task ActivateProductAsync(int id);
    }
}