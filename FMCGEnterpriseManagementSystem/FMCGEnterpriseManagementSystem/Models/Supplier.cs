// Title: Entity Framework Core - Entity Properties
// Author: Maseeha17
// Date: 12-01-2023
// Code version: Entity Framework Core
// Availability: https://learn.microsoft.com/en-us/ef/core/modeling/entity-properties

namespace FMCGEnterpriseManagementSystem.Models
{
    // Represents a supplier that provides products to the business.
    public class Supplier
    {
        // Primary key that uniquely identifies the supplier.
        public int SupplierId { get; set; }

        // Stores the supplier's registered company name.
        public string CompanyName { get; set; }

        // Stores the name of the main supplier contact person.
        public string ContactPerson { get; set; }

        // Stores the supplier's contact telephone number.
        public string ContactNumber { get; set; }

        // Stores the supplier's optional email address.
        public string? Email { get; set; }

        // Stores the supplier's physical business address.
        public string PhysicalAddress { get; set; }

        // Stores the maximum credit amount available from the supplier.
        public decimal CreditLimit { get; set; }

        // Stores the payment terms agreed with the supplier.
        public string CreditTerms { get; set; }

        // Stores the supplier's VAT registration number.
        public string VATNumber { get; set; }

        // Stores additional notes about the supplier.
        public string Notes { get; set; }

        // Indicates whether the supplier is currently active.
        public bool IsActive { get; set; } = true;

        // Records when the supplier record was created.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Records the most recent update time, if applicable.
        public DateTime? UpdatedAt { get; set; }
    }
}