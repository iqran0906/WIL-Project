// Title: Entity Framework Core - Entity Properties
// Author: Microsoft
// Date: 12-01-2023
// Code version: Entity Framework Core
// Availability: https://learn.microsoft.com/en-us/ef/core/modeling/entity-properties
//

using System.ComponentModel.DataAnnotations;

namespace FMCGEnterpriseManagementSystem.Models
{
    // Represents a specific batch of stock stored in inventory.
    public class StockBatch
    {
        // Primary key that uniquely identifies the stock batch.
        [Key]
        public int StockBatchId { get; set; }

        // Identifies the inventory record associated with this batch.
        [Required]
        public int InventoryId { get; set; }

        // Stores the unique batch number supplied for the stock.
        [Required]
        [StringLength(50)]
        public string BatchNumber { get; set; } = string.Empty;

        // Stores the quantity of products available in this batch.
        public int Quantity { get; set; }

        // Records the date on which the stock batch was received.
        public DateTime ReceivedDate { get; set; }

        // Records the expiry date of the stock batch.
        public DateTime ExpiryDate { get; set; }

        // Records when the stock batch record was created.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Records the most recent update time, if applicable.
        public DateTime? UpdatedAt { get; set; }

        // Navigation property linking the batch to its inventory record.
        public Inventory Inventory { get; set; } = null!;
    }
}