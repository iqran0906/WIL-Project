/***************************************************************************************
*    Title: Dashboard Metric Data Transfer Object
*    Author: Maseeha17
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/DTOs/DashboardMetricDto.cs
***************************************************************************************/

/***************************************************************************************
*    Title: Classes and Objects - C# Programming Guide
*    Author: Microsoft
*    Date: 2026
*    Code version: C#
*    Availability: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/classes
***************************************************************************************/

namespace FMCGEnterpriseManagementSystem.DTOs
{
    // Represents dashboard metric data passed between application layers.
    public class DashboardMetricDto
    {
        // Stores the name or description of the dashboard metric.
        public string MetricName { get; set; } = string.Empty;

        // Stores the numeric value associated with the metric.
        public decimal Value { get; set; }

        // Stores information showing how the metric has changed.
        public string ChangeIndicator { get; set; } = string.Empty;
    }
}