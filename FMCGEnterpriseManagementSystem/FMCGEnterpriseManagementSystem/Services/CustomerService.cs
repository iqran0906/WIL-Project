
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly INotificationService _notificationService;

        public CustomerService(ICustomerRepository customerRepository, INotificationService notificationService)
        {
            _customerRepository = customerRepository;
            _notificationService = notificationService;

        }

        public async Task<IEnumerable<CustomerViewModel>> GetAllCustomersAsync(string? searchKeyword = null)
        {
            var customers = await _customerRepository.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(searchKeyword))
            {
                var query = searchKeyword.Trim().ToLower();
                customers = customers.Where(c =>
                    (c.Name != null && c.Name.ToLower().Contains(query)) ||
                    (c.Surname != null && c.Surname.ToLower().Contains(query)) ||
                    (c.Email != null && c.Email.ToLower().Contains(query)) ||
                    (c.CellNumber != null && c.CellNumber.Contains(query))
                );
            }

            return customers.Select(c => MapToViewModel(c));
        }

        public async Task<CustomerViewModel?> GetCustomerByIdAsync(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            return customer == null ? null : MapToViewModel(customer);
        }

        public async Task CreateCustomerAsync(CustomerViewModel model)
        {
            var customer = MapToEntity(model);

            if (string.IsNullOrWhiteSpace(customer.DeliveryAddress))
            {
                customer.DeliveryAddress = customer.PhysicalAddress;
            }

            await _customerRepository.AddAsync(customer);
            await _notificationService.NotifyNewCustomerAsync($"{customer.Name} {customer.Surname}", customer.CustomerId);
        }

        public async Task UpdateCustomerAsync(CustomerViewModel model)
        {
            var customer = MapToEntity(model);

            if (string.IsNullOrWhiteSpace(customer.DeliveryAddress))
            {
                customer.DeliveryAddress = customer.PhysicalAddress;
            }

            await _customerRepository.UpdateAsync(customer);
        }

        public async Task DeleteCustomerAsync(int id)
        {
            await _customerRepository.DeleteAsync(id);
        }

        private static CustomerViewModel MapToViewModel(Customer c) => new()
        {
            CustomerId = c.CustomerId,
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
            SalesRepresentativeId = c.SalesRepresentativeId,

            SalesRep = c.SalesRepresentative?.Employee != null
    ? $"{c.SalesRepresentative.Employee.FirstName} {c.SalesRepresentative.Employee.LastName} ({c.SalesRepresentative.SalesRepCode})"
    : "Unassigned",
            VATNumber = c.VATNumber
        };

        private static Customer MapToEntity(CustomerViewModel vm) => new()
        {
            CustomerId = vm.CustomerId,
            Name = vm.Name,
            Surname = vm.Surname,
            IdNumber = vm.IdNumber,
           
            TelephoneNumber = vm.TelephoneNumber ?? string.Empty,
            CellNumber = vm.CellNumber,
            Email = vm.Email,
            PhysicalAddress = vm.PhysicalAddress,
            DeliveryAddress = string.IsNullOrWhiteSpace(vm.DeliveryAddress) ? vm.PhysicalAddress : vm.DeliveryAddress,
            CustomerGroup = vm.CustomerGroup,
            PaymentTerms = vm.PaymentTerms,
            PaymentMethod = vm.PaymentMethod,
            Notes = vm.Notes ?? string.Empty,
            SalesRepresentativeId = vm.SalesRepresentativeId,
            VATNumber = vm.VATNumber ?? string.Empty
        };
    }
}