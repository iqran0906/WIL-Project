using System.ComponentModel.DataAnnotations;

namespace FMCGEnterpriseManagementSystem.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal CostExVat { get; set; }
        public decimal CostIncVat { get; set; }
        public decimal SellingPrice { get; set; }
    }
}