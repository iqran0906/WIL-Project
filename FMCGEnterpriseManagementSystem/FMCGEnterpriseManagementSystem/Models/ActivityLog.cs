// Title: Activity log entity for recording user actions in the system.
// Authors: ImranHussain78612
// Date: 12-01-2023
// Code version: ASP.NET Core 10.0
// Availability: https://learn.microsoft.com/en-us/ef/core/modeling/entity-properties

using System.ComponentModel.DataAnnotations;

namespace FMCGEnterpriseManagementSystem.Models
{
    // Stores a record of an action performed by a user in the system.
    public class ActivityLog
    {
        [Key]
        public int ActivityLogId { get; set; }

        // Stores the Identity user ID of the person who performed the action.
        [Required, StringLength(450)]
        public string UserId { get; set; } = string.Empty;

        // Stores the user's name for easier identification in activity records.
        [StringLength(256)]
        public string? UserName { get; set; }

        // Identifies the area of the system where the action occurred.
        [Required, StringLength(50)]
        public string Category { get; set; } = string.Empty;

        // Provides a short description of the action performed.
        [Required, StringLength(200)]
        public string Description { get; set; } = string.Empty;

        // Stores additional information about the activity when required.
        [StringLength(500)]
        public string? Details { get; set; }

        // Records when the activity occurred using UTC time.
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}