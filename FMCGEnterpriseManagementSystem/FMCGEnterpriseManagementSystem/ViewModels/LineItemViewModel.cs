

using System.ComponentModel.DataAnnotations;

namespace FMCGEnterpriseManagementSystem.ViewModels
{
    // One product line on a quote or invoice, with its validation rules
    public class LineItemViewModel
    {
        public const string VatStandard = "STANDARD";
        public const string VatNone = "[NONE]";

        [Range(1, int.MaxValue, ErrorMessage = "Please select a product.")]
        [Display(Name = "Product")]
        public int ProductId { get; set; }

        public string? ItemCode { get; set; }
        public string? Description { get; set; }

        [Range(1, 100000, ErrorMessage = "Quantity must be between 1 and 100 000.")]
        public int Quantity { get; set; } = 1;

        [Range(typeof(decimal), "0.01", "10000000", ErrorMessage = "Unit price must be greater than zero.")]
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }

        [Range(typeof(decimal), "0", "100", ErrorMessage = "Discount must be between 0% and 100%.")]
        [Display(Name = "Discount %")]
        public decimal DiscountPercent { get; set; }

        [Required(ErrorMessage = "Please choose a VAT category.")]
        [RegularExpression(@"^(STANDARD|\[NONE\])$", ErrorMessage = "Please choose a valid VAT category.")]
        [Display(Name = "VAT Category")]
        public string VatCategory { get; set; } = VatStandard;
    }

    // Data for the shared _LineItemsEditor partial
    public class LineItemsEditorViewModel
    {
        public IEnumerable<ProductViewModel> Products { get; set; } = Enumerable.Empty<ProductViewModel>();
        public IEnumerable<LineItemViewModel> Items { get; set; } = Enumerable.Empty<LineItemViewModel>();
    }
}
