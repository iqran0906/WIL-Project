using ClosedXML.Excel;
using FMCGEnterpriseManagementSystem.DTOs;
using FMCGEnterpriseManagementSystem.Strategies.Interfaces;

namespace FMCGEnterpriseManagementSystem.Strategies
{
    public class ExcelExportStrategy : IExportStrategy
    {
        public ExportResultDto Export<T>(IEnumerable<T> data, string title)
        {
            var properties = typeof(T).GetProperties();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(title);

            for (int col = 0; col < properties.Length; col++)
                worksheet.Cell(1, col + 1).Value = properties[col].Name;

            worksheet.Row(1).Style.Font.Bold = true;

            int row = 2;
            foreach (var item in data)
            {
                for (int col = 0; col < properties.Length; col++)
                {
                    var value = properties[col].GetValue(item);
                    worksheet.Cell(row, col + 1).Value = value?.ToString() ?? "";
                }
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return new ExportResultDto
            {
                FileContents = stream.ToArray(),
                FileName = $"{title}.xlsx",
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            };
        }
    }
}