// Title: ASP.NET Core MVC Views and ViewModels
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/aspnet/core/mvc/views/overview

using System.ComponentModel.DataAnnotations;

namespace FMCGEnterpriseManagementSystem.ViewModels
{
    // View model containing demand forecasting information
    // for an individual product.
    public class DemandForecastViewModel
    {
        // Unique identifier of the product.
        public int ProductId { get; set; }

        // Product identification and classification information.
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;

        // Current inventory quantity and the minimum stock level
        // at which a reorder may be required.
        public int CurrentStock { get; set; }
        public int ReorderLevel { get; set; }

        // Estimated quantity of the product expected to be required
        // during a typical month.
        public int EstimatedMonthlyDemand { get; set; }

        // Quantity recommended by the forecasting process
        // for the next reorder.
        public int RecommendedReorderQuantity { get; set; }

        // Describes the current forecasting/reorder condition
        // of the product.
        public string ForecastStatus { get; set; } = string.Empty;

        // Current selling or unit price of the product.
        public decimal UnitPrice { get; set; }

        // Calculates the estimated financial cost of the recommended
        // reorder quantity.
        public decimal EstimatedReorderCost =>
            RecommendedReorderQuantity * UnitPrice;
    }

    // View model used to display and filter demand forecast results.
    public class ForecastFilterViewModel
    {
        // Optional category selected by the user when filtering forecasts.
        public string? SelectedCategory { get; set; }

        // Optional status selected by the user for filtering results.
        public string? StatusFilter { get; set; }

        // Collection of forecast records displayed after filtering.
        public IEnumerable<DemandForecastViewModel> Forecasts { get; set; } = new List<DemandForecastViewModel>();

        // Collection of available product categories used by
        // the category filter.
        public IEnumerable<string> Categories { get; set; } = new List<string>();

        // Combined estimated cost of all projected reorder quantities.
        public decimal TotalProjectedReorderCost { get; set; }
    }

    // View model used when a user manually triggers a reorder.
    public class TriggerReorderViewModel
    {
        // Identifies the product that is being reordered.
        public int ProductId { get; set; }

        // Displays basic product information to the user.
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;

        // Quantity that should be ordered.
        // Required ensures that a value is supplied.
        // Range prevents zero or negative quantities from being submitted.
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Reorder quantity must be at least 1.")]
        [Display(Name = "Quantity to Order")]
        public int ReorderQuantity { get; set; }

        // Supplier selected for the reorder.
        // Required ensures that a supplier is selected.
        [Required(ErrorMessage = "Please select a supplier.")]
        [Display(Name = "Supplier")]
        public int SupplierId { get; set; }
    }
}
