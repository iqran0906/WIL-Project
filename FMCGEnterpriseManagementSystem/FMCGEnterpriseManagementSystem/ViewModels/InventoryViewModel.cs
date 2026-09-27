using System.ComponentModel.DataAnnotations;

namespace FMCGEnterpriseManagementSystem.ViewModels
{
    public class InventoryViewModel
    {
        public int InventoryId { get; set; }

        [Required(ErrorMessage = "Please select a product.")]
        [Display(Name = "Product")]
        public int ProductId { get; set; }

        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }

        [Required(ErrorMessage = "Quantity on hand is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative.")]
        [Display(Name = "Quantity On Hand")]
        public int QuantityOnHand { get; set; }

        [Required(ErrorMessage = "Reorder level is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Reorder level cannot be negative.")]
        [Display(Name = "Reorder Level")]
        public int ReorderLevel { get; set; }

        [Display(Name = "Total Batches")]
        public int BatchCount { get; set; }

        public string StockStatus => QuantityOnHand switch
        {
            0 => "Out of Stock",
            var q when q <= ReorderLevel => "Low Stock",
            _ => "In Stock"
        };
    }

    public class AdjustStockViewModel
    {
        public int InventoryId { get; set; }
        public string? ProductName { get; set; }
        public int CurrentQuantity { get; set; }

        [Required(ErrorMessage = "Adjustment quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Adjustment quantity must be at least 1.")]
        public int AdjustmentAmount { get; set; }

        [Required]
        public string AdjustmentType { get; set; } = "Add"; // "Add" or "Deduct"
    }
}