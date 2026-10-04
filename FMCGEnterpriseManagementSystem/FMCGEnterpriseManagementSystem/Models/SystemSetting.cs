// Title: Entity Framework Core - Entity Properties
// Author: Microsoft
// Date: 12-01-2023
// Code version: Entity Framework Core
// Availability: https://learn.microsoft.com/en-us/ef/core/modeling/entity-properties
//
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FMCGEnterpriseManagementSystem.Models
{
    // Title: Entity Framework Core - Keys
    // Author: Microsoft
    // Date: 23-11-022
    // Code version: Entity Framework Core
    // Availability: https://learn.microsoft.com/en-us/ef/core/modeling/keys
    //


    // Stores company-wide configuration and business settings.
    // The system is designed to maintain one settings record with Id = 1.
    public class SystemSetting
    {
        // Fixed identifier used to maintain the single settings record.
        public const int SingletonId = 1;

        // Primary key for the system settings record.
        // Database generation is disabled because the ID is always 1.
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; } = SingletonId;

        // ---------- Company profile ----------

        // Stores the registered company name.
        [Required, StringLength(150)]
        public string CompanyName { get; set; } = string.Empty;

        // Stores the company's registration number.
        [StringLength(50)]
        public string? RegistrationNumber { get; set; }

        // Stores the company's VAT registration number.
        [StringLength(20)]
        public string? CompanyVatNumber { get; set; }

        // Stores the company's physical address.
        [Required, StringLength(300)]
        public string PhysicalAddress { get; set; } = string.Empty;

        // Stores the company's city.
        [StringLength(100)]
        public string? City { get; set; }

        // Stores the company's postal code.
        [StringLength(10)]
        public string? PostalCode { get; set; }

        // Stores the company's telephone number.
        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        // Stores the company's email address.
        [StringLength(150)]
        public string? Email { get; set; }

        // Stores the company's website address.
        [StringLength(200)]
        public string? Website { get; set; }

        // ---------- Banking details ----------

        // Stores the company's bank name for invoice payment information.
        [StringLength(100)]
        public string? BankName { get; set; }

        // Stores the name associated with the bank account.
        [StringLength(150)]
        public string? BankAccountName { get; set; }

        // Stores the company's bank account number.
        [StringLength(30)]
        public string? BankAccountNumber { get; set; }

        // Stores the bank branch code.
        [StringLength(20)]
        public string? BranchCode { get; set; }

        // Stores the reference customers should use when making payments.
        [StringLength(100)]
        public string? PaymentReference { get; set; }

        // Stores the email address used for proof-of-payment submissions.
        [StringLength(150)]
        public string? ProofOfPaymentEmail { get; set; }

        // ---------- Tax and document settings ----------

        // Stores the VAT percentage applied by the system.
        [Column(TypeName = "decimal(5,2)")]
        public decimal VatRatePercent { get; set; } = 15m;

        // Stores the prefix used when generating invoice numbers.
        [Required, StringLength(10)]
        public string InvoicePrefix { get; set; } = "ED";

        // Stores the prefix used when generating quote numbers.
        [Required, StringLength(10)]
        public string QuotePrefix { get; set; } = "ED";

        // Stores the number of days for which a quotation remains valid.
        public int QuoteValidityDays { get; set; } = 30;

        // Stores the default payment terms used on documents.
        [StringLength(100)]
        public string? DefaultPaymentTerms { get; set; } = "30 Days";

        // Stores the default message displayed at the bottom of invoices.
        [StringLength(500)]
        public string? InvoiceFooterNote { get; set; } = "Thanking you for your business!";

        // ---------- Notification settings ----------

        // Indicates whether email notifications are enabled.
        public bool EmailNotificationsEnabled { get; set; } = true;

        // Stores the email address used for system notifications.
        [StringLength(150)]
        public string? NotificationEmail { get; set; }

        // ---------- Audit information ----------

        // Records when the settings were last updated.
        public DateTime? UpdatedAt { get; set; }

        // Stores the user responsible for the latest settings update.
        [StringLength(256)]
        public string? UpdatedBy { get; set; }
    }
}