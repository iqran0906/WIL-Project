using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task<IActionResult> Index()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();

            return View(employees);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new EmployeeViewModel
            {
                DateOfEmployment = DateTime.Today
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var employee = new Employee
            {
                EmployeeNumber = model.EmployeeNumber,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                ContactNumber = model.ContactNumber,
                JobTitle = model.JobTitle,
                DateOfEmployment = model.DateOfEmployment,
                UserID = model.UserID,

                NextOfKin = new NextOfKin
                {
                    FullName = model.NextOfKinFullName,
                    Relationship = model.NextOfKinRelationship,
                    ContactNumber = model.NextOfKinContactNumber,
                    Email = model.NextOfKinEmail
                }
            };

            var created = await _employeeService.CreateEmployeeAsync(employee);

            if (!created)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "An employee with the same employee number or email already exists.");

                return View(model);
            }

            TempData["SuccessMessage"] = "Employee created successfully.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            var employee = await _employeeService.GetEmployeeByIdAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

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
                UserID = employee.UserID,
                IsActive = employee.IsActive,

                NextOfKinID = employee.NextOfKin?.NextOfKinID,
                NextOfKinFullName = employee.NextOfKin?.FullName ?? string.Empty,
                NextOfKinRelationship = employee.NextOfKin?.Relationship ?? string.Empty,
                NextOfKinContactNumber = employee.NextOfKin?.ContactNumber ?? string.Empty,
                NextOfKinEmail = employee.NextOfKin?.Email
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmployeeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var employee = await _employeeService.GetEmployeeByIdAsync(model.EmployeeID!);

            if (employee == null)
            {
                return NotFound();
            }

            employee.EmployeeNumber = model.EmployeeNumber;
            employee.FirstName = model.FirstName;
            employee.LastName = model.LastName;
            employee.Email = model.Email;
            employee.ContactNumber = model.ContactNumber;
            employee.JobTitle = model.JobTitle;
            employee.DateOfEmployment = model.DateOfEmployment;
            employee.UserID = model.UserID;

            if (employee.NextOfKin == null)
            {
                employee.NextOfKin = new NextOfKin();
            }

            employee.NextOfKin.FullName = model.NextOfKinFullName;
            employee.NextOfKin.Relationship = model.NextOfKinRelationship;
            employee.NextOfKin.ContactNumber = model.NextOfKinContactNumber;
            employee.NextOfKin.Email = model.NextOfKinEmail;

            var updated = await _employeeService.UpdateEmployeeAsync(employee);

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            var deactivated = await _employeeService.DeactivateEmployeeAsync(id);

            if (!deactivated)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Employee deactivated successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}