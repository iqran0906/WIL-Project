using System.ComponentModel.DataAnnotations;

namespace FMCGEnterpriseManagementSystem.ViewModels
{
    public class SupplierViewModel
    {
        public string SupplierID { get; set; } = string.Empty;

        [Required(ErrorMessage = "Company name is required.")]
        public string CompanyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact person is required.")]
        public string ContactPerson { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact number is required.")]
        public string ContactNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Physical address is required.")]
        public string PhysicalAddress { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Credit limit must be a non-negative number.")]
        public decimal CreditLimit { get; set; }

        [Required(ErrorMessage = "Credit terms are required.")]
        public string CreditTerms { get; set; } = string.Empty;

        public string VATNumber { get; set; } = string.Empty;

        public string? Notes { get; set; }
    }
}