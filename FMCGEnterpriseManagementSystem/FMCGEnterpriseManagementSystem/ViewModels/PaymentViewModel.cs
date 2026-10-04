// Title: System.ComponentModel.DataAnnotations Namespace
// Author: Microsoft
// Date: 04-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations

using FMCGEnterpriseManagementSystem.Enums;
using System.ComponentModel.DataAnnotations;

namespace FMCGEnterpriseManagementSystem.ViewModels
{
    // View model used when recording and displaying customer invoice payments.
    // IValidatableObject allows additional validation rules to be defined in code.
    public class PaymentViewModel : IValidatableObject
    {
        // Payment methods accepted by the application.
        private static readonly string[] AllowedMethods = { "Cash", "Card", "EFT", "Bank Transfer" };

        // Unique identifier of the payment.
        public int PaymentId { get; set; }

        // Invoice selected for the payment.
        [Range(1, int.MaxValue, ErrorMessage = "Please choose the invoice being paid.")]
        public int InvoiceId { get; set; }

        // Display information for the selected invoice.
        public string? InvoiceNumber { get; set; }
        public string? CustomerName { get; set; }

        // Invoice amount and payment balance information used by the payment form.
        public decimal InvoiceTotal { get; set; }
        public decimal AmountAlreadyPaid { get; set; }
        public decimal OutstandingBalance { get; set; }

        // Date on which the payment is recorded.
        [Required(ErrorMessage = "Payment date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Payment Date")]
        public DateTime PaymentDate { get; set; } = DateTime.Today;

        // Payment method selected by the user.
        [Required(ErrorMessage = "Please select a payment method.")]
        [StringLength(50)]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; } = string.Empty;

        // Amount received from the customer.
        [Required(ErrorMessage = "Enter the amount paid.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Payment amount must be greater than zero.")]
        [Display(Name = "Amount Paid")]
        public decimal AmountPaid { get; set; }

        // Current status of the payment.
        public PaymentStatus Status { get; set; }

        // Indicates whether the payment is overdue.
        public bool IsOverdue { get; set; }

        // Previous payments associated with the invoice.
        public List<PaymentViewModel>? PaymentHistory { get; set; }

        // Performs validation that depends on more than basic property attributes.
        // The outstanding balance is re-checked against the database in PaymentService
        // to ensure that the payment is validated against the current persisted data.
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Prevent users from recording a payment using a future date.
            if (PaymentDate.Date > DateTime.Today)
            {
                yield return new ValidationResult(
                    "Payment date cannot be in the future.",
                    new[] { nameof(PaymentDate) });
            }

            // Ensure that the selected payment method is one of the supported methods.
            if (!string.IsNullOrWhiteSpace(PaymentMethod) && !AllowedMethods.Contains(PaymentMethod))
            {
                yield return new ValidationResult(
                    "Please select a valid payment method.",
                    new[] { nameof(PaymentMethod) });
            }
        }
    }
}
