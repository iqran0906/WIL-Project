//    Title: Model Validation in ASP.NET Core MVC
//    Author: Microsoft
//    Date: 2026
//    Code version: ASP.NET Core
//    Availability: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation

using System.ComponentModel.DataAnnotations;

namespace FMCGEnterpriseManagementSystem.ViewModels
{
    // ViewModel used to transfer supplier information between
    // supplier forms, controllers, services, and views.
    public class SupplierViewModel
    {
        // Stores the unique identifier of the supplier.
        public int SupplierId { get; set; }

        // Stores the supplier company's name.
        [Required(ErrorMessage = "Company name is required.")]
        [StringLength(150, ErrorMessage = "Company name cannot be longer than 150 characters.")]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; } = string.Empty;

        // Stores the name of the main contact person at the supplier.
        [Required(ErrorMessage = "Contact person is required.")]
        [StringLength(100, ErrorMessage = "Contact person cannot be longer than 100 characters.")]
        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; } = string.Empty;

        // Stores the supplier's contact telephone number.
        [Required(ErrorMessage = "Contact number is required.")]
        [RegularExpression(@"^\+?[0-9 ()-]{10,15}$", ErrorMessage = "Enter a valid contact number (10 to 15 digits, e.g. 031 123 4567).")]
        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set; } = string.Empty;

        // Stores the supplier's email address when provided.
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [StringLength(150)]
        public string? Email { get; set; }

        // Stores the physical address of the supplier.
        [Required(ErrorMessage = "Physical address is required.")]
        [StringLength(300, ErrorMessage = "Physical address cannot be longer than 300 characters.")]
        [Display(Name = "Physical Address")]
        public string PhysicalAddress { get; set; } = string.Empty;

        // Stores the credit limit available from the supplier.
        [Range(0, 100000000, ErrorMessage = "Credit limit must be between R0 and R100 000 000.")]
        [Display(Name = "Credit Limit")]
        public decimal CreditLimit { get; set; }

        // Stores the agreed credit/payment terms for the supplier.
        [Required(ErrorMessage = "Credit terms are required.")]
        [StringLength(50)]
        [Display(Name = "Credit Terms")]
        public string CreditTerms { get; set; } = string.Empty;

        // Stores the supplier's South African VAT registration number.
        // The validation requires a 10-digit number beginning with 4.
        [Required(ErrorMessage = "VAT number is required.")]
        [RegularExpression(@"^4\d{9}$", ErrorMessage = "VAT number must be 10 digits and start with 4.")]
        [Display(Name = "VAT Number")]
        public string VATNumber { get; set; } = string.Empty;

        // Stores optional notes relating to the supplier.
        [StringLength(500, ErrorMessage = "Notes cannot be longer than 500 characters.")]
        public string? Notes { get; set; }

        // Indicates whether the supplier is currently active.
        [Display(Name = "Status")]
        public bool IsActive { get; set; } = true;
    }
}