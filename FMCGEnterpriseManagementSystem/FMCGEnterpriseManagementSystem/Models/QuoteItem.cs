/***************************************************************************************
*    Title: Quote Item Database Entity
*    Author: Naseeha27, Sayali-St10458649
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Models/QuoteItem.cs
***************************************************************************************/
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FMCGEnterpriseManagementSystem.Models
{
    public class QuoteItem
    {
        [Key]
        public int QuoteItemId { get; set; }

        [Required]
        public int QuoteId { get; set; }

        [ForeignKey("QuoteId")]
        public Quote? Quote { get; set; }
        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal DiscountPercent { get; set; }

        public string VatCategory { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal LineTotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal LineTotalExclVat { get; set; }
    }
}