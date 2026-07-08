namespace FMCGEnterpriseManagementSystem.Models
{
    public class Inventory
    {
        public string InventoryID { get; set; }

        public string ProductID { get; set; }

        public int QuantityOnHand { get; set; }

        public int ReorderLevel { get; set; }

        public DateTime ExpiryDate { get; set; }
    }
}