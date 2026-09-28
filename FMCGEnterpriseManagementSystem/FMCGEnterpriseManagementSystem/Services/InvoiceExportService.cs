using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FMCGEnterpriseManagementSystem.Services
{
    public class InvoiceExportService : IInvoiceExportService
    {
        public byte[] GenerateInvoicePdf(InvoiceViewModel invoice)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Column(column =>
                    {
                        column.Item().Text("Exclusive Distributors (pty) LTD")
                            .Bold()
                            .FontSize(18);

                        column.Item().Text("Durban")
                            .FontSize(10);

                        column.Item().PaddingTop(15).LineHorizontal(1);
                    });

                    page.Content().Column(column =>
                    {
                        column.Spacing(10);

                        column.Item().Text("INVOICE")
                            .Bold()
                            .FontSize(20);

                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Column(left =>
                            {
                                left.Item().Text($"Customer: {invoice.CustomerName ?? "N/A"}");
                                left.Item().Text($"Business: {invoice.BusinessName ?? "N/A"}");
                                left.Item().Text($"Billing Address: {invoice.BillingAddress ?? "N/A"}");
                            });

                            row.ConstantItem(180).Column(right =>
                            {
                                right.Item().Text($"Invoice No: {invoice.InvoiceNumber}");
                                right.Item().Text($"Date: {invoice.InvoiceDate:yyyy-MM-dd}");
                                right.Item().Text($"Payment Terms: {invoice.PaymentTerms ?? "N/A"}");
                            });
                        });

                        column.Item().PaddingTop(15).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(40);
                                columns.ConstantColumn(80);
                                columns.RelativeColumn();
                                columns.ConstantColumn(70);
                                columns.ConstantColumn(50);
                                columns.ConstantColumn(85);
                                columns.ConstantColumn(60);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Qty");
                                header.Cell().Element(CellStyle).Text("Product Code");
                                header.Cell().Element(CellStyle).Text("Description");
                                header.Cell().Element(CellStyle).Text("Unit Price");
                                header.Cell().Element(CellStyle).Text("Disc %");
                                header.Cell().Element(CellStyle).Text("Total");
                                header.Cell().Element(CellStyle).Text("VAT");
                            });

                            foreach (var item in invoice.Items)
                            {
                                var lineExVat = item.Quantity *
                                                item.UnitPrice *
                                                (1 - item.DiscountPercent / 100m);

                                var lineVat = item.VatCategory == "[NONE]"
                                    ? 0
                                    : lineExVat * 0.15m;

                                table.Cell().Element(CellStyle).Text(item.Quantity.ToString());
                                table.Cell().Element(CellStyle).Text(item.ItemCode ?? "");
                                table.Cell().Element(CellStyle).Text(item.Description ?? "");
                                table.Cell().Element(CellStyle).Text($"R{item.UnitPrice:0.00}");
                                table.Cell().Element(CellStyle).Text($"{item.DiscountPercent:0.00}%");
                                table.Cell().Element(CellStyle).Text($"R{lineExVat:0.00}");
                                table.Cell().Element(CellStyle).Text($"R{lineVat:0.00}");
                            }
                        });

                        column.Item().PaddingTop(15).AlignRight().Column(totals =>
                        {
                            totals.Item().Text($"Subtotal: R{invoice.Subtotal:0.00}");
                            totals.Item().Text($"VAT: R{invoice.VatTotal:0.00}");
                            totals.Item().Text($"Total: R{invoice.Total:0.00}")
                                .Bold()
                                .FontSize(12);
                            totals.Item().Text($"Amount Due: R{invoice.AmountDue:0.00}")
                                .Bold();
                        });

                        column.Item().PaddingTop(20).Text($"Status: {invoice.Status}");
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text("Exclusive Distributors (pty) LTD");
                });
            });

            return document.GeneratePdf();
        }

        private static IContainer CellStyle(IContainer container)
        {
            return container
                .Border(1)
                .Padding(5);
        }
    }
}