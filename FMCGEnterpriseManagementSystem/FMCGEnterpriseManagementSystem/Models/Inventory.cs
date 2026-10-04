// Title: Entity Framework Core - Entity Properties
// Author: Maseeha17
// Date: 12-01-2023
// Code version: Entity Framework Core
// Availability: https://learn.microsoft.com/en-us/ef/core/modeling/entity-properties

using System.ComponentModel.DataAnnotations;
using FMCGEnterpriseManagementSystem.Models;
using System.ComponentModel.DataAnnotations.Schema;

public class Inventory
{
    // Primary key for the inventory record.
    [Key]
    public int InventoryId { get; set; }

    // Identifies the product associated with this inventory record.
    [Required]
    public int ProductId { get; set; }

    // Navigation property linking inventory to its product.
    [ForeignKey("ProductId")]
    public Product Product { get; set; } = null!;

    // Stores the current quantity available in stock.
    public int QuantityOnHand { get; set; }

    // Stores the minimum stock level before a reorder is required.
    public int ReorderLevel { get; set; }

    // Optional notes about the inventory record.
    public string? Notes { get; set; }

    // Records when the inventory record was created.
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Records the most recent update time, if the record has been changed.
    public DateTime? UpdatedAt { get; set; }

    // Collection of stock batches associated with this inventory record.
    public ICollection<StockBatch> StockBatches { get; set; } = new List<StockBatch>();
}