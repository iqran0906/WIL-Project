using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FMCGEnterpriseManagementSystem.Enums;

namespace FMCGEnterpriseManagementSystem.Models
{
    public class Invoice
    {
        [Key]
        public int InvoiceId { get; set; }

        [Required]
        public string InvoiceNumber { get; set; } = string.Empty;

        [Required]
        public DateTime InvoiceDate { get; set; }

        public int? QuoteId { get; set; }

        [ForeignKey("QuoteId")]
        public Quote? Quote { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; } = null!;

        public string BillingAddress { get; set; } = string.Empty;

        public string PaymentTerms { get; set; } = string.Empty;

        public int? SalesRepresentativeId { get; set; }

        public SalesRepresentative? SalesRepresentative { get; set; }

        public string Status { get; set; } = "Draft";

        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        public ICollection<InvoiceItem> InvoiceItems { get; set; } =
            new List<InvoiceItem>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}