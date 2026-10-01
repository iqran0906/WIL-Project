// Purpose: Data and validation rules for the supplier pages and forms.
// Authors: iqran0906, Naseeha27, Maseeha17 (from git history)

using System.ComponentModel.DataAnnotations;

namespace FMCGEnterpriseManagementSystem.ViewModels
{
    public class SupplierViewModel
    {
        public int SupplierId { get; set; }

        [Required(ErrorMessage = "Company name is required.")]
        [StringLength(150, ErrorMessage = "Company name cannot be longer than 150 characters.")]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact person is required.")]
        [StringLength(100, ErrorMessage = "Contact person cannot be longer than 100 characters.")]
        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact number is required.")]
        [RegularExpression(@"^\+?[0-9 ()-]{10,15}$", ErrorMessage = "Enter a valid contact number (10 to 15 digits, e.g. 031 123 4567).")]
        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Physical address is required.")]
        [StringLength(300, ErrorMessage = "Physical address cannot be longer than 300 characters.")]
        [Display(Name = "Physical Address")]
        public string PhysicalAddress { get; set; } = string.Empty;

        [Range(0, 100000000, ErrorMessage = "Credit limit must be between R0 and R100 000 000.")]
        [Display(Name = "Credit Limit")]
        public decimal CreditLimit { get; set; }

        [Required(ErrorMessage = "Credit terms are required.")]
        [StringLength(50)]
        [Display(Name = "Credit Terms")]
        public string CreditTerms { get; set; } = string.Empty;

        // South African VAT numbers are 10 digits starting with 4
        [Required(ErrorMessage = "VAT number is required.")]
        [RegularExpression(@"^4\d{9}$", ErrorMessage = "VAT number must be 10 digits and start with 4.")]
        [Display(Name = "VAT Number")]
        public string VATNumber { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Notes cannot be longer than 500 characters.")]
        public string? Notes { get; set; }

        [Display(Name = "Status")]
        public bool IsActive { get; set; } = true;
    }
}
