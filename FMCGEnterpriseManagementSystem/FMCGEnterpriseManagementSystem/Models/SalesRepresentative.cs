using System.ComponentModel.DataAnnotations;

namespace FMCGEnterpriseManagementSystem.Models
{
    public class SalesRepresentative
    {
        public int SalesRepId { get; set; } 
        public string SalesRepCode { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string Territory { get; set; } = string.Empty;
        public decimal CommissionRate { get; set; }
    }
}