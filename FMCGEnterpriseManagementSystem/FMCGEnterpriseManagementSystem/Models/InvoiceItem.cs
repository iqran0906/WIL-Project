// Title: Entity Framework Core - Entity Properties
// Author: Microsoft
// Date: 12-01-2023
// Code version: Entity Framework Core
// Availability: https://learn.microsoft.com/en-us/ef/core/modeling/entity-properties

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FMCGEnterpriseManagementSystem.Models
{
    public class InvoiceItem
    {
        // Primary key for the invoice item record.
        [Key]
        public int InvoiceItemId { get; set; }

        // Identifies the invoice that contains this item.
        [Required]
        public int InvoiceId { get; set; }

        // Navigation property linking the item to its invoice.
        [ForeignKey("InvoiceId")]
        public Invoice Invoice { get; set; } = null!;

        // Identifies the product included on the invoice.
        [Required]
        public int ProductId { get; set; }

        // Navigation property linking the item to its product.
        [ForeignKey("ProductId")]
        public Product Product { get; set; } = null!;

        // Stores the quantity of the product being invoiced.
        [Required]
        public int Quantity { get; set; }

        // Stores the selling price for one unit of the product.
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        // Stores the discount percentage applied to the item.
        [Column(TypeName = "decimal(5,2)")]
        public decimal DiscountPercent { get; set; }

        // Stores the VAT category applied to the product.
        public string VatCategory { get; set; } = string.Empty;

        // Stores the calculated total for this invoice line.
        [Column(TypeName = "decimal(18,2)")]
        public decimal LineTotal { get; set; }
    }
}