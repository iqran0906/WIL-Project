
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

                    // =========================
                    // HEADER
                    // =========================
                    page.Header().Column(header =>
                    {
                        header.Item().Row(row =>
                        {
                            row.RelativeItem().Column(company =>
                            {
                                company.Item()
                                    .Text("EXCLUSIVE DISTRIBUTORS (PTY) LTD")
                                    .Bold()
                                    .FontSize(18);

                                company.Item()
                                    .Text("Durban, South Africa")
                                    .FontSize(10);

                                company.Item()
                                    .Text("Wholesale & Distribution")
                                    .FontSize(9);
                            });

                            row.ConstantItem(150)
                                .AlignRight()
                                .Text("INVOICE")
                                .Bold()
                                .FontSize(24);
                        });

                        header.Item()
                            .PaddingTop(15)
                            .LineHorizontal(1);
                    });

                    // =========================
                    // CONTENT
                    // =========================
                    page.Content().Column(column =>
                    {
                        column.Spacing(15);

                        // Invoice information
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Background("#F5F5F5").Padding(10).Column(left =>
                            {
                                left.Item()
                                    .Text("BILL TO")
                                    .Bold()
                                    .FontSize(10);

                                left.Item()
                                    .PaddingTop(5)
                                    .Text(invoice.CustomerName ?? "N/A")
                                    .Bold();

                                left.Item()
                                    .Text(invoice.BusinessName ?? "N/A");

                                left.Item()
                                    .Text(invoice.BillingAddress ?? "N/A");
                            });

                            row.ConstantItem(15);

                            row.ConstantItem(190).Background("#F5F5F5").Padding(10).Column(right =>
                            {
                                right.Item()
                                    .Text($"Invoice No: {invoice.InvoiceNumber}")
                                    .Bold();

                                right.Item()
                                    .Text($"Invoice Date: {invoice.InvoiceDate:dd/MM/yyyy}");

                                right.Item()
                                    .Text($"Payment Terms: {invoice.PaymentTerms ?? "N/A"}");

                             
                            });
                        });

                        // =========================
                        // ITEMS TABLE
                        // =========================
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(35);
                                columns.ConstantColumn(75);
                                columns.RelativeColumn();
                                columns.ConstantColumn(65);
                                columns.ConstantColumn(50);
                                columns.ConstantColumn(80);
                                columns.ConstantColumn(55);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(HeaderCellStyle).Text("Qty");
                                header.Cell().Element(HeaderCellStyle).Text("Product Code");
                                header.Cell().Element(HeaderCellStyle).Text("Description");
                                header.Cell().Element(HeaderCellStyle).AlignRight().Text("Unit Price");
                                header.Cell().Element(HeaderCellStyle).AlignRight().Text("Disc %");
                                header.Cell().Element(HeaderCellStyle).AlignRight().Text("Total");
                                header.Cell().Element(HeaderCellStyle).AlignRight().Text("VAT");
                            });

                            foreach (var item in invoice.Items)
                            {
                                var lineExVat = item.Quantity *
                                                item.UnitPrice *
                                                (1 - item.DiscountPercent / 100m);

                                var lineVat = item.VatCategory == "[NONE]"
                                    ? 0
                                    : lineExVat * 0.15m;

                                table.Cell().Element(DataCellStyle)
                                    .Text(item.Quantity.ToString());

                                table.Cell().Element(DataCellStyle)
                                    .Text(item.ItemCode ?? "");

                                table.Cell().Element(DataCellStyle)
                                    .Text(item.Description ?? "");

                                table.Cell().Element(DataCellStyle)
                                    .AlignRight()
                                    .Text($"R{item.UnitPrice:0.00}");

                                table.Cell().Element(DataCellStyle)
                                    .AlignRight()
                                    .Text($"{item.DiscountPercent:0.00}%");

                                table.Cell().Element(DataCellStyle)
                                    .AlignRight()
                                    .Text($"R{lineExVat:0.00}");

                                table.Cell().Element(DataCellStyle)
                                    .AlignRight()
                                    .Text($"R{lineVat:0.00}");
                            }
                        });

                        // =========================
                        // TOTALS
                        // =========================
                        column.Item()
                            .AlignRight()
                            .Width(250)
                            .Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.ConstantColumn(100);
                                });

                                table.Cell().Element(TotalLabelStyle)
                                    .Text("Subtotal");

                                table.Cell().Element(TotalValueStyle)
                                    .Text($"R{invoice.Subtotal:0.00}");

                                table.Cell().Element(TotalLabelStyle)
                                    .Text("VAT");

                                table.Cell().Element(TotalValueStyle)
                                    .Text($"R{invoice.VatTotal:0.00}");

                                table.Cell().Element(TotalLabelStyle)
                                    .Text("TOTAL")
                                    .Bold();

                                table.Cell().Element(TotalValueStyle)
                                    .Text($"R{invoice.Total:0.00}")
                                    .Bold()
                                    .FontSize(11);

                                table.Cell().Element(TotalLabelStyle)
                                    .Text("AMOUNT DUE")
                                    .Bold();

                                table.Cell().Element(TotalValueStyle)
                                    .Text($"R{invoice.AmountDue:0.00}")
                                    .Bold()
                                    .FontSize(11);
                            });

                        // =========================
                        // PAYMENT INFORMATION
                        // =========================
                        column.Item()
                            .PaddingTop(20)
                            .BorderTop(1)
                            .PaddingTop(10)
                            .Column(payment =>
                            {
                                payment.Item()
                                    .Text("PAYMENT INFORMATION")
                                    .Bold()
                                    .FontSize(10);

                                payment.Item()
                                    .PaddingTop(5)
                                    .Text($"Payment Terms: {invoice.PaymentTerms ?? "N/A"}");

                                payment.Item()
                                    .Text("Please use the invoice number as your payment reference.");
                            });

                        // Thank you message
                        column.Item()
                            .PaddingTop(15)
                            .AlignCenter()
                            .Text("Thank you for your business.")
                            .Italic()
                            .FontSize(10);
                    });

                    // =========================
                    // FOOTER
                    // =========================
                    page.Footer()
                        .AlignCenter()
                        .Column(footer =>
                        {
                            footer.Item()
                                .LineHorizontal(1);

                            footer.Item()
                                .PaddingTop(5)
                                .Text("Exclusive Distributors (Pty) Ltd • Durban, South Africa")
                                .FontSize(8);

                            footer.Item()
                                .Text("Invoice generated electronically")
                                .FontSize(8);
                        });
                });
            });

            return document.GeneratePdf();
        }

        // =========================
        // TABLE STYLES
        // =========================

        private static IContainer HeaderCellStyle(IContainer container)
        {
            return container
                .Background("#E8E8E8")
                .Border(1)
                .Padding(5)
                .DefaultTextStyle(x => x.Bold().FontSize(8));
        }

        private static IContainer DataCellStyle(IContainer container)
        {
            return container
                .BorderBottom(1)
                .Padding(5)
                .DefaultTextStyle(x => x.FontSize(8));
        }

        private static IContainer TotalLabelStyle(IContainer container)
        {
            return container
                .BorderBottom(1)
                .Padding(5)
                .AlignRight();
        }

        private static IContainer TotalValueStyle(IContainer container)
        {
            return container
                .BorderBottom(1)
                .Padding(5)
                .AlignRight();
        }
    }
}

