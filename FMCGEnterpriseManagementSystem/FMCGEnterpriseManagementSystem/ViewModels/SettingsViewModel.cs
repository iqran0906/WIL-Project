//  Title: Model Validation in ASP.NET Core MVC
//    Author: Microsoft
//    Date: 2026
//    Code version: ASP.NET Core
//    Availability: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation

using System.ComponentModel.DataAnnotations;

namespace FMCGEnterpriseManagementSystem.ViewModels
{
    // ViewModel containing company profile and banking information.
    // Fields are optional so the settings can be completed and saved incrementally;
    // validation is applied when a value is supplied.
    public class SettingsViewModel
    {
        // Indicates whether the current user has permission to edit
        // company-wide system settings.
        public bool CanEditSystemSettings { get; set; }

        // Stores the date and time when the settings were last updated.
        public DateTime? UpdatedAt { get; set; }

        // Stores the identifier or name of the user who last updated the settings.
        public string? UpdatedBy { get; set; }

        // Indicates whether company profile details have been provided.
        public bool HasCompanyDetails =>
            !string.IsNullOrWhiteSpace(CompanyName) || !string.IsNullOrWhiteSpace(PhysicalAddress);

        // Indicates whether banking details have been provided.
        public bool HasBankingDetails =>
            !string.IsNullOrWhiteSpace(BankName) || !string.IsNullOrWhiteSpace(BankAccountNumber);

        // ---------- Company profile ----------

        // Stores the registered or trading name of the company.
        [StringLength(150)]
        [Display(Name = "Company Name")]
        public string? CompanyName { get; set; }

        // Stores the company's CIPC registration number.
        [RegularExpression(@"^\d{4}/\d{6}/\d{2}$", ErrorMessage = "Use the CIPC format, e.g. 2015/123456/07.")]
        [Display(Name = "Company Registration No.")]
        public string? RegistrationNumber { get; set; }

        // Stores the company's VAT registration number.
        [RegularExpression(@"^4\d{9}$", ErrorMessage = "VAT number must be 10 digits and start with 4.")]
        [Display(Name = "VAT Registration No.")]
        public string? CompanyVatNumber { get; set; }

        // Stores the company's physical address.
        [StringLength(300)]
        [Display(Name = "Physical Address")]
        public string? PhysicalAddress { get; set; }

        // Stores the city where the company is located.
        [StringLength(100)]
        public string? City { get; set; }

        // Stores the company's four-digit postal code.
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Postal code must be 4 digits.")]
        [Display(Name = "Postal Code")]
        public string? PostalCode { get; set; }

        // Stores the company's contact telephone number.
        [RegularExpression(@"^\+?[0-9 ()-]{10,15}$", ErrorMessage = "Enter a valid phone number, e.g. 031 123 4567.")]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        // Stores the company's email address.
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [StringLength(150)]
        [Display(Name = "Company Email")]
        public string? Email { get; set; }

        // Stores the company's website address.
        [Url(ErrorMessage = "Enter a full web address, e.g. https://www.example.co.za.")]
        [StringLength(200)]
        public string? Website { get; set; }

        // ---------- Banking ----------

        // Stores the name of the company's bank.
        [StringLength(100)]
        [Display(Name = "Bank Name")]
        public string? BankName { get; set; }

        // Stores the name of the account holder.
        [StringLength(150)]
        [Display(Name = "Account Holder")]
        public string? BankAccountName { get; set; }

        // Stores the company's bank account number.
        [RegularExpression(@"^\d{6,16}$", ErrorMessage = "Account number must be 6 to 16 digits.")]
        [Display(Name = "Account Number")]
        public string? BankAccountNumber { get; set; }

        // Stores the bank branch code.
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Branch code must be 6 digits.")]
        [Display(Name = "Branch Code")]
        public string? BranchCode { get; set; }

        // Stores the payment reference used for banking transactions.
        [StringLength(100)]
        [Display(Name = "Payment Reference")]
        public string? PaymentReference { get; set; }

        // Stores the email address used for receiving proof of payment.
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [StringLength(150)]
        [Display(Name = "Proof of Payment Email")]
        public string? ProofOfPaymentEmail { get; set; }
    }
}