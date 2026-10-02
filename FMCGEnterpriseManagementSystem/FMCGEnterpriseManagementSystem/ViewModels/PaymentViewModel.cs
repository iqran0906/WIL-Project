// Purpose: Data and validation rules for the payment pages and forms.
// Authors: Sayali-St10458649 (from git history)

using FMCGEnterpriseManagementSystem.Enums;
using System.ComponentModel.DataAnnotations;

namespace FMCGEnterpriseManagementSystem.ViewModels
{
    public class PaymentViewModel : IValidatableObject
    {
        private static readonly string[] AllowedMethods = { "Cash", "Card", "EFT", "Bank Transfer" };

        public int PaymentId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please choose the invoice being paid.")]
        public int InvoiceId { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? CustomerName { get; set; }

        public decimal InvoiceTotal { get; set; }
        public decimal AmountAlreadyPaid { get; set; }
        public decimal OutstandingBalance { get; set; }

        [Required(ErrorMessage = "Payment date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Payment Date")]
        public DateTime PaymentDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Please select a payment method.")]
        [StringLength(50)]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter the amount paid.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Payment amount must be greater than zero.")]
        [Display(Name = "Amount Paid")]
        public decimal AmountPaid { get; set; }

        public PaymentStatus Status { get; set; }
        public bool IsOverdue { get; set; }

        public List<PaymentViewModel>? PaymentHistory { get; set; }

        // The outstanding balance is re-checked against the database in PaymentService
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (PaymentDate.Date > DateTime.Today)
            {
                yield return new ValidationResult(
                    "Payment date cannot be in the future.",
                    new[] { nameof(PaymentDate) });
            }

            if (!string.IsNullOrWhiteSpace(PaymentMethod) && !AllowedMethods.Contains(PaymentMethod))
            {
                yield return new ValidationResult(
                    "Please select a valid payment method.",
                    new[] { nameof(PaymentMethod) });
            }
        }
    }
}
