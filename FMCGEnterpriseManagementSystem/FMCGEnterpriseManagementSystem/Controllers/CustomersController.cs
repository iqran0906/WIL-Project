using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    [Authorize(Roles = "Administrator,Employee,SalesRepresentative")]
    public class CustomersController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly ApplicationDbContext _context;

        public CustomersController(ICustomerService customerService, ApplicationDbContext context)
        {
            _customerService = customerService;
            _context = context;
        }

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

            ViewBag.SalesRepresentatives = new SelectList(
                salesReps,
                "SalesRepresentativeId",
                "DisplayName"
            );
        }

        // GET: /Customers/CustomerList
        public async Task<IActionResult> CustomerList(string? searchKeyword)
        {
            var customers = await _customerService.GetAllCustomersAsync(searchKeyword);
            return View(customers);
        }

        // GET: /Customers/AddCustomer
        [HttpGet]
        public async Task<IActionResult> AddCustomer()
        {
            await PopulateSalesRepsDropdownAsync();
            return View(new CustomerViewModel());
        }

        // POST: /Customers/AddCustomer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCustomer(CustomerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateSalesRepsDropdownAsync();
                return View(model);
            }

            await _customerService.CreateCustomerAsync(model);
            return RedirectToAction(nameof(CustomerList));
        }

        // GET: /Customers/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0) return NotFound();

            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null) return NotFound();

            await PopulateSalesRepsDropdownAsync();
            return View(customer);
        }

        // POST: /Customers/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CustomerViewModel model)
        {
            if (id != model.CustomerId) return BadRequest();

            if (!ModelState.IsValid)
            {
                await PopulateSalesRepsDropdownAsync();
                return View(model);
            }

            await _customerService.UpdateCustomerAsync(model);
            return RedirectToAction(nameof(CustomerList));
        }

        // GET: /Customers/Delete/{id}
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return NotFound();

            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null) return NotFound();

            return View(customer);
        }

        // POST: /Customers/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _customerService.DeleteCustomerAsync(id);
            return RedirectToAction(nameof(CustomerList));
        }
    }
}