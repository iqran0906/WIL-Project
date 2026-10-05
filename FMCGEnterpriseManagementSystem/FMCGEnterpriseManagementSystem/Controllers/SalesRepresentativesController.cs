//  Title: Role-based authorization in ASP.NET Core
//  Author: Microsoft
//  Date: 14-10-2024
//  Code version: ASP.NET Core 10.0
//  Availability: https://learn.microsoft.com/aspnet/core/security/authorization/roles

using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    // Only administrators can manage sales representatives.
    [Authorize(Roles = "Administrator")]
    public class SalesRepresentativesController : Controller
    {
        // Service handles sales representative business operations.
        private readonly ISalesRepresentativeService
            _salesRepresentativeService;

        // Dependency injection provides the sales representative service.
        public SalesRepresentativesController(
            ISalesRepresentativeService salesRepresentativeService)
        {
            _salesRepresentativeService =
                salesRepresentativeService;
        }

        // Displays active sales representatives and supports searching.
        [HttpGet]
        public async Task<IActionResult> Index(string? keyword)
        {
            var salesRepresentatives =
                await _salesRepresentativeService
                    .GetAllAsync(keyword);

            // The main page only displays active representatives.
            salesRepresentatives = salesRepresentatives
                .Where(sr => sr.IsActive)
                .ToList();

            ViewBag.Keyword = keyword;

            return View(salesRepresentatives);
        }

        // Displays inactive sales representatives.
        [HttpGet]
        public async Task<IActionResult> Inactive(string? keyword)
        {
            var salesRepresentatives =
                await _salesRepresentativeService
                    .GetAllAsync(keyword);

            salesRepresentatives = salesRepresentatives
                .Where(sr => !sr.IsActive)
                .ToList();

            ViewBag.Keyword = keyword;

            return View(salesRepresentatives);
        }

        // Displays the Create Sales Representative form.
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadEligibleEmployeesAsync();

            return View(new SalesRepresentativeViewModel());
        }

        // Creates the Sales Representative.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
     SalesRepresentativeViewModel model)
        {
            // Return the form when required information is missing or invalid.
            if (!ModelState.IsValid)
            {
                await LoadEligibleEmployeesAsync(
                    model.EmployeeID);

                return View(model);
            }

            // The service checks whether the selected employee can be assigned as a sales representative.
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

        // Displays the Edit Sales Representative form.
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

        // Updates the Sales Representative.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            SalesRepresentativeViewModel model)
        {
            // Prevent invalid sales representative information from being saved.
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check that the sales representative code is unique, excluding the current sales representative.
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

        // Deactivates a Sales Representative without deleting history.
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

        // Reactivates an inactive sales representative.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reactivate(int id)
        {
            var reactivated =
                await _salesRepresentativeService
                    .ReactivateAsync(id);

            if (!reactivated)
            {
                TempData["ErrorMessage"] =
                    "The sales representative could not be reactivated. " +
                    "Make sure the linked employee is active.";

                return RedirectToAction(nameof(Inactive));
            }

            TempData["SuccessMessage"] =
                "Sales representative reactivated successfully.";

            return RedirectToAction(nameof(Inactive));
        }

        // Loads employees who are eligible to become sales representatives.
        private async Task LoadEligibleEmployeesAsync(
     string? selectedEmployeeId = null)
        {
            var employees =
                await _salesRepresentativeService
                    .GetEligibleEmployeesAsync();

            // Converts eligible employees into dropdown-friendly display values.
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