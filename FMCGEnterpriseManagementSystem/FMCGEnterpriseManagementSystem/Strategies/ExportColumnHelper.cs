
//    Title: Classes and Objects - C# Programming Guide
//    Author: Microsoft
//    Date: 2026
//    Code version: C#
//    Availability: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/classes
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace FMCGEnterpriseManagementSystem.Strategies
{
    // Shared column selection and formatting rules used by the export strategies.
    public static class ExportColumnHelper
    {
        // Defines the South African culture used when formatting exported values.
        private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("en-ZA");

        // Retrieves the public readable properties that can be represented as simple columns.
        public static PropertyInfo[] GetColumns<T>()
        {
            return typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && IsSimple(p.PropertyType))
                .ToArray();
        }

        // Gets a user-friendly column header.
        // Uses the Display attribute when available; otherwise converts
        // a property such as "CustomerName" into "Customer Name".
        public static string GetHeader(PropertyInfo property)
        {
            // Checks whether the property has a Display attribute with a custom name.
            var display = property.GetCustomAttribute<DisplayAttribute>()?.GetName();

            // Returns the custom display name or generates a readable name from the property name.
            return !string.IsNullOrWhiteSpace(display)
                ? display
                : Regex.Replace(property.Name, "(?<=[a-z0-9])(?=[A-Z])|(?<=[A-Z])(?=[A-Z][a-z])", " ");
        }

        // Converts values into consistent text representations for exports.
        public static string FormatValue(object? value)
        {
            return value switch
            {
                // Represents missing values as an empty string.
                null => string.Empty,

                // Formats dates according to whether they include a time component.
                DateTime date => date.TimeOfDay == TimeSpan.Zero
                    ? date.ToString("yyyy-MM-dd", Culture)
                    : date.ToString("yyyy-MM-dd HH:mm", Culture),

                // Formats decimal numbers with two decimal places.
                decimal number => number.ToString("N2", Culture),

                // Formats double values with two decimal places.
                double number => number.ToString("N2", Culture),

                // Formats floating-point values with two decimal places.
                float number => number.ToString("N2", Culture),

                // Converts Boolean values into user-friendly Yes/No text.
                bool flag => flag ? "Yes" : "No",

                // Converts all other supported values to their string representation.
                _ => value.ToString() ?? string.Empty
            };
        }

        // Determines whether a type represents a numeric value.
        public static bool IsNumeric(Type type)
        {
            // Removes nullable wrapping before checking the underlying type.
            type = Nullable.GetUnderlyingType(type) ?? type;

            return type == typeof(int) || type == typeof(long) || type == typeof(short) ||
                   type == typeof(decimal) || type == typeof(double) || type == typeof(float);
        }

        // Determines whether a property can be treated as a simple exportable value.
        private static bool IsSimple(Type type)
        {
            // Removes nullable wrapping before checking the underlying type.
            type = Nullable.GetUnderlyingType(type) ?? type;

            // Allows primitive types, enumerations, strings, numeric values,
            // date/time values and GUIDs to be exported as individual columns.
            return type.IsPrimitive || type.IsEnum ||
                   type == typeof(string) || type == typeof(decimal) ||
                   type == typeof(DateTime) || type == typeof(DateTimeOffset) ||
                   type == typeof(Guid);
        }
    }
}