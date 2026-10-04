

using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    // Restricts customer management access to authorised business users.
    [Authorize(Roles = "Administrator,Employee,SalesRepresentative")]
    public class CustomersController : Controller
    {
        // Service responsible for customer-related business operations.
        private readonly ICustomerService _customerService;

        // Database context used to retrieve sales representative information.
        private readonly ApplicationDbContext _context;

        // Dependency injection provides the required services to the controller.
        public CustomersController(ICustomerService customerService, ApplicationDbContext context)
        {
            _customerService = customerService;
            _context = context;
        }

        // Populates the Sales Representative dropdown with active representatives.
        private async Task PopulateSalesRepsDropdownAsync()
        {
            var salesReps = await _context.SalesRepresentatives
                .Include(sr => sr.Employee)
                .Where(sr => sr.IsActive)
                .Select(sr => new
                {
                    sr.SalesRepresentativeId,
                    DisplayName = sr.Employee.FirstName + " " +
                                  sr.Employee.LastName + " (" +
                                  sr.SalesRepCode + ")"
                })
                .ToListAsync();

            // Makes the sales representative list available to the View.
            ViewBag.SalesRepresentatives = new SelectList(
                salesReps,
                "SalesRepresentativeId",
                "DisplayName"
            );
        }

        // GET: /Customers/CustomerList
        // Retrieves customers and optionally filters them using a search keyword.
        public async Task<IActionResult> CustomerList(string? searchKeyword)
        {
            var customers = await _customerService.GetAllCustomersAsync(searchKeyword);
            return View(customers);
        }

        // GET: /Customers/AddCustomer
        // Displays the form for creating a new customer.
        [HttpGet]
        public async Task<IActionResult> AddCustomer()
        {
            await PopulateSalesRepsDropdownAsync();
            return View(new CustomerViewModel());
        }

        // POST: /Customers/AddCustomer
        // Processes the submitted customer creation form.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCustomer(CustomerViewModel model)
        {
            // Checks whether the submitted customer information is valid.
            if (!ModelState.IsValid)
            {
                // Reloads the dropdown if validation fails.
                await PopulateSalesRepsDropdownAsync();
                return View(model);
            }

            // Creates the customer through the customer service.
            await _customerService.CreateCustomerAsync(model);

            // Returns the user to the customer list after successful creation.
            return RedirectToAction(nameof(CustomerList));
        }

        // GET: /Customers/Edit/{id}
        // Retrieves an existing customer for editing.
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Ensures that a valid customer ID was supplied.
            if (id <= 0) return NotFound();

            var customer = await _customerService.GetCustomerByIdAsync(id);

            // Returns NotFound if the requested customer does not exist.
            if (customer == null) return NotFound();

            await PopulateSalesRepsDropdownAsync();
            return View(customer);
        }

        // POST: /Customers/Edit/{id}
        // Processes the submitted customer update form.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CustomerViewModel model)
        {
            // Confirms that the URL ID matches the submitted customer ID.
            if (id != model.CustomerId) return BadRequest();

            // Validates the submitted customer information.
            if (!ModelState.IsValid)
            {
                await PopulateSalesRepsDropdownAsync();
                return View(model);
            }

            // Updates the customer through the customer service.
            await _customerService.UpdateCustomerAsync(model);

            return RedirectToAction(nameof(CustomerList));
        }

        // GET: /Customers/Delete/{id}
        // Displays the customer before deletion is confirmed.
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            // Ensures that a valid customer ID was supplied.
            if (id <= 0) return NotFound();

            var customer = await _customerService.GetCustomerByIdAsync(id);

            // Returns NotFound if the requested customer does not exist.
            if (customer == null) return NotFound();

            return View(customer);
        }

        // POST: /Customers/Delete/{id}
        // Permanently removes the selected customer.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _customerService.DeleteCustomerAsync(id);

            // Returns the user to the customer list after deletion.
            return RedirectToAction(nameof(CustomerList));
        }
    }
}