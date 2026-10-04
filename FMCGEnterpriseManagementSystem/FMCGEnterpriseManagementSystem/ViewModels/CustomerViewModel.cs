//    Title: Model Validation in ASP.NET Core MVC
//    Author: Microsoft
//    Date: 30-08-2024
//    Code version: ASP.NET Core
//    Availability: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation

using System.ComponentModel.DataAnnotations;

namespace FMCGEnterpriseManagementSystem.ViewModels
{
    // ViewModel used to transfer customer data between the customer forms,
    // controllers, and service layer.
    public class CustomerViewModel
    {
        // Stores the unique identifier of the customer.
        public int CustomerId { get; set; }

        // Stores the customer's first name.
        // The Required attribute prevents the form from being submitted without a name.
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = string.Empty;

        // Stores the customer's surname.
        [Required(ErrorMessage = "Surname is required.")]
        public string Surname { get; set; } = string.Empty;

        // Stores the customer's identification number.
        [Required(ErrorMessage = "ID Number is required.")]
        public string IdNumber { get; set; } = string.Empty;

        // Stores the customer's telephone number.
        // This field is optional.
        public string? TelephoneNumber { get; set; } = string.Empty;

        // Stores the customer's mobile/cell number.
        [Required(ErrorMessage = "Cell number is required.")]
        public string CellNumber { get; set; } = string.Empty;

        // Stores the customer's email address.
        // EmailAddress validates that the supplied value follows an email format.
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }

        // Stores the customer's physical address.
        [Required(ErrorMessage = "Physical address is required.")]
        public string PhysicalAddress { get; set; } = string.Empty;

        // Stores the address where customer deliveries should be made.
        // This field is optional.
        public string? DeliveryAddress { get; set; }

        // Stores the group assigned to the customer.
        [Required(ErrorMessage = "Customer group is required.")]
        public string CustomerGroup { get; set; } = string.Empty;

        // Stores the payment terms agreed with the customer.
        [Required(ErrorMessage = "Payment terms are required.")]
        public string PaymentTerms { get; set; } = string.Empty;

        // Stores the payment method used by the customer.
        [Required(ErrorMessage = "Payment method is required.")]
        public string PaymentMethod { get; set; } = string.Empty;

        // Stores additional notes relating to the customer.
        // This field is optional.
        public string? Notes { get; set; } = string.Empty;

        // Stores the sales representative associated with the customer.
        // This field is optional.
        public string? SalesRep { get; set; } = string.Empty;

        // Stores the customer's VAT registration number when applicable.
        // This field is optional.
        public string? VATNumber { get; set; }

        // Stores the identifier of the sales representative assigned to the customer.
        // The nullable integer allows the customer to have no sales representative assigned.
        public int? SalesRepresentativeId { get; set; }
    }
}