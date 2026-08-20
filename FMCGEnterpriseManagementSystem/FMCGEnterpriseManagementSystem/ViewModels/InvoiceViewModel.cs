using System.ComponentModel.DataAnnotations;


namespace FMCGEnterpriseManagementSystem.ViewModels
{

    
        public class InvoiceViewModel
        {
            public int Id { get; set; }

            [Required]
            public string InvoiceNumber { get; set; }

            [Required]
            public DateTime InvoiceDate { get; set; }

            [Required]
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
            public string BillingAddress { get; set; }

            public string PaymentTerms { get; set; }
            public int? SalesPersonId { get; set; }
            public string SalesPersonName { get; set; }

            public List<InvoiceItemViewModel> Items { get; set; } = new();

            public decimal Subtotal { get; set; }
            public decimal VatTotal { get; set; }
            public decimal Total { get; set; }
        public decimal AmountDue { get; set; }
        public string Status { get; set; }
        }

        public class InvoiceItemViewModel
        {
            public int Id { get; set; }

            [Required]
            public string ItemCode { get; set; }

            public string Description { get; set; }

            [Range(1, int.MaxValue)]
            public int Quantity { get; set; }

            [Range(0, double.MaxValue)]
            public decimal UnitPrice { get; set; }

            public decimal DiscountPercent { get; set; }
            public decimal VatPercent { get; set; }
            public decimal LineTotal { get; set; }
        }
    }