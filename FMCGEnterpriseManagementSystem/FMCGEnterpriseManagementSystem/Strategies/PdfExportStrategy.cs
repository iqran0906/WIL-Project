using FMCGEnterpriseManagementSystem.DTOs;
using FMCGEnterpriseManagementSystem.Strategies.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Reflection;
using System.Reflection.Metadata;

namespace FMCGEnterpriseManagementSystem.Strategies
{
    public class PdfExportStrategy : IExportStrategy
    {
        public ExportResultDto Export<T>(IEnumerable<T> data, string title)
        {
            var properties = typeof(T).GetProperties();

            var document = QuestPDF.Fluent.Document.Create(container => {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Header().Text(title).FontSize(18).Bold();

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            foreach (var _ in properties)
                                columns.RelativeColumn();
                        });

                        foreach (var prop in properties)
                            table.Cell().Text(prop.Name).Bold();

                        foreach (var item in data)
                        {
                            foreach (var prop in properties)
                                table.Cell().Text(prop.GetValue(item)?.ToString() ?? "");
                        }
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