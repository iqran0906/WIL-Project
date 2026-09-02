using System.ComponentModel.DataAnnotations;

namespace FMCGEnterpriseManagementSystem.Models
{
    public class StockBatch
    {
        [Key]
        public int StockBatchId { get; set; }

        [Required]
        public int InventoryId { get; set; }

        [Required]
        [StringLength(50)]
        public string BatchNumber { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public DateTime ReceivedDate { get; set; }

        public DateTime ExpiryDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public Inventory Inventory { get; set; } = null!;
    }
}