using System.ComponentModel.DataAnnotations;

namespace FMCGEnterpriseManagementSystem.ViewModels
{
    public class ProductViewModel
    {
        public string ProductId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a supplier.")]
        [Display(Name = "Supplier ID")]
        public string SupplierId { get; set; } = string.Empty;

        [Display(Name = "Product Code")]
        public string? ProductCode { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required.")]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cost (Excl. VAT) is required.")]
        [Range(0.00, double.MaxValue, ErrorMessage = "Cost cannot be negative.")]
        [Display(Name = "Cost (Excl. VAT)")]
        public decimal CostExVat { get; set; }

        [Display(Name = "Cost (Incl. VAT)")]
        public decimal CostIncVat { get; set; }

        [Required(ErrorMessage = "Selling price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Selling price must be greater than zero.")]
        [Display(Name = "Selling Price")]
        public decimal SellingPrice { get; set; }
    }
}