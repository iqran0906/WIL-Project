

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace FMCGEnterpriseManagementSystem.ViewModels
{
    public class InvoiceViewModel : IValidatableObject
    {
        public int InvoiceId { get; set; }

        // Generated when the invoice is saved, so not entered on the form
        public string? InvoiceNumber { get; set; }

        [Required(ErrorMessage = "Invoice date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Invoice Date")]
        public DateTime InvoiceDate { get; set; }

        public int? QuoteId { get; set; }

        [Required(ErrorMessage = "Please select a customer.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a customer.")]
        [Display(Name = "Customer")]
        public int? CustomerId { get; set; }
        public string? CustomerName { get; set; }

        [StringLength(100, ErrorMessage = "Business name cannot be longer than 100 characters.")]
        [Display(Name = "Business Name")]
        public string? BusinessName { get; set; }

        [ValidateNever]
        public List<FMCGEnterpriseManagementSystem.Models.Customer> AvailableCustomers { get; set; } = new();
        [ValidateNever]
        public List<ProductViewModel> AvailableProducts { get; set; } = new();

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

        [MinLength(1, ErrorMessage = "Add at least one item to the invoice.")]
        public List<InvoiceItemViewModel> Items { get; set; } = new();

        public decimal Subtotal { get; set; }
        public decimal VatTotal { get; set; }
        public decimal Total { get; set; }
        public decimal AmountDue { get; set; }

        public string? Status { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (InvoiceDate.Date > DateTime.Today.AddYears(1))
            {
                yield return new ValidationResult(
                    "Invoice date cannot be more than a year in the future.",
                    new[] { nameof(InvoiceDate) });
            }
        }
    }

    public class InvoiceItemViewModel : LineItemViewModel
    {
        public int InvoiceItemId { get; set; }
        public decimal LineTotal { get; set; }
    }
}
