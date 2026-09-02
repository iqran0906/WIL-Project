using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<IEnumerable<CustomerViewModel>> GetAllCustomersAsync()
        {
            var customers = await _customerRepository.GetAllAsync();
            return customers.Select(c => MapToViewModel(c));
        }

        public async Task<CustomerViewModel?> GetCustomerByIdAsync(string id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            return customer == null ? null : MapToViewModel(customer);
        }

        public async Task CreateCustomerAsync(CustomerViewModel model)
        {
            var customer = MapToEntity(model);

            // Auto-fallback: use billing/physical address if delivery address is left empty[cite: 1]
            if (string.IsNullOrWhiteSpace(customer.DeliveryAddress))
            {
                customer.DeliveryAddress = customer.PhysicalAddress;
            }

            await _customerRepository.AddAsync(customer);
        }

        public async Task UpdateCustomerAsync(CustomerViewModel model)
        {
            var customer = MapToEntity(model);
            await _customerRepository.UpdateAsync(customer);
        }

        public async Task DeleteCustomerAsync(string id)
        {
            await _customerRepository.DeleteAsync(id);
        }

        private static CustomerViewModel MapToViewModel(Customer c) => new()
        {
            CustomerID = c.CustomerID,
            Name = c.Name,
            Surname = c.Surname,
            IdNumber = c.IdNumber,
            TelephoneNumber = c.TelephoneNumber,
            CellNumber = c.CellNumber,
            Email = c.Email,
            PhysicalAddress = c.PhysicalAddress,
            DeliveryAddress = c.DeliveryAddress,
            CustomerGroup = c.CustomerGroup,
            PaymentTerms = c.PaymentTerms,
            PaymentMethod = c.PaymentMethod,
            Notes = c.Notes,
            SalesRep = c.SalesRep,
            VATNumber = c.VATNumber
        };

        private static Customer MapToEntity(CustomerViewModel vm) => new()
        {
            CustomerID = vm.CustomerID,
            Name = vm.Name,
            Surname = vm.Surname,
            IdNumber = vm.IdNumber,
            TelephoneNumber = vm.TelephoneNumber,
            CellNumber = vm.CellNumber,
            Email = vm.Email,
            PhysicalAddress = vm.PhysicalAddress,
            DeliveryAddress = vm.DeliveryAddress,
            CustomerGroup = vm.CustomerGroup,
            PaymentTerms = vm.PaymentTerms,
            PaymentMethod = vm.PaymentMethod,
            Notes = vm.Notes,
            SalesRep = vm.SalesRep,
            VATNumber = vm.VATNumber
        };
    }
}