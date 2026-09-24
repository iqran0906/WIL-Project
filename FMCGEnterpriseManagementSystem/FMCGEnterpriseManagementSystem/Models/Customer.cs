using System.ComponentModel.DataAnnotations;

namespace FMCGEnterpriseManagementSystem.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Surname { get; set; } = string.Empty;

        public string IdNumber { get; set; } = string.Empty;

        public string TelephoneNumber { get; set; } = string.Empty;

        public string CellNumber { get; set; } = string.Empty;

        [Required]
        public string ContactNumber { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? PhysicalAddress { get; set; }

        public string? DeliveryAddress { get; set; }

        public string? CustomerGroup { get; set; }

        public string? PaymentTerms { get; set; }

        public string? PaymentMethod { get; set; }

        public string? SalesRep { get; set; }

        public string? Notes { get; set; }

        public int? SalesRepresentativeId { get; set; }

        public SalesRepresentative? SalesRepresentative { get; set; }

        public string? VATNumber { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}