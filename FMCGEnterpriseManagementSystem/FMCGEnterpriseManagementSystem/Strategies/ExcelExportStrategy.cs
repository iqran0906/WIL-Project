// Purpose: Strategy pattern: exports report rows to an Excel spreadsheet.
// Authors: Sayali-St10458649 (from git history)
// Uses: ClosedXML (MIT) https://github.com/ClosedXML/ClosedXML

using ClosedXML.Excel;
using FMCGEnterpriseManagementSystem.DTOs;
using FMCGEnterpriseManagementSystem.Strategies.Interfaces;

namespace FMCGEnterpriseManagementSystem.Strategies
{
    public class ExcelExportStrategy : IExportStrategy
    {
        public ExportResultDto Export<T>(IEnumerable<T> data, string title)
        {
            var columns = ExportColumnHelper.GetColumns<T>();

            using var workbook = new XLWorkbook();

            // Excel sheet names: max 31 characters, no special characters
            var sheetName = new string(title
                .Where(c => !"[]:*?/\\".Contains(c))
                .Take(31)
                .ToArray());

            var worksheet = workbook.Worksheets.Add(string.IsNullOrWhiteSpace(sheetName) ? "Report" : sheetName);

            for (int col = 0; col < columns.Length; col++)
                worksheet.Cell(1, col + 1).Value = ExportColumnHelper.GetHeader(columns[col]);

            var header = worksheet.Row(1);
            header.Style.Font.Bold = true;
            header.Style.Font.FontColor = XLColor.White;
            header.Style.Fill.BackgroundColor = XLColor.FromHtml("#357383");

            int row = 2;
            foreach (var item in data)
            {
                for (int col = 0; col < columns.Length; col++)
                {
                    var cell = worksheet.Cell(row, col + 1);
                    var value = columns[col].GetValue(item);

                    // Keep numbers and dates as real Excel values so they can be summed/sorted
                    switch (value)
                    {
                        case null:
                            break;
                        case DateTime date:
                            cell.Value = date;
                            cell.Style.DateFormat.Format =
                                date.TimeOfDay == TimeSpan.Zero ? "yyyy-mm-dd" : "yyyy-mm-dd hh:mm";
                            break;
                        case decimal number:
                            cell.Value = number;
                            cell.Style.NumberFormat.Format = "#,##0.00";
                            break;
                        case double or float:
                            cell.Value = Convert.ToDouble(value);
                            cell.Style.NumberFormat.Format = "#,##0.00";
                            break;
                        case int or long or short:
                            cell.Value = Convert.ToInt64(value);
                            break;
                        default:
                            cell.Value = ExportColumnHelper.FormatValue(value);
                            break;
                    }
                }
                row++;
            }

            worksheet.SheetView.FreezeRows(1);
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
