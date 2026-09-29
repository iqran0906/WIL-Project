namespace FMCGEnterpriseManagementSystem.DTOs
{
    public class ForecastResultDto
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal CurrentStock { get; set; }
        public decimal PredictedDemand { get; set; }
        public string ForecastPeriod { get; set; } // e.g., "Next Month"
    }
}