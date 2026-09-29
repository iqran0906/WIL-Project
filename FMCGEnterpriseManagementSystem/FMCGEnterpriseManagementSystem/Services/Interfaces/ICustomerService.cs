// Purpose: Contract (interface) for the customer service.
// Authors: Naseeha27, Maseeha17 (from git history)

using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerViewModel>> GetAllCustomersAsync(string? searchKeyword = null);
        Task<CustomerViewModel?> GetCustomerByIdAsync(int id);
        Task CreateCustomerAsync(CustomerViewModel model);
        Task UpdateCustomerAsync(CustomerViewModel model);
        Task DeleteCustomerAsync(int id);
    }
}