//    Title: Model Validation in ASP.NET Core MVC
//    Author: Microsoft
//    Date: 2026
//    Code version: ASP.NET Core
//    Availability: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace FMCGEnterpriseManagementSystem.ViewModels
{
    // ViewModel used to transfer invoice information between the invoice
    // form, controller, service layer, and invoice views.
    public class InvoiceViewModel : IValidatableObject
    {
        // Stores the unique identifier of the invoice.
        public int InvoiceId { get; set; }

        // Generated when the invoice is saved, so not entered on the form.
        // Stores the generated invoice number.
        public string? InvoiceNumber { get; set; }

        // Stores the date associated with the invoice.
        [Required(ErrorMessage = "Invoice date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Invoice Date")]
        public DateTime InvoiceDate { get; set; }

        // Stores the optional quote associated with the invoice.
        public int? QuoteId { get; set; }

        // Stores the customer associated with the invoice.
        [Required(ErrorMessage = "Please select a customer.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a customer.")]
        [Display(Name = "Customer")]
        public int? CustomerId { get; set; }

        // Stores the customer's name for display purposes.
        public string? CustomerName { get; set; }

        // Stores the customer's business name when applicable.
        [StringLength(100, ErrorMessage = "Business name cannot be longer than 100 characters.")]
        [Display(Name = "Business Name")]
        public string? BusinessName { get; set; }

        // Provides the list of customers available for selection on the invoice form.
        // ValidateNever prevents MVC model validation from validating this list.
        [ValidateNever]
        public List<FMCGEnterpriseManagementSystem.Models.Customer> AvailableCustomers { get; set; } = new();

        // Provides the list of products available for selection on the invoice form.
        [ValidateNever]
        public List<ProductViewModel> AvailableProducts { get; set; } = new();

        // Stores the billing address used on the invoice.
        [Required(ErrorMessage = "Billing address is required. Select a customer with an address on file.")]
        [StringLength(500)]
        [Display(Name = "Billing Address")]
        public string BillingAddress { get; set; } = string.Empty;

        // Stores the payment terms associated with the invoice.
        [Required(ErrorMessage = "Payment terms are required.")]
        [StringLength(100)]
        [Display(Name = "Payment Terms")]
        public string PaymentTerms { get; set; } = string.Empty;

        // Stores the identifier of the sales representative assigned to the invoice.
        public int? SalesRepresentativeId { get; set; }

        // Stores the sales representative's name for display purposes.
        public string? SalesRepresentativeName { get; set; }

        // Stores the line items included in the invoice.
        // At least one item is required when creating an invoice.
        [MinLength(1, ErrorMessage = "Add at least one item to the invoice.")]
        public List<InvoiceItemViewModel> Items { get; set; } = new();

        // Stores the invoice subtotal before VAT.
        public decimal Subtotal { get; set; }

        // Stores the total VAT amount applied to the invoice.
        public decimal VatTotal { get; set; }

        // Stores the final invoice total.
        public decimal Total { get; set; }

        // Stores the amount remaining to be paid.
        public decimal AmountDue { get; set; }

        // Stores the current invoice status.
        public string? Status { get; set; }

        // Provides custom validation rules for the invoice ViewModel.
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Prevents an invoice date from being more than one year in the future.
            if (InvoiceDate.Date > DateTime.Today.AddYears(1))
            {
                yield return new ValidationResult(
                    "Invoice date cannot be more than a year in the future.",
                    new[] { nameof(InvoiceDate) });
            }
        }
    }

    // ViewModel representing an individual line item belonging to an invoice.
    public class InvoiceItemViewModel : LineItemViewModel
    {
        // Stores the unique identifier of the invoice line item.
        public int InvoiceItemId { get; set; }

        // Stores the calculated total for the individual line item.
        public decimal LineTotal { get; set; }
    }
}