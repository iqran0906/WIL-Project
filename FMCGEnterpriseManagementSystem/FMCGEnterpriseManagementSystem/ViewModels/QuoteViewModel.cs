
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace FMCGEnterpriseManagementSystem.ViewModels
{
    // Form model for creating a quote (the quote number is generated on save)
    public class QuoteViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "Quote date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Quote Date")]
        public DateTime QuoteDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Please select a customer.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a customer.")]
        [Display(Name = "Customer")]
        public int? CustomerId { get; set; }

        [Required(ErrorMessage = "Billing address is required. Select a customer with an address on file.")]
        [StringLength(500)]
        [Display(Name = "Billing Address")]
        public string BillingAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Payment terms are required.")]
        [StringLength(100)]
        [Display(Name = "Payment Terms")]
        public string PaymentTerms { get; set; } = string.Empty;

        public int? SalesRepresentativeId { get; set; }
        public string? SalesRepresentativeName { get; set; }

        [MinLength(1, ErrorMessage = "Add at least one item to the quote.")]
        public List<LineItemViewModel> Items { get; set; } = new();

        [ValidateNever]
        public List<FMCGEnterpriseManagementSystem.Models.Customer> AvailableCustomers { get; set; } = new();
        [ValidateNever]
        public List<ProductViewModel> AvailableProducts { get; set; } = new();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (QuoteDate.Date > DateTime.Today.AddYears(1))
            {
                yield return new ValidationResult(
                    "Quote date cannot be more than a year in the future.",
                    new[] { nameof(QuoteDate) });
            }
        }
    }
}
