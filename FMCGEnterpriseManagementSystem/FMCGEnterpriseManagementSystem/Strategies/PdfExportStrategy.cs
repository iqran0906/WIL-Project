
//    Title: QuestPDF Documentation
//    Author: Marcin Ziąbek 
//    Date: 2026
//    Code version: QuestPDF
//    Availability: https://www.questpdf.com

using FMCGEnterpriseManagementSystem.DTOs;
using FMCGEnterpriseManagementSystem.Strategies.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FMCGEnterpriseManagementSystem.Strategies
{
    // Strategy responsible for exporting report data into a PDF document.
    // Implements IExportStrategy so it can be used interchangeably with other export formats.
    public class PdfExportStrategy : IExportStrategy
    {
        // Brand colour used for the report headings and table headers.
        private const string BrandColor = "#357383";

        // Exports the supplied data into a formatted PDF document.
        public ExportResultDto Export<T>(IEnumerable<T> data, string title)
        {
            // Retrieves the properties that should be displayed as columns in the report.
            var columns = ExportColumnHelper.GetColumns<T>();

            // Converts the supplied data into a list so it can be counted and accessed by index.
            var rows = data.ToList();

            // Creates a QuestPDF document and defines its page layout and content.
            var document = Document.Create(container =>
            {

 /*****************************
*    Title: Page Settings
*    Author: Marcin Ziąbek (QuestPDF)
*    Date: 2026
*    Code version: QuestPDF 2026.9.1
*    Availability: https://questpdf.com/api-reference/page/settings.html
*****************************/


                container.Page(page =>
                {
                    // Uses landscape A4 format so reports with multiple columns have more space.
                    page.Size(PageSizes.A4.Landscape());

                    // Sets the page margin around the report content.
                    page.Margin(25);

                    // Sets the default font size for text throughout the document.
                    page.DefaultTextStyle(x => x.FontSize(9));

                    // Creates the header section displayed at the top of each page.
                    page.Header().Column(header =>
                    {
                        // Displays the company name using the application's brand colour.
                        header.Item().Text("Exclusive Distributors")
                            .FontSize(10)
                            .FontColor(BrandColor)
                            .Bold();

                        // Displays the title of the report.
                        header.Item().Text(title)
                            .FontSize(16)
                            .Bold();

                        // Displays the generation date, time and number of records in the report.
                        header.Item()
                            .PaddingBottom(8)
                            .Text($"Generated {DateTime.Now:dd MMM yyyy HH:mm} · {rows.Count} record(s)")
                            .FontSize(8)
                            .FontColor(Colors.Grey.Darken1);
                    });

                    // Creates the main report content using a table layout.
                    page.Content().Table(table =>
                    {
  /*****************************
*    Title: Table: Basics
*    Author: Marcin Ziąbek (QuestPDF)
*    Date: 2026
*    Code version: QuestPDF 2026.9.1
*    Availability: https://questpdf.com/api-reference/table/basics.html
*****************************/

                        // Defines the columns in the PDF table.
                        // Each column is given an equal relative width.
                        table.ColumnsDefinition(definition =>
                        {
                            foreach (var _ in columns)
                                definition.RelativeColumn();
                        });


                        /*****************************
*    Title: Dynamic composition: configurable tables
*    Author: Marcin Ziąbek (QuestPDF)
*    Date: 2026
*    Code version: QuestPDF 2026.9.1
*    Availability: https://questpdf.com/concepts/dynamic-composition/configurable-tables.html
*****************************/




                        // Defines the table header row.
                        table.Header(headerRow =>
                        {
                            foreach (var column in columns)
                            {
                                // Creates and styles each header cell.
                                headerRow.Cell()
                                    .Background(BrandColor)
                                    .Padding(4)
                                    .Text(ExportColumnHelper.GetHeader(column))
                                    .FontColor(Colors.White)
                                    .Bold();
                            }
                        });

                        // Displays a message when the report contains no records.
                        if (rows.Count == 0)
                        {
                            table.Cell()
                                .ColumnSpan((uint)Math.Max(columns.Length, 1))
                                .Padding(6)
                                .Text("No records found for the selected filters.")
                                .Italic();
                        }

                        // Loops through each record in the report.
                        for (var i = 0; i < rows.Count; i++)
                        {
                            // Alternates the row background colour to improve table readability.
                            var background = i % 2 == 0 ? Colors.White : Colors.Grey.Lighten4;

                            // Loops through each selected property/column for the current record.
                            foreach (var column in columns)
                            {
                                // Creates the table cell and applies basic formatting.
                                var text = table.Cell()
                                    .Background(background)
                                    .BorderBottom(0.5f)
                                    .BorderColor(Colors.Grey.Lighten2)
                                    .Padding(4);

                                // Retrieves and formats the property value for display.
                                var value = ExportColumnHelper.FormatValue(column.GetValue(rows[i]));

                                // Aligns numeric values to the right for improved readability.
                                if (ExportColumnHelper.IsNumeric(column.PropertyType))
                                    text.AlignRight().Text(value);
                                else
                                    text.Text(value);
                            }
                        }
                    });

                    // Creates the footer section containing the current page and total page count.
                    page.Footer().AlignRight().Text(text =>
                    {
                        text.Span("Page ").FontSize(8);
                        text.CurrentPageNumber().FontSize(8);
                        text.Span(" of ").FontSize(8);
                        text.TotalPages().FontSize(8);
                    });
                });
            });

            // Generates the PDF document and returns it together with the file name and content type.
            return new ExportResultDto
            {
                FileContents = document.GeneratePdf(),
                FileName = $"{title}.pdf",
                ContentType = "application/pdf"
            };
        }
    }
}