using System.ComponentModel.DataAnnotations;

namespace FMCGEnterpriseManagementSystem.ViewModels
{
    public class EmployeeViewModel
    {
        public string? EmployeeID { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Employee Number")]
        public string EmployeeNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(20)]
        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Job Title")]
        public string JobTitle { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Employment")]
        public DateTime DateOfEmployment { get; set; }

        public string? UserID { get; set; }

        public bool IsActive { get; set; } = true;

        // Next-of-kin details

        public string? NextOfKinID { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Next of Kin Full Name")]
        public string NextOfKinFullName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "Relationship")]
        public string NextOfKinRelationship { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(20)]
        [Display(Name = "Next of Kin Contact Number")]
        public string NextOfKinContactNumber { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(100)]
        [Display(Name = "Next of Kin Email")]
        public string? NextOfKinEmail { get; set; }
    }
}