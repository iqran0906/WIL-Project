/***************************************************************************************
*    Title: Shared Export Formatting Helper
*    Author: ST10068525
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Strategies/ExportFormattingHelper.cs
***************************************************************************************/

/***************************************************************************************
*    Title: Classes and Objects - C# Programming Guide
*    Author: Microsoft
*    Date: 2026
*    Code version: C#
*    Availability: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/classes
***************************************************************************************/

using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace FMCGEnterpriseManagementSystem.Strategies
{
    // Shared column and formatting rules for the PDF and Excel exports
    public static class ExportColumnHelper
    {
        private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("en-ZA");

        // Only simple values become columns (lists, nested objects, etc. are skipped)
        public static PropertyInfo[] GetColumns<T>()
        {
            return typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && IsSimple(p.PropertyType))
                .ToArray();
        }

        // [Display(Name = "...")] if set, otherwise "CustomerName" -> "Customer Name"
        public static string GetHeader(PropertyInfo property)
        {
            var display = property.GetCustomAttribute<DisplayAttribute>()?.GetName();

            return !string.IsNullOrWhiteSpace(display)
                ? display
                : Regex.Replace(property.Name, "(?<=[a-z0-9])(?=[A-Z])|(?<=[A-Z])(?=[A-Z][a-z])", " ");
        }

        public static string FormatValue(object? value)
        {
            return value switch
            {
                null => string.Empty,
                DateTime date => date.TimeOfDay == TimeSpan.Zero
                    ? date.ToString("yyyy-MM-dd", Culture)
                    : date.ToString("yyyy-MM-dd HH:mm", Culture),
                decimal number => number.ToString("N2", Culture),
                double number => number.ToString("N2", Culture),
                float number => number.ToString("N2", Culture),
                bool flag => flag ? "Yes" : "No",
                _ => value.ToString() ?? string.Empty
            };
        }

        public static bool IsNumeric(Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;

            return type == typeof(int) || type == typeof(long) || type == typeof(short) ||
                   type == typeof(decimal) || type == typeof(double) || type == typeof(float);
        }

        private static bool IsSimple(Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;

            return type.IsPrimitive || type.IsEnum ||
                   type == typeof(string) || type == typeof(decimal) ||
                   type == typeof(DateTime) || type == typeof(DateTimeOffset) ||
                   type == typeof(Guid);
        }
    }
}
