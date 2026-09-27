using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    [Authorize(Roles = "Administrator")] 
    public class UserAccountsController : Controller
    {
        private readonly IUserAccountService _userAccountService;

        public UserAccountsController(IUserAccountService userAccountService)
        {
            _userAccountService = userAccountService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
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
            if (!ModelState.IsValid)
            {
                var employee = await _userAccountService
                    .GetEmployeeByIdAsync(model.EmployeeID);

                ViewBag.EmployeeName = employee == null
                    ? string.Empty
                    : $"{employee.FirstName} {employee.LastName}";

                return View(model);
            }

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