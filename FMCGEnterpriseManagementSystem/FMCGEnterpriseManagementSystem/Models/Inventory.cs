using FMCGEnterpriseManagementSystem.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Inventory
{
    [Key]
    public int InventoryId { get; set; }

    [Required]
    public int ProductId { get; set; }

    [ForeignKey("ProductId")]
    public Product Product { get; set; } = null!;

    public int QuantityOnHand { get; set; }

  
    public int QuantityInStock
    {
        get => QuantityOnHand;
        set => QuantityOnHand = value;
    }

    public int ReorderLevel { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public ICollection<StockBatch> StockBatches { get; set; } = new List<StockBatch>();
}