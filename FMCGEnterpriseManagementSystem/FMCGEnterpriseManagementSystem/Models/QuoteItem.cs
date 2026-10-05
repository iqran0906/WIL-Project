// Title: Entity Framework Core - Entity Properties
// Author: Microsoft
// Date: 12-01-2023
// Code version: Entity Framework Core
// Availability: https://learn.microsoft.com/en-us/ef/core/modeling/entity-properties
//
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FMCGEnterpriseManagementSystem.Models
{
    // Represents an individual product included in a customer quotation.
    public class QuoteItem
    {
        // Primary key that uniquely identifies the quotation item.
        [Key]
        public int QuoteItemId { get; set; }

        // Identifies the quotation that contains this item.
        [Required]
        public int QuoteId { get; set; }

        // Navigation property linking the item to its quotation.
        [ForeignKey("QuoteId")]
        public Quote? Quote { get; set; }

        // Identifies the product included in the quotation.
        [Required]
        public int ProductId { get; set; }

        // Navigation property linking the item to its product.
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        // Stores the quantity of the product being quoted.
        [Required]
        public int Quantity { get; set; }

        // Stores the quoted selling price for one unit.
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        // Stores the discount percentage applied to the item.
        [Column(TypeName = "decimal(5,2)")]
        public decimal DiscountPercent { get; set; }

        // Stores the VAT category applied to the product.
        public string VatCategory { get; set; }

        // Stores the total value of the quotation line.
        [Column(TypeName = "decimal(18,2)")]
        public decimal LineTotal { get; set; }

        // Stores the quotation line total before VAT.
        [Column(TypeName = "decimal(18,2)")]
        public decimal LineTotalExclVat { get; set; }
    }
}