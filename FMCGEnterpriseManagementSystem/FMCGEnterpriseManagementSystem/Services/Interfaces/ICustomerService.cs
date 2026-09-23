using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerViewModel>> GetAllCustomersAsync();
        Task<CustomerViewModel?> GetCustomerByIdAsync(int id);
        Task CreateCustomerAsync(CustomerViewModel model);
        Task UpdateCustomerAsync(CustomerViewModel model);
        Task DeleteCustomerAsync(int id);
    }
}