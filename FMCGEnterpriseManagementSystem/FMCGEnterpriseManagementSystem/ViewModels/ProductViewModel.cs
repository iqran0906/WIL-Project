// Title: ASP.NET Core MVC Model Validation
// Author: Microsoft
// Date: 04-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace FMCGEnterpriseManagementSystem.ViewModels
{
    // View model used to transfer product information between the
    // controller, views and application services.
    public class ProductViewModel
    {
        // Unique identifier of the product.
        public int ProductId { get; set; }

        // Supplier associated with the product.
        [Required(ErrorMessage = "Please select a supplier.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid supplier.")]
        [Display(Name = "Supplier")]
        public int SupplierId { get; set; }

        // Display name of the supplier.
        [Display(Name = "Supplier")]
        public string SupplierName { get; set; } = string.Empty;

        // Optional product code used to identify the product.
        [Display(Name = "Product Code")]
        public string? ProductCode { get; set; }

        // Product name displayed to users.
        [Required(ErrorMessage = "Product name is required.")]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; } = string.Empty;

        // Optional description of the product.
        public string? Description { get; set; }

        // Product category.
        [Required(ErrorMessage = "Category is required.")]
        public string Category { get; set; } = string.Empty;

        // Product purchase cost excluding VAT.
        [Required(ErrorMessage = "Cost (Excl. VAT) is required.")]
        [Range(0.00, double.MaxValue, ErrorMessage = "Cost cannot be negative.")]
        [Display(Name = "Cost (Excl. VAT)")]
        public decimal CostExVat { get; set; }

        // Product purchase cost including VAT.
        [Display(Name = "Cost (Incl. VAT)")]
        public decimal CostIncVat { get; set; }

        // Price at which the product is sold to customers.
        [Required(ErrorMessage = "Selling price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Selling price must be greater than zero.")]
        [Display(Name = "Selling Price")]
        public decimal SellingPrice { get; set; }

        // Indicates whether the product is currently available for use.
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        // Date and time when the product was created.
        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }

        // Date and time when the product was last updated.
        [Display(Name = "Updated At")]
        public DateTime? UpdatedAt { get; set; }
    }
}


