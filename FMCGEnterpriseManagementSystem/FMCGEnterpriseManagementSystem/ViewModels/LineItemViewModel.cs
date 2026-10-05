// Title: Model Validation in ASP.NET Core MVC
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation

using System.ComponentModel.DataAnnotations;

namespace FMCGEnterpriseManagementSystem.ViewModels
{
    // Represents one product line included in a quote or invoice.
    // The DataAnnotations attributes define the validation rules
    // applied when the model is submitted through an MVC form.
    public class LineItemViewModel
    {
        // Constant representing the standard VAT category.
        // Using a constant prevents repeated hard-coded values.
        public const string VatStandard = "STANDARD";

        // Constant representing an item that does not have VAT applied.
        public const string VatNone = "[NONE]";

        // Identifies the product selected for this line item.
        // Range validation ensures that a valid product ID is selected.
        [Range(1, int.MaxValue, ErrorMessage = "Please select a product.")]
        [Display(Name = "Product")]
        public int ProductId { get; set; }

        // Optional product code displayed for the line item.
        public string? ItemCode { get; set; }

        // Optional product description.
        public string? Description { get; set; }

        // Quantity of the product being quoted or invoiced.
        // The allowed range prevents zero, negative, or excessively
        // large quantities from being submitted.
        [Range(1, 100000, ErrorMessage = "Quantity must be between 1 and 100 000.")]
        public int Quantity { get; set; } = 1;

        // Unit selling price of the product.
        // The decimal Range validation ensures the price is greater
        // than zero and does not exceed the configured maximum.
        [Range(typeof(decimal), "0.01", "10000000", ErrorMessage = "Unit price must be greater than zero.")]
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }

        // Percentage discount applied to this line item.
        // The value must remain between 0% and 100%.
        [Range(typeof(decimal), "0", "100", ErrorMessage = "Discount must be between 0% and 100%.")]
        [Display(Name = "Discount %")]
        public decimal DiscountPercent { get; set; }

        // VAT category applied to the line item.
        // Required ensures that a VAT category is selected.
        // RegularExpression restricts the value to the two supported
        // categories defined by the constants above.
        [Required(ErrorMessage = "Please choose a VAT category.")]
        [RegularExpression(@"^(STANDARD|\[NONE\])$", ErrorMessage = "Please choose a valid VAT category.")]
        [Display(Name = "VAT Category")]
        public string VatCategory { get; set; } = VatStandard;
    }

    // View model used by the shared _LineItemsEditor partial view.
    // It provides both the available products and the current
    // collection of line items that need to be displayed or edited.
    public class LineItemsEditorViewModel
    {
        // Collection of products available for selection
        // when creating or editing a line item.
        public IEnumerable<ProductViewModel> Products { get; set; } = Enumerable.Empty<ProductViewModel>();

        // Collection of line items currently associated with
        // the quote or invoice being edited.
        public IEnumerable<LineItemViewModel> Items { get; set; } = Enumerable.Empty<LineItemViewModel>();
    }
}
