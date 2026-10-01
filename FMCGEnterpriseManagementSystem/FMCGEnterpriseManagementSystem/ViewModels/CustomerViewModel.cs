// Purpose: Data and validation rules for the customer pages and forms.
// Authors: Naseeha27, Maseeha17 (from git history)

using System.ComponentModel.DataAnnotations;

namespace FMCGEnterpriseManagementSystem.ViewModels
{
    public class CustomerViewModel
    {
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Surname is required.")]
        public string Surname { get; set; } = string.Empty;

        [Required(ErrorMessage = "ID Number is required.")]
        public string IdNumber { get; set; } = string.Empty;

        public string? TelephoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cell number is required.")]
        public string CellNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Physical address is required.")]
        public string PhysicalAddress { get; set; } = string.Empty;

        public string? DeliveryAddress { get; set; } 

        [Required(ErrorMessage = "Customer group is required.")]
        public string CustomerGroup { get; set; } = string.Empty;

        [Required(ErrorMessage = "Payment terms are required.")]
        public string PaymentTerms { get; set; } = string.Empty;

        [Required(ErrorMessage = "Payment method is required.")]
        public string PaymentMethod { get; set; } = string.Empty;

        public string? Notes { get; set; } = string.Empty;

        public string? SalesRep { get; set; } = string.Empty;

        public string? VATNumber { get; set; }

        
        public int? SalesRepresentativeId { get; set; }
    }
}