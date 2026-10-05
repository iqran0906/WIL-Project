// Title: Employee entity for storing employee and employment information.
// Author: Microsoft
// Date: 12-01-2023
// Code version: ASP.NET Core 10.0
// Availability: https://learn.microsoft.com/en-us/ef/core/modeling/entity-properties

using System.ComponentModel.DataAnnotations;

namespace FMCGEnterpriseManagementSystem.Models
{
    // Represents an employee and their employment, contact and account information.
    public class Employee
    {
        // Unique identifier for the employee.
        [Key]
        [StringLength(20)]
        public string EmployeeID { get; set; } = string.Empty;

        // Internal employee number used by the business.
        [Required]
        [StringLength(20)]
        public string EmployeeNumber { get; set; } = string.Empty;

        // Employee's first name.
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        // Employee's last name.
        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        // Employee's email address.
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        // Employee's contact telephone number.
        [Required]
        [Phone]
        [StringLength(20)]
        public string ContactNumber { get; set; } = string.Empty;

        // Employee's position or job title.
        [Required]
        [StringLength(100)]
        public string JobTitle { get; set; } = string.Empty;

        // Date on which the employee joined the business.
        [Required]
        public DateTime DateOfEmployment { get; set; }

        // Indicates whether the employee is currently active.
        public bool IsActive { get; set; } = true;

        // Optional link between the employee and their application user account.
        public string? UserId { get; set; }

        // Navigation property for the employee's application user.
        public User? User { get; set; }

        // Records when the employee record was created.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Records when the employee record was last updated.
        public DateTime? UpdatedAt { get; set; }

        // Navigation property containing the employee's next-of-kin information.
        public NextOfKin? NextOfKin { get; set; }
    }
}