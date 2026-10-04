// Title: Entity Framework Core - Entity Properties
// Author: Microsoft
// Date: 12-01-2023
// Code version: Entity Framework Core
// Availability: https://learn.microsoft.com/en-us/ef/core/modeling/entity-properties
//
using System.ComponentModel.DataAnnotations;

namespace FMCGEnterpriseManagementSystem.Models
{
    // Represents an employee's next-of-kin contact details.
    public class NextOfKin
    {
        // Primary key that uniquely identifies the next-of-kin record.
        [Key]
        [StringLength(20)]
        public string NextOfKinID { get; set; } = string.Empty;

        // Identifies the employee associated with the next-of-kin record.
        [Required]
        [StringLength(20)]
        public string EmployeeID { get; set; } = string.Empty;

        // Stores the full name of the next-of-kin contact.
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        // Stores the relationship between the contact and employee.
        [Required]
        [StringLength(50)]
        public string Relationship { get; set; } = string.Empty;

        // Stores the contact's telephone number.
        [Required]
        [Phone]
        [StringLength(20)]
        public string ContactNumber { get; set; } = string.Empty;

        // Stores an optional email address for the contact.
        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        // Navigation property linking the next-of-kin record to an employee.
        public Employee Employee { get; set; } = null!;
    }
}