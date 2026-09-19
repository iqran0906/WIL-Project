using System.ComponentModel.DataAnnotations;

namespace FMCGEnterpriseManagementSystem.Models
{
    public class Inventory
    {
        public int Id { get; set; }

        [Required]
        public string InventoryID { get; set; } = string.Empty;

        [Required]
        public string ProductID { get; set; } = string.Empty;

        public int QuantityOnHand { get; set; }
        public int ReorderLevel { get; set; }
    }
}