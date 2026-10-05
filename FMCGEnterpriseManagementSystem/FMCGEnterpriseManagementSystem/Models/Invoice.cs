// Title: Entity Framework Core - Entity Properties
// Author: Microsoft
// Date: 12-01-2023
// Code version: Entity Framework Core
// Availability: https://learn.microsoft.com/en-us/ef/core/modeling/entity-properties

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FMCGEnterpriseManagementSystem.Enums;

namespace FMCGEnterpriseManagementSystem.Models
{
    public class Invoice
    {
        // Primary key for the invoice record.
        [Key]
        public int InvoiceId { get; set; }

        // Unique invoice number used to identify the invoice.
        [Required]
        public string InvoiceNumber { get; set; } = string.Empty;

        // Date on which the invoice was issued.
        [Required]
        public DateTime InvoiceDate { get; set; }

        // Optional quote associated with the invoice.
        public int? QuoteId { get; set; }

        // Navigation property linking the invoice to its quote.
        [ForeignKey("QuoteId")]
        public Quote? Quote { get; set; }

        // Identifies the customer receiving the invoice.
        [Required]
        public int CustomerId { get; set; }

        // Navigation property linking the invoice to its customer.
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; } = null!;

        // Stores the customer's billing address shown on the invoice.
        public string BillingAddress { get; set; } = string.Empty;

        // Optional business name displayed on the invoice.
        public string? BusinessName { get; set; }

        // Stores the payment terms agreed with the customer.
        public string PaymentTerms { get; set; } = string.Empty;

        // Optional sales representative responsible for the invoice.
        public int? SalesRepresentativeId { get; set; }

        // Navigation property linking the invoice to its sales representative.
        public SalesRepresentative? SalesRepresentative { get; set; }

        // Stores the current invoice status, such as Draft or Paid.
        public string Status { get; set; } = "Draft";

        // Stores the invoice subtotal before the final total.
        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        // Stores the final invoice amount.
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        // Collection of products and quantities included on the invoice.
        public ICollection<InvoiceItem> InvoiceItems { get; set; } =
            new List<InvoiceItem>();

        // Collection of payments recorded against the invoice.
        public ICollection<Payment> Payments { get; set; } =
            new List<Payment>();

        // Records when the invoice was created.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Records the most recent update time, if applicable.
        public DateTime? UpdatedAt { get; set; }
    }
}