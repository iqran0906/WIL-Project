// Purpose: Strategy pattern: exports report rows to a formatted PDF.
// Authors: Sayali-St10458649 (from git history)
// Uses: QuestPDF (QuestPDF Community License) https://www.questpdf.com

using FMCGEnterpriseManagementSystem.DTOs;
using FMCGEnterpriseManagementSystem.Strategies.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FMCGEnterpriseManagementSystem.Strategies
{
    public class PdfExportStrategy : IExportStrategy
    {
        private const string BrandColor = "#357383";

        public ExportResultDto Export<T>(IEnumerable<T> data, string title)
        {
            var columns = ExportColumnHelper.GetColumns<T>();
            var rows = data.ToList();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    // Landscape so wide reports fit
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(25);
                    page.DefaultTextStyle(x => x.FontSize(9));

                    page.Header().Column(header =>
                    {
                        header.Item().Text("Exclusive Distributors").FontSize(10).FontColor(BrandColor).Bold();
                        header.Item().Text(title).FontSize(16).Bold();
                        header.Item()
                            .PaddingBottom(8)
                            .Text($"Generated {DateTime.Now:dd MMM yyyy HH:mm} · {rows.Count} record(s)")
                            .FontSize(8)
                            .FontColor(Colors.Grey.Darken1);
                    });

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(definition =>
                        {
                            foreach (var _ in columns)
                                definition.RelativeColumn();
                        });

                        table.Header(headerRow =>
                        {
                            foreach (var column in columns)
                            {
                                headerRow.Cell()
                                    .Background(BrandColor)
                                    .Padding(4)
                                    .Text(ExportColumnHelper.GetHeader(column))
                                    .FontColor(Colors.White)
                                    .Bold();
                            }
                        });

                        if (rows.Count == 0)
                        {
                            table.Cell()
                                .ColumnSpan((uint)Math.Max(columns.Length, 1))
                                .Padding(6)
                                .Text("No records found for the selected filters.")
                                .Italic();
                        }

                        for (var i = 0; i < rows.Count; i++)
                        {
                            var background = i % 2 == 0 ? Colors.White : Colors.Grey.Lighten4;

                            foreach (var column in columns)
                            {
                                var text = table.Cell()
                                    .Background(background)
                                    .BorderBottom(0.5f)
                                    .BorderColor(Colors.Grey.Lighten2)
                                    .Padding(4);

                                var value = ExportColumnHelper.FormatValue(column.GetValue(rows[i]));

                                if (ExportColumnHelper.IsNumeric(column.PropertyType))
                                    text.AlignRight().Text(value);
                                else
                                    text.Text(value);
                            }
                        }
                    });

                    page.Footer().AlignRight().Text(text =>
                    {
                        text.Span("Page ").FontSize(8);
                        text.CurrentPageNumber().FontSize(8);
                        text.Span(" of ").FontSize(8);
                        text.TotalPages().FontSize(8);
                    });
                });
            });

            return new ExportResultDto
            {
                FileContents = document.GeneratePdf(),
                FileName = $"{title}.pdf",
                ContentType = "application/pdf"
            };
        }
    }
}
