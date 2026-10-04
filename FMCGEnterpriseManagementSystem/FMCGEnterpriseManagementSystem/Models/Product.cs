// Title: Entity Framework Core - Entity Properties
// Author: Microsoft
// Date: 12-01-2023
// Code version: Entity Framework Core
// Availability: https://learn.microsoft.com/en-us/ef/core/modeling/entity-properties

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FMCGEnterpriseManagementSystem.Models
{
    // Represents a product managed and sold by the business.
    public class Product
    {
        // Primary key that uniquely identifies the product.
        [Key]
        public int ProductId { get; set; }

        // Stores the unique business code used to identify the product.
        [Required]
        [StringLength(20)]
        public string ProductCode { get; set; } = string.Empty;

        // Stores the name of the product.
        [Required]
        [StringLength(100)]
        public string ProductName { get; set; } = string.Empty;

        // Optional description providing additional product information.
        [StringLength(250)]
        public string? Description { get; set; }

        // Stores the purchase cost of the product excluding VAT.
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CostExVat { get; set; }

        // Stores the purchase cost of the product including VAT.
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CostIncVat { get; set; }

        // Stores the selling price of the product.
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SellingPrice { get; set; }

        // Stores the category used to classify the product.
        [Required]
        [StringLength(50)]
        public string Category { get; set; } = string.Empty;

        // Indicates whether the product is currently available for use.
        public bool IsActive { get; set; } = true;

        // Identifies the supplier associated with the product.
        [Required]
        public int SupplierId { get; set; }

        // Navigation property linking the product to its supplier.
        [ForeignKey("SupplierId")]
        public Supplier? Supplier { get; set; }

        // Navigation property linking the product to its inventory record.
        public Inventory? Inventory { get; set; }

        // Records when the product was created.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Records the most recent update time, if applicable.
        public DateTime? UpdatedAt { get; set; }
    }
}