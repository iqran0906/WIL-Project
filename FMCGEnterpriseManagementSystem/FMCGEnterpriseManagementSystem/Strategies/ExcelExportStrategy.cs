
//    Title: Classes and Objects - C# Programming Guide
//    Author: Microsoft
//    Date: 2026
//    Code version: C#
//    Availability: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/classes

//    Title: ClosedXML Documentation
//    Author: ClosedXML
//    Date: 2026
//    Code version: ClosedXML
//    Availability: https://github.com/ClosedXML/ClosedXML

using ClosedXML.Excel;
using FMCGEnterpriseManagementSystem.DTOs;
using FMCGEnterpriseManagementSystem.Strategies.Interfaces;

namespace FMCGEnterpriseManagementSystem.Strategies
{
    // Strategy responsible for exporting report data into an Excel workbook.
    public class ExcelExportStrategy : IExportStrategy
    {
        // Generates an Excel file from a collection of data and returns the file information.
        public ExportResultDto Export<T>(IEnumerable<T> data, string title)
        {
            // Retrieves the properties that should be included as Excel columns.
            var columns = ExportColumnHelper.GetColumns<T>();

            // Creates a new Excel workbook.
            using var workbook = new XLWorkbook();

            // Excel sheet names have a maximum length of 31 characters
            // and cannot contain certain special characters.
            var sheetName = new string(title
                .Where(c => !"[]:*?/\\ ".Contains(c))
                .Take(31)
                .ToArray());

            // Creates the worksheet using the report title or a default name.
            var worksheet = workbook.Worksheets.Add(string.IsNullOrWhiteSpace(sheetName) ? "Report" : sheetName);

            // Adds the column headers to the first row of the worksheet.
            for (int col = 0; col < columns.Length; col++)
                worksheet.Cell(1, col + 1).Value = ExportColumnHelper.GetHeader(columns[col]);

            // Formats the header row to make the column names easier to identify.
            var header = worksheet.Row(1);
            header.Style.Font.Bold = true;
            header.Style.Font.FontColor = XLColor.White;
            header.Style.Fill.BackgroundColor = XLColor.FromHtml("#357383");

            // Starts writing exported data from the second row.
            int row = 2;

            // Processes each item that needs to be exported.
            foreach (var item in data)
            {
                // Processes each selected property and places it into the appropriate cell.
                for (int col = 0; col < columns.Length; col++)
                {
                    var cell = worksheet.Cell(row, col + 1);
                    var value = columns[col].GetValue(item);

                    // Keeps numbers and dates as real Excel values so they can be
                    // sorted, filtered and used in calculations within Excel.
                    switch (value)
                    {
                        // Leaves the cell empty when there is no value.
                        case null:
                            break;

                        // Stores dates as actual Excel dates and applies an appropriate format.
                        case DateTime date:
                            cell.Value = date;
                            cell.Style.DateFormat.Format =
                                date.TimeOfDay == TimeSpan.Zero ? "yyyy-mm-dd" : "yyyy-mm-dd hh:mm";
                            break;

                        // Stores decimal values as numbers with two decimal places.
                        case decimal number:
                            cell.Value = number;
                            cell.Style.NumberFormat.Format = "#,##0.00";
                            break;

                        // Converts floating-point values to Excel numeric values.
                        case double or float:
                            cell.Value = Convert.ToDouble(value);
                            cell.Style.NumberFormat.Format = "#,##0.00";
                            break;

                        // Stores integer values as numeric Excel values.
                        case int or long or short:
                            cell.Value = Convert.ToInt64(value);
                            break;

                        // Formats other values as text using the shared helper.
                        default:
                            cell.Value = ExportColumnHelper.FormatValue(value);
                            break;
                    }
                }

                // Moves to the next worksheet row.
                row++;
            }

            // Keeps the header row visible when scrolling through the worksheet.
            worksheet.SheetView.FreezeRows(1);

            // Automatically adjusts column widths based on their contents.
            worksheet.Columns().AdjustToContents();

            // Creates an in-memory stream for the generated Excel workbook.
            using var stream = new MemoryStream();

            // Saves the workbook into the memory stream.
            workbook.SaveAs(stream);

            // Returns the generated file together with its name and content type.
            return new ExportResultDto
            {
                FileContents = stream.ToArray(),
                FileName = $"{title}.xlsx",
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            };
        }
    }
}