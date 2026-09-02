using System.ComponentModel.DataAnnotations;

namespace FMCGEnterpriseManagementSystem.ViewModels
{
    public class CustomerViewModel
    {
        public string CustomerID { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Surname is required.")]
        public string Surname { get; set; } = string.Empty;

        [Required(ErrorMessage = "ID Number is required.")]
        public string IdNumber { get; set; } = string.Empty;

        public string TelephoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cell number is required.")]
        public string CellNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Physical address is required.")]
        public string PhysicalAddress { get; set; } = string.Empty;

        public string DeliveryAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Customer group is required.")]
        public string CustomerGroup { get; set; } = string.Empty;

        [Required(ErrorMessage = "Payment terms are required.")]
        public string PaymentTerms { get; set; } = string.Empty;

        [Required(ErrorMessage = "Payment method is required.")]
        public string PaymentMethod { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sales representative is required.")]
        public string SalesRep { get; set; } = string.Empty;

        public string? VATNumber { get; set; }
    }
}