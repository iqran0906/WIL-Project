// Title: Model Validation in ASP.NET Core
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation

using System.ComponentModel.DataAnnotations;

namespace FMCGEnterpriseManagementSystem.ViewModels
{
    // View model used to transfer employee information
    // between the MVC controllers and views.
    public class EmployeeViewModel
    {
        // Unique identifier for the employee.
        public string? EmployeeID { get; set; }

        // Employee's internal employee number.
        // The StringLength attribute limits the value to 20 characters.
        [StringLength(20)]
        [Display(Name = "Employee Number")]
        public string? EmployeeNumber { get; set; }

        // Employee's first name.
        // Required prevents the field from being submitted empty.
        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        // Employee's surname.
        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        // Employee's email address.
        // EmailAddress validates that the supplied value follows
        // an accepted email address format.
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        // Employee's contact number.
        [Required]
        [Phone]
        [StringLength(20)]
        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set; } = string.Empty;

        // Employee's job title or position within the business.
        [Required]
        [StringLength(100)]
        [Display(Name = "Job Title")]
        public string JobTitle { get; set; } = string.Empty;

        // Date on which the employee started employment.
        // DataType.Date indicates that the value represents a date.
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Employment")]
        public DateTime DateOfEmployment { get; set; }

        // Identity user ID associated with the employee account.
        // Nullable because an employee may not yet have a login account.
        public string? UserId { get; set; }

        // Indicates whether the employee is currently active.
        // New view model instances default to active.
        public bool IsActive { get; set; } = true;

        // Next-of-kin details.

        // Unique identifier for the next-of-kin record.
        public string? NextOfKinID { get; set; }

        // Full name of the employee's next of kin.
        [Required]
        [StringLength(100)]
        [Display(Name = "Next of Kin Full Name")]
        public string NextOfKinFullName { get; set; } = string.Empty;

        // Describes the relationship between the employee
        // and the nominated next of kin.
        [Required]
        [StringLength(50)]
        [Display(Name = "Relationship")]
        public string NextOfKinRelationship { get; set; } = string.Empty;

        // Contact number for the next of kin.
        [Required]
        [Phone]
        [StringLength(20)]
        [Display(Name = "Next of Kin Contact Number")]
        public string NextOfKinContactNumber { get; set; } = string.Empty;

        // Optional email address for the next of kin.
        // EmailAddress validates the format when a value is supplied.
        [EmailAddress]
        [StringLength(100)]
        [Display(Name = "Next of Kin Email")]
        public string? NextOfKinEmail { get; set; }
    }
}
