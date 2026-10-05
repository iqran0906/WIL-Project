// Title: Customer entity for storing customer information and account details.
// Authors: Maseeha17
// Date: 12-01-2023
// Code version: ASP.NET Core 10.0
// Availability: https://learn.microsoft.com/en-us/ef/core/modeling/entity-properties

namespace FMCGEnterpriseManagementSystem.Models
{
    // Represents a customer and their business, contact and payment information.
    public class Customer
    {
        // Unique identifier for the customer.
        public int CustomerId { get; set; }

        // Customer's first name.
        public string Name { get; set; }

        // Customer's surname.
        public string Surname { get; set; }

        // Customer's identification number.
        public string IdNumber { get; set; }

        // Customer's telephone contact number.
        public string TelephoneNumber { get; set; }

        // Customer's mobile contact number.
        public string CellNumber { get; set; }

        // Optional customer email address.
        public string? Email { get; set; }

        // Customer's physical/business address.
        public string PhysicalAddress { get; set; }

        // Optional delivery address if different from the physical address.
        public string? DeliveryAddress { get; set; }

        // Groups the customer according to the business customer classification.
        public string CustomerGroup { get; set; }

        // Defines the payment terms agreed with the customer.
        public string PaymentTerms { get; set; }

        // Defines the customer's preferred payment method.
        public string PaymentMethod { get; set; }

        // Stores additional notes relating to the customer.
        public string Notes { get; set; }

        // Optional link to the sales representative assigned to the customer.
        public int? SalesRepresentativeId { get; set; }

        // Navigation property for the customer's assigned sales representative.
        public SalesRepresentative? SalesRepresentative { get; set; }

        // Optional VAT registration number for the customer.
        public string? VATNumber { get; set; }

        // Indicates whether the customer is currently active.
        public bool IsActive { get; set; } = true;

        // Records when the customer was created.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Records when the customer information was last updated.
        public DateTime? UpdatedAt { get; set; }
    }
}