//  Title: Model Validation in ASP.NET Core MVC
//    Author: Microsoft
//    Date: 2026
//    Code version: ASP.NET Core
//    Availability: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation

using System.ComponentModel.DataAnnotations;

namespace FMCGEnterpriseManagementSystem.ViewModels
{
    // ViewModel used to transfer inventory information between the inventory
    // forms, controllers, services, and views.
    public class InventoryViewModel
    {
        // Stores the unique identifier of the inventory record.
        public int InventoryId { get; set; }

        // Stores the identifier of the product associated with the inventory record.
        [Display(Name = "Product")]
        public int ProductId { get; set; }

        // Stores the product code displayed with the inventory information.
        public string? ProductCode { get; set; }

        // Stores the name of the associated product.
        public string? ProductName { get; set; }

        // Stores an optional description of the product.
        public string? Description { get; set; }

        // Stores the quantity currently available in stock.
        [Required(ErrorMessage = "Quantity on hand is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative.")]
        [Display(Name = "Quantity On Hand")]
        public int QuantityOnHand { get; set; }

        // Stores the stock quantity at which the product should be reordered.
        [Required(ErrorMessage = "Reorder level is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Reorder level cannot be negative.")]
        [Display(Name = "Reorder Level")]
        public int ReorderLevel { get; set; }

        // Stores the number of stock batches associated with the product.
        [Display(Name = "Total Batches")]
        public int BatchCount { get; set; }

        // --- Form Inputs & Relationships ---

        // Stores the selling price of the product.
        [Display(Name = "Selling Price")]
        public string SellingPrice { get; set; } = string.Empty;

        // Stores the product cost excluding VAT.
        [Display(Name = "Cost Ex VAT")]
        public decimal CostExVat { get; set; }

        // Stores the category assigned to the product.
        [Display(Name = "Category")]
        public string? CategoryName { get; set; }

        // Stores the identifier of the supplier associated with the inventory item.
        [Display(Name = "Supplier")]
        public int SupplierId { get; set; }

        // Stores the applicable VAT rate when available.
        public decimal? VatRate { get; set; }

        // Stores the expiry date of the inventory item or batch.
        public DateTime? ExpiryDate { get; set; }

        // Stores optional notes relating to the inventory item.
        public string? Notes { get; set; }

        // -----------------------------------

        // Calculates the current stock status based on the quantity available
        // and the configured reorder level.
        public string StockStatus => QuantityOnHand switch
        {
            // Indicates that there is no stock available.
            0 => "Out of Stock",

            // Indicates that stock has reached or fallen below the reorder level.
            var q when q <= ReorderLevel => "Low Stock",

            // Indicates that sufficient stock is currently available.
            _ => "In Stock"
        };
    }

    // ViewModel used when adjusting the quantity of an inventory item.
    public class AdjustStockViewModel
    {
        // Stores the identifier of the inventory record being adjusted.
        public int InventoryId { get; set; }

        // Stores the name of the product being adjusted.
        public string? ProductName { get; set; }

        // Stores the quantity currently available before the adjustment.
        public int CurrentQuantity { get; set; }

        // Stores the quantity that will be added or adjusted.
        [Required(ErrorMessage = "Adjustment quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Adjustment quantity must be at least 1.")]
        public int AdjustmentAmount { get; set; }

        // Stores whether the stock adjustment is an addition or another supported type.
        [Required]
        public string AdjustmentType { get; set; } = "Add";
    }
}