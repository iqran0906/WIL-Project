/***************************************************************************************
*    Title: Forecast Result Data Transfer Object
*    Author: Maseeha17          
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/DTOs/ForecastResultDto.cs
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
    // Stores the forecast information passed between application layers.
    public class ForecastResultDto
    {
        // Identifies the product being forecast.
        public int ProductID { get; set; }

        // Stores the name of the product for display in forecast results.
        public string ProductName { get; set; }

        // Stores the quantity of the product currently available in stock.
        public decimal CurrentStock { get; set; }

        // Stores the predicted demand for the selected forecast period.
        public decimal PredictedDemand { get; set; }

        // Describes the period covered by the forecast, such as "Next Month".
        public string ForecastPeriod { get; set; } // e.g., "Next Month"
    }
}