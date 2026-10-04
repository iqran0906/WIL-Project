//  Title: Role-based authorization in ASP.NET Core
//  Author: iqra0906
//  Date: 14-10-2024
//  Code version: ASP.NET Core 10.0
//  Availability: https://learn.microsoft.com/aspnet/core/security/authorization/roles

using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    // Restricts user account management to administrators.
    [Authorize(Roles = "Administrator")]
    // Title: ASP.NET Core Identity
    // Author: iqra0906 
    // Date: 10-11-2025
    // Code version: ASP.NET Core 10.0
    // Availability: https://learn.microsoft.com/aspnet/core/security/authentication/identity
    public class UserAccountsController : Controller
    {
        private readonly IUserAccountService _userAccountService;

        // Injects the user account service through dependency injection.
        public UserAccountsController(IUserAccountService userAccountService)
        {
            _userAccountService = userAccountService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Retrieves employees with and without linked user accounts.
            var employeesWithAccounts =
                await _userAccountService.GetEmployeesWithAccountsAsync();

            var employeesWithoutAccounts =
                await _userAccountService.GetEmployeesWithoutAccountsAsync();

            ViewBag.EmployeesWithoutAccounts = employeesWithoutAccounts;

            return View(employeesWithAccounts);
        }

        [HttpGet]
        public async Task<IActionResult> Create(string employeeId)
        {
            // A valid employee ID is required to create an account.
            if (string.IsNullOrWhiteSpace(employeeId))
            {
                return BadRequest();
            }

            var employee = await _userAccountService
                .GetEmployeeByIdAsync(employeeId);

            if (employee == null)
            {
                return NotFound();
            }

            // Prevents multiple user accounts from being created for one employee.
            if (!string.IsNullOrWhiteSpace(employee.UserId))
            {
                TempData["ErrorMessage"] =
                    "This employee already has a user account.";

                return RedirectToAction(nameof(Index));
            }

            var model = new CreateUserAccountViewModel
            {
                EmployeeID = employee.EmployeeID,
                Email = employee.Email
            };

            ViewBag.EmployeeName =
                $"{employee.FirstName} {employee.LastName}";

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            CreateUserAccountViewModel model)
        {
            // Stops invalid account information from being submitted.
            if (!ModelState.IsValid)
            {
                var employee = await _userAccountService
                    .GetEmployeeByIdAsync(model.EmployeeID);

                ViewBag.EmployeeName = employee == null
                    ? string.Empty
                    : $"{employee.FirstName} {employee.LastName}";

                return View(model);
            }

            // Creates the account using the selected employee, credentials and role.
            var created = await _userAccountService.CreateAccountAsync(
                model.EmployeeID,
                model.Email,
                model.TemporaryPassword,
                model.Role);

            if (!created)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "The user account could not be created. Check that the employee does not already have an account, the email is unique, the password meets security requirements, and a valid role was selected.");

                var employee = await _userAccountService
                    .GetEmployeeByIdAsync(model.EmployeeID);

                ViewBag.EmployeeName = employee == null
                    ? string.Empty
                    : $"{employee.FirstName} {employee.LastName}";

                return View(model);
            }

            TempData["SuccessMessage"] =
                "User account created successfully.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(string employeeId)
        {
            // A valid employee ID is required before activating the account.
            if (string.IsNullOrWhiteSpace(employeeId))
            {
                return BadRequest();
            }

            var activated = await _userAccountService
                .ActivateAccountAsync(employeeId);

            TempData[activated ? "SuccessMessage" : "ErrorMessage"] =
                activated
                    ? "User account activated successfully."
                    : "The user account could not be activated.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(string employeeId)
        {
            // A valid employee ID is required before deactivating the account.
            if (string.IsNullOrWhiteSpace(employeeId))
            {
                return BadRequest();
            }

            var deactivated = await _userAccountService
                .DeactivateAccountAsync(employeeId);

            TempData[deactivated ? "SuccessMessage" : "ErrorMessage"] =
                deactivated
                    ? "User account deactivated successfully."
                    : "The user account could not be deactivated.";

            return RedirectToAction(nameof(Index));
        }
    }
}