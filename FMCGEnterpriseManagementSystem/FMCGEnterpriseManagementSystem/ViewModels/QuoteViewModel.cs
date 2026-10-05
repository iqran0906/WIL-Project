
// Title: ASP.NET Core MVC Model Validation
// Author: Microsoft
// Date: 04-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace FMCGEnterpriseManagementSystem.ViewModels
{
    // Form model for creating a quote (the quote number is generated on save).
    // IValidatableObject is used for validation rules that require custom logic.
    public class QuoteViewModel : IValidatableObject
    {
        // Date on which the quote is created.
        [Required(ErrorMessage = "Quote date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Quote Date")]
        public DateTime QuoteDate { get; set; } = DateTime.Today;

        // Customer selected for the quote.
        [Required(ErrorMessage = "Please select a customer.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a customer.")]
        [Display(Name = "Customer")]
        public int? CustomerId { get; set; }

        // Billing address used for the quote.
        [Required(ErrorMessage = "Billing address is required. Select a customer with an address on file.")]
        [StringLength(500)]
        [Display(Name = "Billing Address")]
        public string BillingAddress { get; set; } = string.Empty;

        // Payment terms agreed for the quote.
        [Required(ErrorMessage = "Payment terms are required.")]
        [StringLength(100)]
        [Display(Name = "Payment Terms")]
        public string PaymentTerms { get; set; } = string.Empty;

        // Optional sales representative responsible for the quote.
        public int? SalesRepresentativeId { get; set; }
        public string? SalesRepresentativeName { get; set; }

        // Products and quantities included in the quote.
        [MinLength(1, ErrorMessage = "Add at least one item to the quote.")]
        public List<LineItemViewModel> Items { get; set; } = new();

        // Customer data supplied to the view for selection.
        // ValidateNever prevents these display-only properties from being
        // included in normal model validation.
        [ValidateNever]
        public List<FMCGEnterpriseManagementSystem.Models.Customer> AvailableCustomers { get; set; } = new();

        // Product data supplied to the view for selecting quote items.
        [ValidateNever]
        public List<ProductViewModel> AvailableProducts { get; set; } = new();

        // Performs additional validation for the quote date.
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Prevent a quote from being created more than one year in the future.
            if (QuoteDate.Date > DateTime.Today.AddYears(1))
            {
                yield return new ValidationResult(
                    "Quote date cannot be more than a year in the future.",
                    new[] { nameof(QuoteDate) });
            }
        }
    }
}

