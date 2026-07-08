namespace FMCGEnterpriseManagementSystem.Models
{
    public class Customer
    {
        public string CustomerID { get; set; }

        public string CompanyName { get; set; }

        public string ContactPerson { get; set; }

        public string ContactNumber { get; set; }

        public string Email { get; set; }

        public string PhysicalAddress { get; set; }

        public string DeliveryAddress { get; set; }

        public string CustomerGroup { get; set; }

        public string PaymentTerms { get; set; }

        public string PaymentMethod { get; set; }

        public string VATNumber { get; set; }
    }
}