using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FMCGEnterpriseManagementSystem.Enums;

namespace FMCGEnterpriseManagementSystem.Models
{
    public class Quote
    {
        [Key]
        public int QuoteId { get; set; }

        [Required]
        public string QuoteNumber { get; set; }

        [Required]
        public DateTime QuoteDate { get; set; }

        public DateTime ExpiryDate { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        public string BillingAddress { get; set; }

        public string PaymentTerms { get; set; }

        public int? SalesRepresentativeId { get; set; }

        public SalesRepresentative? SalesRepresentative { get; set; }

        [Required]
        public QuoteStatus Status { get; set; } = QuoteStatus.Draft;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        public ICollection<QuoteItem> QuoteItems { get; set; } = new List<QuoteItem>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}