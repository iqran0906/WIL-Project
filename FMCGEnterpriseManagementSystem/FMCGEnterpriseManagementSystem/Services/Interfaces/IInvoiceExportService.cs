/***************************************************************************************
*    Title: Invoice Export Service Interface
*    Author: Sayali-St10458649
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Services/Interfaces/IInvoiceExportService.cs
***************************************************************************************/
using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    public interface IInvoiceExportService
    {
        byte[] GenerateInvoicePdf(InvoiceViewModel invoice);
    }
}