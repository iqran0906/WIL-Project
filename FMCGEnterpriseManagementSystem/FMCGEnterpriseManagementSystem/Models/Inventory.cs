using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 

namespace FMCGEnterpriseManagementSystem.Models
{
    public class Inventory
    {
        [Key]
        public int InventoryId { get; set; }

        [Required]
        public int ProductId { get; set; }

        // Explicitly map this navigation property to prevent EF Core from creating 'ProductId1'
        [ForeignKey("ProductId")]
        public Product Product { get; set; } = null!;

        public int QuantityOnHand { get; set; }

        public int ReorderLevel { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public ICollection<StockBatch> StockBatches { get; set; } = new List<StockBatch>();
    }
}