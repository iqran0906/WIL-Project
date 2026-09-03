namespace FMCGEnterpriseManagementSystem.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string IdNumber { get; set; }

        public string TelephoneNumber { get; set; }

        public string CellNumber { get; set; }

        public string Email { get; set; }

        public string PhysicalAddress { get; set; }

        public string DeliveryAddress { get; set; }

        public string CustomerGroup { get; set; }

        public string PaymentTerms { get; set; }

        public string PaymentMethod { get; set; }

        public string Notes { get; set; }

        public int? SalesRepresentativeId { get; set; }

        public SalesRepresentative? SalesRepresentative { get; set; }

        public string? VATNumber { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}