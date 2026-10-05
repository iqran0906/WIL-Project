// Title: Entity Framework Core - Entity Properties
// Author: Microsoft
// Date: 12-01-2023
// Code version: Entity Framework Core
// Availability: https://learn.microsoft.com/en-us/ef/core/modeling/entity-properties
//

using System.ComponentModel.DataAnnotations;

namespace FMCGEnterpriseManagementSystem.Models
{
    // Represents a sales representative and their sales-related information.
    public class SalesRepresentative
    {
        // Primary key that uniquely identifies the sales representative.
        [Key]
        public int SalesRepresentativeId { get; set; }

        // Identifies the employee assigned to the sales representative role.
        [Required]
        [StringLength(20)]
        public string EmployeeID { get; set; } = string.Empty;

        // Stores the unique code used to identify the sales representative.
        [Required]
        [StringLength(20)]
        public string SalesRepCode { get; set; } = string.Empty;

        // Stores the geographical or customer area assigned to the representative.
        [StringLength(100)]
        public string? Area { get; set; }

        // Stores the representative's salary.
        public decimal Salary { get; set; }

        // Stores the commission percentage earned from sales.
        public decimal CommissionRate { get; set; }

        // Stores the sales target assigned to the representative.
        public decimal SalesTarget { get; set; }

        // Indicates whether the sales representative is currently active.
        public bool IsActive { get; set; } = true;

        // Records when the sales representative record was created.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Records the most recent update time, if applicable.
        public DateTime? UpdatedAt { get; set; }

        // Navigation property linking the representative to an employee.
        public Employee Employee { get; set; } = null!;

        // Collection of customers assigned to the representative.
        public ICollection<Customer> Customers { get; set; } = new List<Customer>();

        // Collection of quotations managed by the representative.
        public ICollection<Quote> Quotes { get; set; } = new List<Quote>();
    }
}