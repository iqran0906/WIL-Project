// Purpose: Settings page: company profile, banking details (admin) and personal preferences.
// Authors: ST10068525 (new file, not yet committed)

using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    // Everyone can open Settings (personal preferences + account);
    // only administrators can change the company profile and banking details.
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly ISettingsService _settingsService;
        private readonly ILogger<SettingsController> _logger;

        public SettingsController(ISettingsService settingsService, ILogger<SettingsController> logger)
        {
            _settingsService = settingsService;
            _logger = logger;
        }

        // GET: Settings
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var settings = await _settingsService.GetAsync();

            var model = ToViewModel(settings);
            model.CanEditSystemSettings = User.IsInRole("Administrator");

            return View(model);
        }

        // POST: Settings
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Index(SettingsViewModel model)
        {
            model.CanEditSystemSettings = true;

            var current = await _settingsService.GetAsync();

            if (!ModelState.IsValid)
            {
                model.UpdatedAt = current.UpdatedAt;
                model.UpdatedBy = current.UpdatedBy;
                return View(model);
            }

            await _settingsService.UpdateAsync(ApplyTo(current, model), User.Identity?.Name);

            _logger.LogInformation("Company settings updated by {User}", User.Identity?.Name);

            TempData["SuccessMessage"] = "Settings saved.";
            return RedirectToAction(nameof(Index));
        }

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

        // Copies the page's fields onto the current settings. Values that are not on the
        // page (VAT rate, number prefixes, notifications, ...) are kept as they are.
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

        private static string? Clean(string? value) =>
            string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
