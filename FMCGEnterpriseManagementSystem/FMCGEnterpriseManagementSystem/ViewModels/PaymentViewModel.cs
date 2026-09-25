using FMCGEnterpriseManagementSystem.Enums;
using System.ComponentModel.DataAnnotations;

namespace FMCGEnterpriseManagementSystem.ViewModels
{
    public class PaymentViewModel
    {
        public int PaymentId { get; set; }

        [Required]
        public int InvoiceId { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? CustomerName { get; set; }

        public decimal InvoiceTotal { get; set; }
        public decimal AmountAlreadyPaid { get; set; }
        public decimal OutstandingBalance { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Payment amount must be greater than zero.")]
        public decimal AmountPaid { get; set; }

        public PaymentStatus Status { get; set; }
        public bool IsOverdue { get; set; }

        public List<PaymentViewModel>? PaymentHistory { get; set; }
    }
}