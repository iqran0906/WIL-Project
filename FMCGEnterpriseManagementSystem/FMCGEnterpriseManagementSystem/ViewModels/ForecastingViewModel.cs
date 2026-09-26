using System.ComponentModel.DataAnnotations;

namespace FMCGEnterpriseManagementSystem.ViewModels
{
    public class DemandForecastViewModel
    {
        public int ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int CurrentStock { get; set; }
        public int ReorderLevel { get; set; }
        public int EstimatedMonthlyDemand { get; set; }
        public int RecommendedReorderQuantity { get; set; }
        public string ForecastStatus { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public decimal EstimatedReorderCost => RecommendedReorderQuantity * UnitPrice;
    }

    public class ForecastFilterViewModel
    {
        public string? SelectedCategory { get; set; }
        public string? StatusFilter { get; set; }
        public IEnumerable<DemandForecastViewModel> Forecasts { get; set; } = new List<DemandForecastViewModel>();
        public IEnumerable<string> Categories { get; set; } = new List<string>();
        public decimal TotalProjectedReorderCost { get; set; }
    }

    public class TriggerReorderViewModel
    {
        public int ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Reorder quantity must be at least 1.")]
        [Display(Name = "Quantity to Order")]
        public int ReorderQuantity { get; set; }

        [Required(ErrorMessage = "Please select a supplier.")]
        [Display(Name = "Supplier")]
        public int SupplierId { get; set; }
    }
}