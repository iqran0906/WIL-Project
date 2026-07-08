namespace FMCGEnterpriseManagementSystem.Models
{
    public class Supplier
    {
        public string SupplierID { get; set; }

        public string CompanyName { get; set; }

        public string ContactPerson { get; set; }

        public string ContactNumber { get; set; }

        public string Email { get; set; }

        public string PhysicalAddress { get; set; }

        public decimal CreditLimit { get; set; }

        public string CreditTerms { get; set; }

        public string VATNumber { get; set; }
    }
}