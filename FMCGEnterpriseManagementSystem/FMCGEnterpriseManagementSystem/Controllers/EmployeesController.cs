using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    // Only users with the Administrator role can manage employees.
    [Authorize(Roles = "Administrator")]

   
  //  Title: Role-based authorization in ASP.NET Core
  //  Author: iqra0906
  //  Date: 14-10-2024
  //  Code version: ASP.NET Core 10.0
  //  Availability: https://learn.microsoft.com/aspnet/core/security/authorization/roles

    public class EmployeesController : Controller
    {
        private readonly IEmployeeService _employeeService;

        // Dependency injection provides the employee service to the controller.
        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        // Displays all employees or filters the list using a search keyword.
        public async Task<IActionResult> Index(string? keyword)
        {
            IEnumerable<Employee> employees;

            if (string.IsNullOrWhiteSpace(keyword))
            {
                employees = await _employeeService.GetAllEmployeesAsync();
            }
            else
            {
                employees = await _employeeService.SearchEmployeesAsync(keyword);
            }

            ViewBag.Keyword = keyword;

            return View(employees);
        }

        // Displays the employee creation form with today's date as the default employment date.
        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new EmployeeViewModel
            {
                DateOfEmployment = DateTime.Today
            };

            return View(viewModel);
        }

        // Processes the submitted employee creation form.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeViewModel model)
        {
            // Return the form if the submitted data fails validation.
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Prevents duplicate employee email addresses.
            if (await _employeeService.EmailExistsAsync(model.Email))
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "An employee with this email address already exists.");

                return View(model);
            }

            // Maps the submitted view model data to an Employee entity.
            var employee = new Employee
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                ContactNumber = model.ContactNumber,
                JobTitle = model.JobTitle,
                DateOfEmployment = model.DateOfEmployment,

                // Creates the employee's Next of Kin details.
                NextOfKin = new NextOfKin
                {
                    FullName = model.NextOfKinFullName,
                    Relationship = model.NextOfKinRelationship,
                    ContactNumber = model.NextOfKinContactNumber,
                    Email = model.NextOfKinEmail
                }
            };

            var created = await _employeeService.CreateEmployeeAsync(employee);

            // Displays the form again if the employee could not be created.
            if (!created)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "The employee could not be created. Please check the employee details and try again.");

                return View(model);
            }

            TempData["SuccessMessage"] = "Employee created successfully.";

            return RedirectToAction(nameof(Index));
        }

        // Retrieves an employee and displays their details for editing.
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            // Validate that an employee ID was supplied.
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            var employee = await _employeeService.GetEmployeeByIdAsync(id);

            // Return NotFound if the employee does not exist.
            if (employee == null)
            {
                return NotFound();
            }

            // Maps the existing employee data into the edit view model.
            var viewModel = new EmployeeViewModel
            {
                EmployeeID = employee.EmployeeID,
                EmployeeNumber = employee.EmployeeNumber,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                ContactNumber = employee.ContactNumber,
                JobTitle = employee.JobTitle,
                DateOfEmployment = employee.DateOfEmployment,
                IsActive = employee.IsActive,

                // Safely loads Next of Kin details if they exist.
                NextOfKinID = employee.NextOfKin?.NextOfKinID,
                NextOfKinFullName = employee.NextOfKin?.FullName ?? string.Empty,
                NextOfKinRelationship = employee.NextOfKin?.Relationship ?? string.Empty,
                NextOfKinContactNumber = employee.NextOfKin?.ContactNumber ?? string.Empty,
                NextOfKinEmail = employee.NextOfKin?.Email
            };

            return View(viewModel);
        }

        // Processes the submitted employee edit form.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmployeeViewModel model)
        {
            // Return the form if the submitted data fails validation.
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Checks for duplicate email addresses while excluding the current employee.
            if (await _employeeService.EmailExistsAsync(
                    model.Email,
                    model.EmployeeID))
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "An employee with this email address already exists.");

                return View(model);
            }

            var employee = await _employeeService.GetEmployeeByIdAsync(model.EmployeeID!);

            // Return NotFound if the employee no longer exists.
            if (employee == null)
            {
                return NotFound();
            }

            // Updates the employee's details using the submitted model.
            employee.FirstName = model.FirstName;
            employee.LastName = model.LastName;
            employee.Email = model.Email;
            employee.ContactNumber = model.ContactNumber;
            employee.JobTitle = model.JobTitle;
            employee.DateOfEmployment = model.DateOfEmployment;

            // Creates a Next of Kin record if one does not already exist.
            if (employee.NextOfKin == null)
            {
                employee.NextOfKin = new NextOfKin();
            }

            // Updates the employee's Next of Kin details.
            employee.NextOfKin.FullName = model.NextOfKinFullName;
            employee.NextOfKin.Relationship = model.NextOfKinRelationship;
            employee.NextOfKin.ContactNumber = model.NextOfKinContactNumber;
            employee.NextOfKin.Email = model.NextOfKinEmail;

            var updated = await _employeeService.UpdateEmployeeAsync(employee);

            // Displays an error if the employee could not be updated.
            if (!updated)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "The employee could not be updated.");

                return View(model);
            }

            TempData["SuccessMessage"] = "Employee updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        // Deactivates an employee rather than permanently deleting the record.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(string id)
        {
            // Validate that an employee ID was supplied.
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            var deactivated = await _employeeService.DeactivateEmployeeAsync(id);

            // Return NotFound if the employee could not be found.
            if (!deactivated)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Employee deactivated successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}