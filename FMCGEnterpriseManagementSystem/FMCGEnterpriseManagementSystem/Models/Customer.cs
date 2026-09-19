using System.ComponentModel.DataAnnotations;

namespace FMCGEnterpriseManagementSystem.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        public string CustomerID { get; set; } = string.Empty;

        [Required]
        public string CompanyName { get; set; } = string.Empty;

        [Required]
        public string ContactPerson { get; set; } = string.Empty;

        [Required]
        public string ContactNumber { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string PhysicalAddress { get; set; } = string.Empty;
        public string DeliveryAddress { get; set; } = string.Empty;
        public string CustomerGroup { get; set; } = string.Empty;
        public string PaymentTerms { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string VATNumber { get; set; } = string.Empty;
    }
}