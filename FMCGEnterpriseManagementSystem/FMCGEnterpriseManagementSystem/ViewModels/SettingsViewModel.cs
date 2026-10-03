/***************************************************************************************
*    Title: Settings View Model
*    Author: ST10068525
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/ViewModels/SettingsViewModel.cs
***************************************************************************************/

using System.ComponentModel.DataAnnotations;

namespace FMCGEnterpriseManagementSystem.ViewModels
{
    // Company profile + banking details. Every field is optional so the details
    // can be added and saved bit by bit; anything entered must be valid.
    public class SettingsViewModel
    {
        // Only administrators can change company-wide settings
        public bool CanEditSystemSettings { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        public bool HasCompanyDetails =>
            !string.IsNullOrWhiteSpace(CompanyName) || !string.IsNullOrWhiteSpace(PhysicalAddress);

        public bool HasBankingDetails =>
            !string.IsNullOrWhiteSpace(BankName) || !string.IsNullOrWhiteSpace(BankAccountNumber);

        // ---------- Company profile ----------
        [StringLength(150)]
        [Display(Name = "Company Name")]
        public string? CompanyName { get; set; }

        // CIPC format, e.g. 2015/123456/07
        [RegularExpression(@"^\d{4}/\d{6}/\d{2}$", ErrorMessage = "Use the CIPC format, e.g. 2015/123456/07.")]
        [Display(Name = "Company Registration No.")]
        public string? RegistrationNumber { get; set; }

        [RegularExpression(@"^4\d{9}$", ErrorMessage = "VAT number must be 10 digits and start with 4.")]
        [Display(Name = "VAT Registration No.")]
        public string? CompanyVatNumber { get; set; }

        [StringLength(300)]
        [Display(Name = "Physical Address")]
        public string? PhysicalAddress { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [RegularExpression(@"^\d{4}$", ErrorMessage = "Postal code must be 4 digits.")]
        [Display(Name = "Postal Code")]
        public string? PostalCode { get; set; }

        [RegularExpression(@"^\+?[0-9 ()-]{10,15}$", ErrorMessage = "Enter a valid phone number, e.g. 031 123 4567.")]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [StringLength(150)]
        [Display(Name = "Company Email")]
        public string? Email { get; set; }

        [Url(ErrorMessage = "Enter a full web address, e.g. https://www.example.co.za.")]
        [StringLength(200)]
        public string? Website { get; set; }

        // ---------- Banking ----------
        [StringLength(100)]
        [Display(Name = "Bank Name")]
        public string? BankName { get; set; }

        [StringLength(150)]
        [Display(Name = "Account Holder")]
        public string? BankAccountName { get; set; }

        [RegularExpression(@"^\d{6,16}$", ErrorMessage = "Account number must be 6 to 16 digits.")]
        [Display(Name = "Account Number")]
        public string? BankAccountNumber { get; set; }

        [RegularExpression(@"^\d{6}$", ErrorMessage = "Branch code must be 6 digits.")]
        [Display(Name = "Branch Code")]
        public string? BranchCode { get; set; }

        [StringLength(100)]
        [Display(Name = "Payment Reference")]
        public string? PaymentReference { get; set; }

        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [StringLength(150)]
        [Display(Name = "Proof of Payment Email")]
        public string? ProofOfPaymentEmail { get; set; }
    }
}
