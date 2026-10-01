using FMCGEnterpriseManagementSystem.ViewModels;

namespace FMCGEnterpriseManagementSystem.Services.Interfaces
{
    public interface IInvoiceExportService
    {
        byte[] GenerateInvoicePdf(InvoiceViewModel invoice);
    }
}