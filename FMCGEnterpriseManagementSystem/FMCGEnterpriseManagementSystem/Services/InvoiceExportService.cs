```csharp
// Title: Asynchronous programming with async and await
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FMCGEnterpriseManagementSystem.Services
{
    // Service responsible for generating printable PDF invoices.
    // Implements IInvoiceExportService so invoice export functionality
    // can be used through dependency injection.
    public class InvoiceExportService : IInvoiceExportService
    {
        // Generates a PDF document from the supplied invoice view model.
        // The resulting PDF is returned as a byte array so it can be
        // downloaded or returned directly from an MVC controller.
        public byte[] GenerateInvoicePdf(InvoiceViewModel invoice)
        {
            // Creates a QuestPDF document and defines its page layout.
            var document = Document.Create(container =>
            {
                // Creates an A4 page for the invoice.
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);

                    // Sets the page margin in points.
                    page.Margin(40);

                    // Sets the default font size for text on the page.
                    page.DefaultTextStyle(x => x.FontSize(10));

                    // =========================
                    // HEADER
                    // =========================

                    // Defines the invoice header section.
                    page.Header().Column(header =>
                    {
                        // Places the company information and invoice title
                        // next to each other horizontally.
                        header.Item().Row(row =>
                        {
                            // RelativeItem allows the company information
                            // to use the available horizontal space.
                            row.RelativeItem().Column(company =>
                            {
                                // Displays the company name prominently.
                                company.Item()
                                    .Text("EXCLUSIVE DISTRIBUTORS (PTY) LTD")
                                    .Bold()
                                    .FontSize(18);

                                // Displays the company's location.
                                company.Item()
                                    .Text("Durban, South Africa")
                                    .FontSize(10);

                                // Displays the type of business.
                                company.Item()
                                    .Text("Wholesale & Distribution")
                                    .FontSize(9);
                            });

                            // Reserves a fixed-width area for the invoice heading.
                            row.ConstantItem(150)
                                .AlignRight()
                                .Text("INVOICE")
                                .Bold()
                                .FontSize(24);
                        });

                        // Adds spacing followed by a horizontal divider
                        // underneath the invoice header.
                        header.Item()
                            .PaddingTop(15)
                            .LineHorizontal(1);
                    });

                    // =========================
                    // CONTENT
                    // =========================

                    // Defines the main invoice content.
                    page.Content().Column(column =>
                    {
                        // Adds consistent spacing between content sections.
                        column.Spacing(15);

                        // Invoice information
                        column.Item().Row(row =>
                        {
                            // Displays customer billing information.
                            row.RelativeItem().Background("#F5F5F5").Padding(10).Column(left =>
                            {
                                left.Item()
                                    .Text("BILL TO")
                                    .Bold()
                                    .FontSize(10);

                                // Displays the customer's name.
                                // "N/A" is used if no value is supplied.
                                left.Item()
                                    .PaddingTop(5)
                                    .Text(invoice.CustomerName ?? "N/A")
                                    .Bold();

                                // Displays the customer's business name.
                                left.Item()
                                    .Text(invoice.BusinessName ?? "N/A");

                                // Displays the customer's billing address.
                                left.Item()
                                    .Text(invoice.BillingAddress ?? "N/A");
                            });

                            // Adds horizontal spacing between the two
                            // invoice information sections.
                            row.ConstantItem(15);

                            // Displays invoice-specific information.
                            row.ConstantItem(190).Background("#F5F5F5").Padding(10).Column(right =>
                            {
                                // Displays the invoice number.
                                right.Item()
                                    .Text($"Invoice No: {invoice.InvoiceNumber}")
                                    .Bold();

                                // Displays the invoice date using
                                // day/month/year formatting.
                                right.Item()
                                    .Text($"Invoice Date: {invoice.InvoiceDate:dd/MM/yyyy}");

                                // Displays the agreed payment terms.
                                right.Item()
                                    .Text($"Payment Terms: {invoice.PaymentTerms ?? "N/A"}");
                            });
                        });

                        // =========================
                        // ITEMS TABLE
                        // =========================

                        // Creates the table containing the products
                        // included on the invoice.
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

                            // Defines the table header row.
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

                            // Loops through each item belonging to the invoice.
                            foreach (var item in invoice.Items)
                            {
                                // Calculates the line amount excluding VAT
                                // after applying the item's discount.
                                var lineExVat = item.Quantity *
                                                item.UnitPrice *
                                                (1 - item.DiscountPercent / 100m);

                                // Calculates VAT at 15% unless the item is
                                // explicitly marked as VAT exempt/[NONE].
                                var lineVat = item.VatCategory == "[NONE]"
                                    ? 0
                                    : lineExVat * 0.15m;

                                // Displays the quantity ordered.
                                table.Cell().Element(DataCellStyle)
                                    .Text(item.Quantity.ToString());

                                // Displays the product/item code.
                                table.Cell().Element(DataCellStyle)
                                    .Text(item.ItemCode ?? "");

                                // Displays the product description.
                                table.Cell().Element(DataCellStyle)
                                    .Text(item.Description ?? "");

                                // Displays the unit price formatted as currency.
                                table.Cell().Element(DataCellStyle)
                                    .AlignRight()
                                    .Text($"R{item.UnitPrice:0.00}");

                                // Displays the discount percentage.
                                table.Cell().Element(DataCellStyle)
                                    .AlignRight()
                                    .Text($"{item.DiscountPercent:0.00}%");

                                // Displays the calculated line total excluding VAT.
                                table.Cell().Element(DataCellStyle)
                                    .AlignRight()
                                    .Text($"R{lineExVat:0.00}");

                                // Displays the VAT calculated for the line item.
                                table.Cell().Element(DataCellStyle)
                                    .AlignRight()
                                    .Text($"R{lineVat:0.00}");
                            }
                        });

                        // =========================
                        // TOTALS
                        // =========================

                        // Creates a right-aligned table for invoice totals.
                        column.Item()
                            .AlignRight()
                            .Width(250)
                            .Table(table =>
                            {
                                // Defines the label and value columns.
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.ConstantColumn(100);
                                });

                                // Displays the invoice subtotal.
                                table.Cell().Element(TotalLabelStyle)
                                    .Text("Subtotal");

                                table.Cell().Element(TotalValueStyle)
                                    .Text($"R{invoice.Subtotal:0.00}");

                                // Displays the total VAT amount.
                                table.Cell().Element(TotalLabelStyle)
                                    .Text("VAT");

                                table.Cell().Element(TotalValueStyle)
                                    .Text($"R{invoice.VatTotal:0.00}");

                                // Displays the final invoice total.
                                table.Cell().Element(TotalLabelStyle)
                                    .Text("TOTAL")
                                    .Bold();

                                table.Cell().Element(TotalValueStyle)
                                    .Text($"R{invoice.Total:0.00}")
                                    .Bold()
                                    .FontSize(11);

                                // Displays the outstanding amount still payable.
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

                        // Adds payment instructions underneath the totals.
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

                                // Displays the payment terms.
                                payment.Item()
                                    .PaddingTop(5)
                                    .Text($"Payment Terms: {invoice.PaymentTerms ?? "N/A"}");

                                // Instructs the customer to use the invoice
                                // number when making a payment.
                                payment.Item()
                                    .Text("Please use the invoice number as your payment reference.");
                            });

                        // Displays a closing message at the bottom
                        // of the invoice content.
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

                    // Defines the footer displayed at the bottom of the page.
                    page.Footer()
                        .AlignCenter()
                        .Column(footer =>
                        {
                            // Adds a horizontal separator above the footer.
                            footer.Item()
                                .LineHorizontal(1);

                            // Displays company and location information.
                            footer.Item()
                                .PaddingTop(5)
                                .Text("Exclusive Distributors (Pty) Ltd • Durban, South Africa")
                                .FontSize(8);

                            // Indicates that the invoice was generated electronically.
                            footer.Item()
                                .Text("Invoice generated electronically")
                                .FontSize(8);
                        });
                });
            });

            // Generates the completed QuestPDF document as a byte array.
            return document.GeneratePdf();
        }

        // =========================
        // TABLE STYLES
        // =========================

        // Defines the reusable styling applied to invoice table headers.
        private static IContainer HeaderCellStyle(IContainer container)
        {
            return container
                .Background("#E8E8E8")
                .Border(1)
                .Padding(5)
                .DefaultTextStyle(x => x.Bold().FontSize(8));
        }

        // Defines the reusable styling applied to normal invoice data cells.
        private static IContainer DataCellStyle(IContainer container)
        {
            return container
                .BorderBottom(1)
                .Padding(5)
                .DefaultTextStyle(x => x.FontSize(8));
        }

        // Defines the styling used for labels in the totals section.
        private static IContainer TotalLabelStyle(IContainer container)
        {
            return container
                .BorderBottom(1)
                .Padding(5)
                .AlignRight();
        }

        // Defines the styling used for numeric values in the totals section.
        private static IContainer TotalValueStyle(IContainer container)
        {
            return container
                .BorderBottom(1)
                .Padding(5)
                .AlignRight();
        }
    }
}
```
