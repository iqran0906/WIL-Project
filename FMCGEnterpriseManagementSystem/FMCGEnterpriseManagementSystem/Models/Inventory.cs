using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Required for [ForeignKey]

namespace FMCGEnterpriseManagementSystem.Models
{
    public class Inventory
    {
        [Key]
        public int InventoryId { get; set; }

        [Required]
        public int ProductId { get; set; }

        public int QuantityOnHand { get; set; }

        public int ReorderLevel { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("ProductId")] // <--- Explicitly links the foreign key to avoid ProductId1
        public Product Product { get; set; } = null!;

        public ICollection<StockBatch> StockBatches { get; set; } = new List<StockBatch>();
    }
}