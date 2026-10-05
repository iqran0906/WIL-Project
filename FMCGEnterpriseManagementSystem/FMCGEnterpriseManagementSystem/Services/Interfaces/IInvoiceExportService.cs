// Title: Asynchronous programming with async and await
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    // Defines the operations used to generate invoice export documents.
    public interface IInvoiceExportService
    {
        // Generates a PDF document for the supplied invoice
        // and returns the generated file as a byte array.
        byte[] GenerateInvoicePdf(InvoiceViewModel invoice);
    }
}