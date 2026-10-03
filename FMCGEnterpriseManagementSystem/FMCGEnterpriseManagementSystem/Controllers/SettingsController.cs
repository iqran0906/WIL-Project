/***************************************************************************************
*    Title: Settings Controller
*    Author: ST10068525
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Controllers/SettingsController.cs
***************************************************************************************/

/***************************************************************************************
*    Title: Role-based authorization in ASP.NET Core
*    Author: Microsoft
*    Date: 2026
*    Code version: ASP.NET Core
*    Availability: https://learn.microsoft.com/aspnet/core/security/authorization/roles
***************************************************************************************/

using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    // Allows authenticated users to access their settings and personal preferences.
    // Only administrators can submit changes to system settings.
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly ISettingsService _settingsService;
        private readonly ILogger<SettingsController> _logger;

        // Injects the settings service and logger through dependency injection.
        public SettingsController(ISettingsService settingsService, ILogger<SettingsController> logger)
        {
            _settingsService = settingsService;
            _logger = logger;
        }

        // GET: Settings
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Retrieves the current system settings from the service.
            var settings = await _settingsService.GetAsync();

            var model = ToViewModel(settings);

            // Controls whether the current user can edit system settings.
            model.CanEditSystemSettings = User.IsInRole("Administrator");

            return View(model);
        }

        // POST: Settings
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Index(SettingsViewModel model)
        {
            // The POST action is administrator-only, so editing is enabled.
            model.CanEditSystemSettings = true;

            var current = await _settingsService.GetAsync();

            // Preserve existing audit information when validation fails.
            if (!ModelState.IsValid)
            {
                model.UpdatedAt = current.UpdatedAt;
                model.UpdatedBy = current.UpdatedBy;
                return View(model);
            }

            // Applies editable values while retaining system settings that are not part of this page.
            await _settingsService.UpdateAsync(ApplyTo(current, model), User.Identity?.Name);

            // Records which authenticated user updated the company settings.
            _logger.LogInformation("Company settings updated by {User}", User.Identity?.Name);

            TempData["SuccessMessage"] = "Settings saved.";
            return RedirectToAction(nameof(Index));
        }

        // Converts the database settings model into the ViewModel used by the page.
        private static SettingsViewModel ToViewModel(SystemSetting s) => new()
        {
            UpdatedAt = s.UpdatedAt,
            UpdatedBy = s.UpdatedBy,

            CompanyName = s.CompanyName,
            RegistrationNumber = s.RegistrationNumber,
            CompanyVatNumber = s.CompanyVatNumber,
            PhysicalAddress = s.PhysicalAddress,
            City = s.City,
            PostalCode = s.PostalCode,
            PhoneNumber = s.PhoneNumber,
            Email = s.Email,
            Website = s.Website,

            BankName = s.BankName,
            BankAccountName = s.BankAccountName,
            BankAccountNumber = s.BankAccountNumber,
            BranchCode = s.BranchCode,
            PaymentReference = s.PaymentReference,
            ProofOfPaymentEmail = s.ProofOfPaymentEmail
        };

        // Copies editable page fields onto the current settings.
        // Values not displayed on this page are retained from the existing settings.
        private static SystemSetting ApplyTo(SystemSetting current, SettingsViewModel m) => new()
        {
            CompanyName = Clean(m.CompanyName) ?? string.Empty,
            RegistrationNumber = Clean(m.RegistrationNumber),
            CompanyVatNumber = Clean(m.CompanyVatNumber),
            PhysicalAddress = Clean(m.PhysicalAddress) ?? string.Empty,
            City = Clean(m.City),
            PostalCode = Clean(m.PostalCode),
            PhoneNumber = Clean(m.PhoneNumber),
            Email = Clean(m.Email),
            Website = Clean(m.Website),

            BankName = Clean(m.BankName),
            BankAccountName = Clean(m.BankAccountName),
            BankAccountNumber = Clean(m.BankAccountNumber),
            BranchCode = Clean(m.BranchCode),
            PaymentReference = Clean(m.PaymentReference),
            ProofOfPaymentEmail = Clean(m.ProofOfPaymentEmail),

            VatRatePercent = current.VatRatePercent,
            InvoicePrefix = current.InvoicePrefix,
            QuotePrefix = current.QuotePrefix,
            QuoteValidityDays = current.QuoteValidityDays,
            DefaultPaymentTerms = current.DefaultPaymentTerms,
            InvoiceFooterNote = current.InvoiceFooterNote,
            EmailNotificationsEnabled = current.EmailNotificationsEnabled,
            NotificationEmail = current.NotificationEmail
        };

        // Removes unnecessary whitespace and converts blank values to null.
        private static string? Clean(string? value) =>
            string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}