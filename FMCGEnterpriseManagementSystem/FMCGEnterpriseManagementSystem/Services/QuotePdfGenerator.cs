// Title: Asynchronous programming with async and await
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using FMCGEnterpriseManagementSystem.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FMCGEnterpriseManagementSystem.Services
{
    // Static utility class responsible for generating quote documents as PDF files.
    public static class QuotePdfGenerator
    {
        // Generates a PDF representation of the supplied quote and returns it as a byte array.
        public static byte[] Generate(Quote quote)
        {
            // Creates the QuestPDF document and defines its page layout and content.
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    // Configures the PDF page size, margins and default font size.
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    // =========================
                    // HEADER
                    // =========================
                    // Creates the header containing the company information and document title.
                    page.Header().Column(header =>
                    {
                        header.Item().Row(row =>
                        {
                            // Displays the company name, location and business description.
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

                            // Displays the QUOTE title on the right side of the header.
                            row.ConstantItem(150)
                                .AlignRight()
                                .Text("QUOTE")
                                .Bold()
                                .FontSize(24);
                        });

                        // Adds a horizontal line below the company header.
                        header.Item()
                            .PaddingTop(15)
                            .LineHorizontal(1);
                    });

                    // =========================
                    // CONTENT
                    // =========================
                    // Defines the main content area of the PDF.
                    page.Content().Column(column =>
                    {
                        column.Spacing(15);

                        // Displays customer and quote information.
                        column.Item().Row(row =>
                        {
                            // Displays the customer billing information.
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

                            // Displays quote number, date, payment terms and sales representative.
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
                        // Creates the table containing all products included in the quote.
                        column.Item().Table(table =>
                        {
                            // Defines the width of each table column.
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

                            // Creates the table header row.
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

                            // Adds each quoted product as a row in the table.
                            foreach (var item in quote.QuoteItems)
                            {
                                // Calculates the amount excluding VAT and the VAT amount for the item.
                                var lineExVat = item.LineTotalExclVat;
                                var lineVat = item.LineTotal - item.LineTotalExclVat;

                                table.Cell().Element(DataCellStyle)
                                    .Text(item.Quantity.ToString());

                                // Displays the product code.
                                table.Cell().Element(DataCellStyle)
                                    .Text(item.Product?.ProductCode ?? "");

                                // Displays the product description.
                                table.Cell().Element(DataCellStyle)
                                    .Text(item.Product?.Description ?? "");

                                // Displays the unit price.
                                table.Cell().Element(DataCellStyle)
                                    .AlignRight()
                                    .Text($"R{item.UnitPrice:0.00}");

                                // Displays the discount percentage.
                                table.Cell().Element(DataCellStyle)
                                    .AlignRight()
                                    .Text($"{item.DiscountPercent:0.00}%");

                                // Displays the VAT amount for the item.
                                table.Cell().Element(DataCellStyle)
                                    .AlignRight()
                                    .Text($"R{lineVat:0.00}");

                                // Displays the total excluding VAT for the item.
                                table.Cell().Element(DataCellStyle)
                                  .AlignRight()
                                  .Text($"R{lineExVat:0.00}");
                            }
                        });

                        // =========================
                        // BANKING DETAILS + TOTALS
                        // =========================
                        // Displays the company's banking details alongside the quote totals.
                        column.Item().PaddingTop(20).BorderTop(1).PaddingTop(10).Row(row =>
                        {
                            // Displays the banking information required for payment.
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

                                // Uses the quote number as the payment reference.
                                banking.Item()
                                    .Text($"Reference: {quote.QuoteNumber}");

                                banking.Item()
                                    .Text("Proof of Payment: exclusivedistributors2@gmail.com");
                            });

                            row.ConstantItem(15);

                            // Displays the subtotal, VAT and final quote total.
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

                                // Highlights the final total payable on the quote.
                                totals.Item().PaddingTop(6).BorderTop(1).PaddingTop(6).Row(r =>
                                {
                                    r.RelativeItem().Text("TOTAL").Bold().FontSize(13);
                                    r.ConstantItem(80).AlignRight().Text($"R{quote.Total:0.00}").Bold().FontSize(13);
                                });
                            });
                        });

                        // Displays a closing message at the bottom of the quote content.
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
                    // Creates the footer that appears at the bottom of the PDF page.
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

            // Generates the configured QuestPDF document and returns the PDF as a byte array.
            return document.GeneratePdf();
        }

        // =========================
        // TABLE STYLES
        // =========================

        // Defines the visual styling used for table header cells.
        private static IContainer HeaderCellStyle(IContainer container)
        {
            return container
                .Background("#E8E8E8")
                .Border(1)
                .Padding(5)
                .DefaultTextStyle(x => x.Bold().FontSize(8));
        }

        // Defines the visual styling used for table data cells.
        private static IContainer DataCellStyle(IContainer container)
        {
            return container
                .BorderBottom(1)
                .Padding(5)
                .DefaultTextStyle(x => x.FontSize(8));
        }

        // Defines styling for total labels.
        private static IContainer TotalLabelStyle(IContainer container)
        {
            return container
               .PaddingVertical(5)
                .BorderBottom(1)
                     .Width(70)
                .Padding(5)
                .AlignRight();
        }

        // Defines styling for total values.
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