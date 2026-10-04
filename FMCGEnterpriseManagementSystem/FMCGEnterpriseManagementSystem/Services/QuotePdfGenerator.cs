
using FMCGEnterpriseManagementSystem.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FMCGEnterpriseManagementSystem.Services
{
    public static class QuotePdfGenerator
    {
        public static byte[] Generate(Quote quote)
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
                                .Text("QUOTE")
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

                        // Quote information
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
                                    .Text($"{quote.Customer?.Name} {quote.Customer?.Surname}".Trim())
                                    .Bold();

                                left.Item()
                                    .Text(quote.BillingAddress ?? "N/A");
                            });

                            row.ConstantItem(15);

                            row.ConstantItem(190).Background("#F5F5F5").Padding(10).Column(right =>
                            {
                                right.Item()
                                    .Text($"Quote No: {quote.QuoteNumber}")
                                    .Bold();

                                right.Item()
                                    .Text($"Quote Date: {quote.QuoteDate:dd/MM/yyyy}");

                                right.Item()
                                    .Text($"Payment Terms: {quote.PaymentTerms ?? "N/A"}");

                                right.Item()
                                    .Text($"Sales Person: {quote.SalesRepresentative?.Employee?.FirstName} {quote.SalesRepresentative?.Employee?.LastName}");
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
                                header.Cell().Element(HeaderCellStyle).AlignRight().Text("VAT");
                                header.Cell().Element(HeaderCellStyle).AlignRight().Text("Total");
                            });

                            foreach (var item in quote.QuoteItems)
                            {
                                var lineExVat = item.LineTotalExclVat;
                                var lineVat = item.LineTotal - item.LineTotalExclVat;

                                table.Cell().Element(DataCellStyle)
                                    .Text(item.Quantity.ToString());

                                table.Cell().Element(DataCellStyle)
                                    .Text(item.Product?.ProductCode ?? "");

                                table.Cell().Element(DataCellStyle)
                                    .Text(item.Product?.Description ?? "");

                                table.Cell().Element(DataCellStyle)
                                    .AlignRight()
                                    .Text($"R{item.UnitPrice:0.00}");

                                table.Cell().Element(DataCellStyle)
                                    .AlignRight()
                                    .Text($"{item.DiscountPercent:0.00}%");


                                table.Cell().Element(DataCellStyle)
                                    .AlignRight()
                                    .Text($"R{lineVat:0.00}");

                                table.Cell().Element(DataCellStyle)
                                  .AlignRight()
                                  .Text($"R{lineExVat:0.00}");
                            }
                        });

                        // =========================
                        // BANKING DETAILS + TOTALS
                        // =========================
                        column.Item().PaddingTop(20).BorderTop(1).PaddingTop(10).Row(row =>
                        {
                            row.RelativeItem().Column(banking =>
                            {
                                banking.Item()
                                    .Text("BANKING DETAILS")
                                    .Bold()
                                    .FontSize(10);

                                banking.Item()
                                    .PaddingTop(5)
                                    .Text("Bank: FNB");

                                banking.Item()
                                    .Text("Branch Code: 250655");

                                banking.Item()
                                    .Text("Account Name: Exclusive Distributors (Pty) Ltd");

                                banking.Item()
                                    .Text("Account Number: 63191446202");

                                banking.Item()
                                    .Text($"Reference: {quote.QuoteNumber}");

                                banking.Item()
                                    .Text("Proof of Payment: exclusivedistributors2@gmail.com");
                            });

                            row.ConstantItem(15);

                            row.ConstantItem(220).Column(totals =>
                            {
                                totals.Item().Row(r =>
                                {
                                    r.RelativeItem().Text("Subtotal").FontSize(9);
                                    r.ConstantItem(80).AlignRight().Text($"R{quote.Subtotal:0.00}").FontSize(9);
                                });

                                totals.Item().PaddingTop(3).Row(r =>
                                {
                                    r.RelativeItem().Text("VAT").FontSize(9);
                                    r.ConstantItem(80).AlignRight().Text($"R{(quote.Total - quote.Subtotal):0.00}").FontSize(9);
                                });

                                totals.Item().PaddingTop(6).BorderTop(1).PaddingTop(6).Row(r =>
                                {
                                    r.RelativeItem().Text("TOTAL").Bold().FontSize(13);
                                    r.ConstantItem(80).AlignRight().Text($"R{quote.Total:0.00}").Bold().FontSize(13);
                                });
                            });
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
                                .Text("Quote generated electronically")
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
               .PaddingVertical(5)
                .BorderBottom(1)
                     .Width(70)
                .Padding(5)
                .AlignRight();
        }

        private static IContainer TotalValueStyle(IContainer container)
        {
            return container
                .PaddingVertical(5)
                .BorderBottom(1)
                .Width(70)
                .PaddingBottom(3)
                .AlignRight();
        }
    }
}