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
    // Represents a sales quotation created for a customer.
    public class Quote
    {
        // Primary key that uniquely identifies the quotation.
        [Key]
        public int QuoteId { get; set; }

        // Stores the quotation number used to identify the quote.
        [Required]
        public string QuoteNumber { get; set; }

        // Records the date on which the quotation was created.
        [Required]
        public DateTime QuoteDate { get; set; }

        // Identifies the customer receiving the quotation.
        [Required]
        public int CustomerId { get; set; }

        // Navigation property linking the quote to its customer.
        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }

        // Stores the billing address used for the quotation.
        public string BillingAddress { get; set; }

        // Stores the payment terms offered to the customer.
        public string PaymentTerms { get; set; }

        // Optional sales representative responsible for the quotation.
        public int? SalesRepresentativeId { get; set; }

        // Navigation property linking the quote to its sales representative.
        public SalesRepresentative? SalesRepresentative { get; set; }

        // Stores the current status of the quotation.
        [Required]
        public QuoteStatus Status { get; set; } = QuoteStatus.Pending;

        // Stores the quotation subtotal before the final total.
        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        // Stores the final quotation amount.
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        // Collection of products included in the quotation.
        public ICollection<QuoteItem> QuoteItems { get; set; } = new List<QuoteItem>();

        // Records when the quotation was created.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Records the most recent update time, if applicable.
        public DateTime? UpdatedAt { get; set; }
    }
}