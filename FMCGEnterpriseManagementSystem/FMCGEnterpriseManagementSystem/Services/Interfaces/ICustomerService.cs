/***************************************************************************************
*    Title: Customer Service Interface
*    Author: Naseeha27, Maseeha17
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Services/Interfaces/ICustomerService.cs
***************************************************************************************/

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