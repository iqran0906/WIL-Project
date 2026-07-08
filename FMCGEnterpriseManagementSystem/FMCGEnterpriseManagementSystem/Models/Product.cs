namespace FMCGEnterpriseManagementSystem.Models
{
    public class Product
    {
        public string ProductID { get; set; }

        public string SupplierID { get; set; }

        public string ProductCode { get; set; }

        public string ProductName { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public decimal CostExVat { get; set; }

        public decimal CostIncVat { get; set; }

        public decimal SellingPrice { get; set; }
    }
}