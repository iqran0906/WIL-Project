// Purpose: Company-wide settings (company profile, banking details, VAT, numbering) - one row.
// Authors: ST10068525 (new file, not yet committed)

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FMCGEnterpriseManagementSystem.Models
{
    // Company-wide settings. There is only ever one row (Id = 1).
    public class SystemSetting
    {
        public const int SingletonId = 1;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; } = SingletonId;

        // ---------- Company profile (blank until an administrator fills it in) ----------
        [Required, StringLength(150)]
        public string CompanyName { get; set; } = string.Empty;

        [StringLength(50)]
        public string? RegistrationNumber { get; set; }

        [StringLength(20)]
        public string? CompanyVatNumber { get; set; }

        [Required, StringLength(300)]
        public string PhysicalAddress { get; set; } = string.Empty;

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(10)]
        public string? PostalCode { get; set; }

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [StringLength(150)]
        public string? Email { get; set; }

        [StringLength(200)]
        public string? Website { get; set; }

        // ---------- Banking details (printed on invoices) ----------
        [StringLength(100)]
        public string? BankName { get; set; }

        [StringLength(150)]
        public string? BankAccountName { get; set; }

        [StringLength(30)]
        public string? BankAccountNumber { get; set; }

        [StringLength(20)]
        public string? BranchCode { get; set; }

        [StringLength(100)]
        public string? PaymentReference { get; set; }

        [StringLength(150)]
        public string? ProofOfPaymentEmail { get; set; }

        // ---------- Tax & documents (not shown on the Settings page; fixed defaults) ----------
        [Column(TypeName = "decimal(5,2)")]
        public decimal VatRatePercent { get; set; } = 15m;

        [Required, StringLength(10)]
        public string InvoicePrefix { get; set; } = "ED";

        [Required, StringLength(10)]
        public string QuotePrefix { get; set; } = "ED";

        public int QuoteValidityDays { get; set; } = 30;

        [StringLength(100)]
        public string? DefaultPaymentTerms { get; set; } = "30 Days";

        [StringLength(500)]
        public string? InvoiceFooterNote { get; set; } = "Thanking you for your business!";

        // ---------- Notifications (not shown on the Settings page; fixed defaults) ----------
        public bool EmailNotificationsEnabled { get; set; } = true;

        [StringLength(150)]
        public string? NotificationEmail { get; set; }

        // ---------- Audit ----------
        public DateTime? UpdatedAt { get; set; }

        [StringLength(256)]
        public string? UpdatedBy { get; set; }
    }
}
