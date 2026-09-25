namespace FMCGEnterpriseManagementSystem.DTOs
{
    public class DashboardMetricDto
    {
        public string MetricName { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public string ChangeIndicator { get; set; } = string.Empty;
    }
}