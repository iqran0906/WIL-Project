// Title: Asynchronous programming with async and await
// Author: Maseeha17
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Repositories.Interfaces;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services
{
    // Service responsible for managing customer-related business operations.
    public class CustomerService : ICustomerService
    {
        // Repository used to access and manage customer records.
        private readonly ICustomerRepository _customerRepository;

        // Service used to create notifications for customer-related activities.
        private readonly INotificationService _notificationService;

        // Initialises the customer service with the required dependencies.
        public CustomerService(ICustomerRepository customerRepository, INotificationService notificationService)
        {
            _customerRepository = customerRepository;
            _notificationService = notificationService;
        }

        // Retrieves all customers and optionally filters them using a search keyword.
        public async Task<IEnumerable<CustomerViewModel>> GetAllCustomersAsync(string? searchKeyword = null)
        {
            // Retrieves all customer records from the repository.
            var customers = await _customerRepository.GetAllAsync();

            // Applies a search filter when a keyword has been provided.
            if (!string.IsNullOrWhiteSpace(searchKeyword))
            {
                // Removes unnecessary spaces and converts the search value to lowercase.
                var query = searchKeyword.Trim().ToLower();

                // Searches customers by name, surname, email address, or cell number.
                customers = customers.Where(c =>
                    (c.Name != null && c.Name.ToLower().Contains(query)) ||
                    (c.Surname != null && c.Surname.ToLower().Contains(query)) ||
                    (c.Email != null && c.Email.ToLower().Contains(query)) ||
                    (c.CellNumber != null && c.CellNumber.Contains(query))
                );
            }

            // Converts the customer entities into view models for the application interface.
            return customers.Select(c => MapToViewModel(c));
        }

        // Retrieves a customer using their unique customer ID.
        public async Task<CustomerViewModel?> GetCustomerByIdAsync(int id)
        {
            // Retrieves the customer from the repository.
            var customer = await _customerRepository.GetByIdAsync(id);

            // Returns null if the customer does not exist; otherwise maps the entity to a view model.
            return customer == null ? null : MapToViewModel(customer);
        }

        // Creates a new customer from the supplied view model.
        public async Task CreateCustomerAsync(CustomerViewModel model)
        {
            // Converts the view model into a customer entity.
            var customer = MapToEntity(model);

            // Uses the physical address as the delivery address when no delivery address is provided.
            if (string.IsNullOrWhiteSpace(customer.DeliveryAddress))
            {
                customer.DeliveryAddress = customer.PhysicalAddress;
            }

            // Adds the new customer to the database through the repository.
            await _customerRepository.AddAsync(customer);

            // Creates a notification when a new customer is added.
            await _notificationService.NotifyNewCustomerAsync($"{customer.Name} {customer.Surname}", customer.CustomerId);
        }

        // Updates an existing customer using the supplied view model.
        public async Task UpdateCustomerAsync(CustomerViewModel model)
        {
            // Converts the view model into a customer entity.
            var customer = MapToEntity(model);

            // Uses the physical address as the delivery address when no delivery address is provided.
            if (string.IsNullOrWhiteSpace(customer.DeliveryAddress))
            {
                customer.DeliveryAddress = customer.PhysicalAddress;
            }

            // Updates the customer through the repository.
            await _customerRepository.UpdateAsync(customer);
        }

        // Deletes a customer using their unique customer ID.
        public async Task DeleteCustomerAsync(int id)
        {
            // Removes the customer through the repository.
            await _customerRepository.DeleteAsync(id);
        }

        // Converts a Customer entity into a CustomerViewModel for use by the application interface.
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

            // Displays the assigned sales representative's name and code.
            // Shows "Unassigned" when no sales representative is linked to the customer.
            SalesRep = c.SalesRepresentative?.Employee != null
    ? $"{c.SalesRepresentative.Employee.FirstName} {c.SalesRepresentative.Employee.LastName} ({c.SalesRepresentative.SalesRepCode})"
    : "Unassigned",
            VATNumber = c.VATNumber
        };

        // Converts a CustomerViewModel into a Customer entity for database operations.
        private static Customer MapToEntity(CustomerViewModel vm) => new()
        {
            CustomerId = vm.CustomerId,
            Name = vm.Name,
            Surname = vm.Surname,
            IdNumber = vm.IdNumber,

            // Uses an empty string when no telephone number is supplied.
            TelephoneNumber = vm.TelephoneNumber ?? string.Empty,
            CellNumber = vm.CellNumber,
            Email = vm.Email,
            PhysicalAddress = vm.PhysicalAddress,

            // Uses the physical address when a separate delivery address is not provided.
            DeliveryAddress = string.IsNullOrWhiteSpace(vm.DeliveryAddress) ? vm.PhysicalAddress : vm.DeliveryAddress,
            CustomerGroup = vm.CustomerGroup,
            PaymentTerms = vm.PaymentTerms,
            PaymentMethod = vm.PaymentMethod,

            // Uses an empty string when notes are not provided.
            Notes = vm.Notes ?? string.Empty,
            SalesRepresentativeId = vm.SalesRepresentativeId,

            // Uses an empty string when a VAT number is not provided.
            VATNumber = vm.VATNumber ?? string.Empty
        };
    }
}