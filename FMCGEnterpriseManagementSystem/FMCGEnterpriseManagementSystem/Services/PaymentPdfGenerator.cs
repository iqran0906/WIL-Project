using FMCGEnterpriseManagementSystem.ViewModels;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FMCGEnterpriseManagementSystem.Services
{
    public static class PaymentPdfGenerator
    {
        public static byte[] Generate(PaymentViewModel payment)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Column(header =>
                    {
                        header.Item().Row(row =>
                        {
                            row.RelativeItem().Column(company =>
                            {
                                company.Item().Text("EXCLUSIVE DISTRIBUTORS (PTY) LTD").Bold().FontSize(18);
                                company.Item().Text("Durban, South Africa").FontSize(10);
                            });

                            row.ConstantItem(150).AlignRight().Text("PAYMENT RECEIPT").Bold().FontSize(20);
                        });

                        header.Item().PaddingTop(15).LineHorizontal(1);
                    });

                    page.Content().Column(column =>
                    {
                        column.Spacing(12);

                        column.Item().Text($"Payment ID: {payment.PaymentId}");
                        column.Item().Text($"Payment Date: {payment.PaymentDate:dd/MM/yyyy}");
                        column.Item().Text($"Payment Method: {payment.PaymentMethod}");
                        column.Item().Text($"Invoice Number: {payment.InvoiceNumber}");
                        column.Item().Text($"Customer: {(string.IsNullOrWhiteSpace(payment.CustomerName) ? "N/A" : payment.CustomerName)}");
                        column.Item().Text($"Invoice Total: R{payment.InvoiceTotal:0.00}");

                        column.Item().PaddingTop(10).BorderTop(1).PaddingTop(10).Row(row =>
                        {
                            row.RelativeItem().Text($"Amount Paid: R{payment.AmountPaid:0.00}").Bold();
                            row.RelativeItem().AlignCenter().Text($"Total Paid: R{payment.AmountAlreadyPaid:0.00}");
                            row.RelativeItem().AlignRight().Text($"Outstanding: R{payment.OutstandingBalance:0.00}");
                        });

                        column.Item().PaddingTop(15).AlignCenter().Text($"Status: {payment.Status}").Bold();
                    });

                    page.Footer().AlignCenter().Text("Exclusive Distributors (Pty) Ltd • Durban, South Africa").FontSize(8);
                });
            });

            return document.GeneratePdf();
        }
    }
}