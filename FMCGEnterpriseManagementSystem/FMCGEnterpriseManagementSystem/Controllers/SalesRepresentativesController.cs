using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class SalesRepresentativesController : Controller
    {
        private readonly ISalesRepresentativeService
            _salesRepresentativeService;

        public SalesRepresentativesController(
            ISalesRepresentativeService salesRepresentativeService)
        {
            _salesRepresentativeService =
                salesRepresentativeService;
        }

        // Displays all sales representatives and supports searching
        [HttpGet]
        public async Task<IActionResult> Index(string? keyword)
        {
            var salesRepresentatives =
                await _salesRepresentativeService
                    .GetAllAsync(keyword);

            ViewBag.Keyword = keyword;

            return View(salesRepresentatives);
        }

        // Displays the Create Sales Representative form
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadEligibleEmployeesAsync();

            return View(new SalesRepresentativeViewModel());
        }

        // Creates the Sales Representative
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            SalesRepresentativeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadEligibleEmployeesAsync(
                    model.EmployeeID);

                return View(model);
            }

            var codeExists =
                await _salesRepresentativeService
                    .SalesRepCodeExistsAsync(
                        model.SalesRepCode);

            if (codeExists)
            {
                ModelState.AddModelError(
                    nameof(model.SalesRepCode),
                    "This sales representative code is already in use.");

                await LoadEligibleEmployeesAsync(
                    model.EmployeeID);

                return View(model);
            }

            var created =
                await _salesRepresentativeService
                    .CreateAsync(model);

            if (!created)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "The sales representative could not be created. " +
                    "The selected employee may already be a sales representative.");

                await LoadEligibleEmployeesAsync(
                    model.EmployeeID);

                return View(model);
            }

            TempData["SuccessMessage"] =
                "Sales representative created successfully.";

            return RedirectToAction(nameof(Index));
        }

        // Displays the Edit Sales Representative form
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var salesRepresentative =
                await _salesRepresentativeService
                    .GetByIdAsync(id);

            if (salesRepresentative == null)
            {
                return NotFound();
            }

            return View(salesRepresentative);
        }

        // Updates the Sales Representative
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            SalesRepresentativeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var codeExists =
                await _salesRepresentativeService
                    .SalesRepCodeExistsAsync(
                        model.SalesRepCode,
                        model.SalesRepresentativeId);

            if (codeExists)
            {
                ModelState.AddModelError(
                    nameof(model.SalesRepCode),
                    "This sales representative code is already in use.");

                return View(model);
            }

            var updated =
                await _salesRepresentativeService
                    .UpdateAsync(model);

            if (!updated)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] =
                "Sales representative updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        // Deactivates a Sales Representative without deleting history
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            var deactivated =
                await _salesRepresentativeService
                    .DeactivateAsync(id);

            if (!deactivated)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] =
                "Sales representative deactivated successfully.";

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadEligibleEmployeesAsync(
     string? selectedEmployeeId = null)
        {
            var employees =
                await _salesRepresentativeService
                    .GetEligibleEmployeesAsync();

            var employeeOptions =
                employees.Select(employee => new
                {
                    employee.EmployeeID,

                    DisplayText =
                        $"{employee.EmployeeNumber} - " +
                        $"{employee.FirstName} {employee.LastName}"
                })
                .ToList();

            ViewBag.Employees =
                new SelectList(
                    employeeOptions,
                    "EmployeeID",
                    "DisplayText",
                    selectedEmployeeId);
        }
    }
}