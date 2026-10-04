// Title: ASP.NET Core MVC View Models
// Author: Microsoft
// Date: 04-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/aspnet/core/mvc/views/overview

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace FMCGEnterpriseManagementSystem.ViewModels.Reports
{
    // View model containing the information required for quote reports.
    public class QuoteReportViewModel
    {
        // Unique identifier of the quote.
        public int QuoteId { get; set; }

        // Human-readable quote reference number.
        public string QuoteNumber { get; set; } = string.Empty;

        // Date on which the quote was created.
        public DateTime QuoteDate { get; set; }

        // Customer associated with the quote.
        public string CustomerName { get; set; } = string.Empty;

        // Current status of the quote.
        public string Status { get; set; } = string.Empty;

        // Quote amount before the final total is calculated.
        public decimal Subtotal { get; set; }

        // Final quote amount.
        public decimal Total { get; set; }
    }
}


