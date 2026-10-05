// Title: Entity Framework Core - Entity Properties
// Author: Microsoft
// Date: 12-01-2023
// Code version: Entity Framework Core
// Availability: https://learn.microsoft.com/en-us/ef/core/modeling/entity-properties

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FMCGEnterpriseManagementSystem.Models
{
    // Represents a payment made against an invoice.
    public class Payment
    {
        // Primary key that uniquely identifies the payment.
        [Key]
        public int PaymentId { get; set; }

        // Identifies the invoice that the payment is linked to.
        [Required]
        public int InvoiceId { get; set; }

        // Navigation property linking the payment to its invoice.
        [ForeignKey("InvoiceId")]
        public Invoice Invoice { get; set; } = null!;

        // Records the date and time when the payment was made.
        [Required]
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        // Stores the method used to make the payment.
        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = string.Empty;

        // Stores the amount paid towards the invoice.
        [Column(TypeName = "decimal(18,2)")]
        public decimal AmountPaid { get; set; }

        // Records when the payment record was created.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Records the most recent update time, if applicable.
        public DateTime? UpdatedAt { get; set; }
    }
}