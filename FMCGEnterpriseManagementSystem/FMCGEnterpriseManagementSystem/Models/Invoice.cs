using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FMCGEnterpriseManagementSystem.Enums;

namespace FMCGEnterpriseManagementSystem.Models
{
    public class Invoice
    {
        public int Id { get; set; }

        [Required]
        public string InvoiceNumber { get; set; }

        [Required]
        public DateTime InvoiceDate { get; set; }

        [Required]
        public string CustomerId { get; set; }
        public Customer Customer { get; set; }

        public string PaymentTerms { get; set; }

        public int? SalesPersonId { get; set; }

        public List<InvoiceItem> Items { get; set; } = new();

        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal VatTotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
    }
}