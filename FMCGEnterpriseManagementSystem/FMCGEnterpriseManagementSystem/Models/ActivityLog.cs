
using System.ComponentModel.DataAnnotations;

namespace FMCGEnterpriseManagementSystem.Models
{
    // One thing a user did in the system (created an invoice, logged in, exported a report, ...)
    public class ActivityLog
    {
        [Key]
        public int ActivityLogId { get; set; }

        // Identity user who did it
        [Required, StringLength(450)]
        public string UserId { get; set; } = string.Empty;

        [StringLength(256)]
        public string? UserName { get; set; }

        // Area of the system, e.g. "Invoices", "Suppliers", "Account"
        [Required, StringLength(50)]
        public string Category { get; set; } = string.Empty;

        // Short description, e.g. "Created an invoice"
        [Required, StringLength(200)]
        public string Description { get; set; } = string.Empty;

        // Extra detail, e.g. the page's success message "Quote QT00003 created successfully."
        [StringLength(500)]
        public string? Details { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
