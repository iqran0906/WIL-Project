/***************************************************************************************
*    Title: Reports Controller
*    Author: iqran0906
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Controllers/ReportsController.cs
***************************************************************************************/

/***************************************************************************************
*    Title: Controller action return types in ASP.NET Core MVC
*    Author: Microsoft
*    Date: 2026
*    Code version: ASP.NET Core\
*    Availability: https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/actions
***************************************************************************************/

using FMCGEnterpriseManagementSystem.Enums;
using FMCGEnterpriseManagementSystem.Factories;
using FMCGEnterpriseManagementSystem.Helpers;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    // Only administrators can access the reports section.
    [Authorize(Roles = "Administrator")]
    public class ReportsController : Controller
    {
        // Service handles retrieving the different report datasets.
        private readonly IReportService _reportService;

        // Factory selects the PDF or Excel export strategy.
        private readonly ExportFactory _exportFactory;

        // Logger records important report and export activity.
        private readonly ILogger<ReportsController> _logger;

        // Dependencies are supplied through dependency injection.
        public ReportsController(
            IReportService reportService,
            ExportFactory exportFactory,
            ILogger<ReportsController> logger)
        {
            _reportService = reportService;
            _exportFactory = exportFactory;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        // ==================================================
        // REPORTS WITH A DATE RANGE
        // ==================================================

        // Each dated report uses the shared DatedReport helper to apply date validation and load the requested report.
        [HttpGet]
        public Task<IActionResult> InvoiceReport(DateTime? startDate, DateTime? endDate) =>
            DatedReport(startDate, endDate, () => _reportService.GetInvoiceReportAsync(startDate, endDate));

        [HttpGet]
        public Task<IActionResult> QuoteReport(DateTime? startDate, DateTime? endDate) =>
            DatedReport(startDate, endDate, () => _reportService.GetQuoteReportAsync(startDate, endDate));

        [HttpGet]
        public Task<IActionResult> CustomerSales(DateTime? startDate, DateTime? endDate) =>
            DatedReport(startDate, endDate, () => _reportService.GetCustomerSalesReportAsync(startDate, endDate));

        [HttpGet]
        public Task<IActionResult> PaymentsReport(DateTime? startDate, DateTime? endDate) =>
            DatedReport(startDate, endDate, () => _reportService.GetPaymentReportAsync(startDate, endDate));

        [HttpGet]
        public Task<IActionResult> SalesReport(DateTime? startDate, DateTime? endDate) =>
            DatedReport(startDate, endDate, () => _reportService.GetSalesReportAsync(startDate, endDate));

        [HttpGet]
        public Task<IActionResult> ItemsPerCustomer(DateTime? startDate, DateTime? endDate) =>
            DatedReport(startDate, endDate, () => _reportService.GetItemsPerCustomerReportAsync(startDate, endDate));

        [HttpGet]
        public Task<IActionResult> ItemSalesReport(DateTime? startDate, DateTime? endDate) =>
            DatedReport(startDate, endDate, () => _reportService.GetItemSalesReportAsync(startDate, endDate));

        [HttpGet]
        public Task<IActionResult> SalesRepReport(DateTime? startDate, DateTime? endDate) =>
            DatedReport(startDate, endDate, () => _reportService.GetSalesRepReportAsync(startDate, endDate));

        [HttpGet]
        public Task<IActionResult> SalesVatReport(DateTime? startDate, DateTime? endDate) =>
            DatedReport(startDate, endDate, () => _reportService.GetSalesVatReportAsync(startDate, endDate));


        // ==================================================
        // REPORTS WITHOUT A DATE RANGE
        // ==================================================

        [HttpGet]
        public async Task<IActionResult> AgeAnalysis() =>
            View(await _reportService.GetAgeAnalysisReportAsync());

        [HttpGet]
        public async Task<IActionResult> CustomerReport() =>
            View(await _reportService.GetCustomerReportAsync());

        [HttpGet]
        public async Task<IActionResult> InventoryReport() =>
            View(await _reportService.GetInventoryReportAsync());


        // ==================================================
        // EXPORT (PDF / Excel)
        // GET: Reports/Export?report=SalesReport&format=pdf&startDate=...&endDate=...
        // ==================================================

        [HttpGet]
        public async Task<IActionResult> Export(
            string report,
            string format,
            DateTime? startDate,
            DateTime? endDate)
        {
            // Convert the requested format into the corresponding export strategy.
            ExportType? exportType = format?.ToLowerInvariant() switch
            {
                "pdf" => ExportType.Pdf,
                "excel" => ExportType.Excel,
                _ => null
            };

            // Reject unsupported export formats.
            if (exportType == null)
            {
                TempData["ErrorMessage"] = "Please choose PDF or Excel as the export format.";
                return RedirectToReport(report, startDate, endDate);
            }

            // Prevent reports from being exported with an invalid date range.
            if (!IsValidDateRange(startDate, endDate))
            {
                TempData["ErrorMessage"] = "The report was not exported because the end date is before the start date.";
                return RedirectToReport(report, startDate, endDate);
            }

            var type = exportType.Value;
            var period = DescribePeriod(startDate, endDate);

            // Select the requested report and generate it using the chosen export strategy.
            IActionResult? file = report switch
            {
                nameof(InvoiceReport) => ToFile(await _reportService.GetInvoiceReportAsync(startDate, endDate), $"Invoice Report{period}", type),
                nameof(QuoteReport) => ToFile(await _reportService.GetQuoteReportAsync(startDate, endDate), $"Quote Report{period}", type),
                nameof(CustomerSales) => ToFile(await _reportService.GetCustomerSalesReportAsync(startDate, endDate), $"Customer Sales{period}", type),
                nameof(PaymentsReport) => ToFile(await _reportService.GetPaymentReportAsync(startDate, endDate), $"Payments Report{period}", type),
                nameof(SalesReport) => ToFile(await _reportService.GetSalesReportAsync(startDate, endDate), $"Sales Report{period}", type),
                nameof(ItemsPerCustomer) => ToFile(await _reportService.GetItemsPerCustomerReportAsync(startDate, endDate), $"Items Per Customer{period}", type),
                nameof(ItemSalesReport) => ToFile(await _reportService.GetItemSalesReportAsync(startDate, endDate), $"Item Sales Report{period}", type),
                nameof(SalesRepReport) => ToFile(await _reportService.GetSalesRepReportAsync(startDate, endDate), $"Sales Rep Report{period}", type),
                nameof(SalesVatReport) => ToFile(await _reportService.GetSalesVatReportAsync(startDate, endDate), $"Sales VAT Report{period}", type),
                nameof(AgeAnalysis) => ToFile(await _reportService.GetAgeAnalysisReportAsync(), "Age Analysis", type),
                nameof(CustomerReport) => ToFile(await _reportService.GetCustomerReportAsync(), "Customer Report", type),
                nameof(InventoryReport) => ToFile(await _reportService.GetInventoryReportAsync(), "Inventory Report", type),
                _ => null
            };

            if (file == null)
            {
                return NotFound();
            }

            // Record which user exported which report and in which format.
            _logger.LogInformation(
                "{User} exported {Report} as {Format}{Period}",
                User.Identity?.Name,
                report,
                type,
                period);

            return file;
        }


        // ==================================================
        // HELPERS
        // ==================================================

        // Shared helper for reports that support start and end dates.
        private async Task<IActionResult> DatedReport<T>(
            DateTime? startDate,
            DateTime? endDate,
            Func<Task<IEnumerable<T>>> loadReport)
        {
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            // Don't run the report on an impossible range - show the error instead.
            if (!IsValidDateRange(startDate, endDate))
            {
                ViewBag.DateError = "The end date cannot be before the start date.";
                return View(Enumerable.Empty<T>());
            }

            return View(await loadReport());
        }

        // Checks that the start date is not later than the end date.
        private static bool IsValidDateRange(DateTime? startDate, DateTime? endDate) =>
            !(startDate.HasValue && endDate.HasValue && startDate.Value.Date > endDate.Value.Date);

        // Creates a readable description of the selected reporting period.
        private static string DescribePeriod(DateTime? startDate, DateTime? endDate) =>
            (startDate, endDate) switch
            {
                (null, null) => string.Empty,
                ({ } s, null) => $" from {s:yyyy-MM-dd}",
                (null, { } e) => $" to {e:yyyy-MM-dd}",
                ({ } s, { } e) => $" {s:yyyy-MM-dd} to {e:yyyy-MM-dd}"
            };

        // Uses the selected export strategy to convert report data into a downloadable file.
        private IActionResult ToFile<T>(IEnumerable<T> data, string title, ExportType type) =>
            FileExportHelper.ToFileResult(_exportFactory.GetStrategy(type).Export(data, title));

        // Redirects users back to a valid report when export validation fails.
        private IActionResult RedirectToReport(string? report, DateTime? startDate, DateTime? endDate)
        {
            // Only known report actions are allowed as redirect targets.
            var knownReports = new[]
            {
                nameof(InvoiceReport), nameof(QuoteReport), nameof(CustomerSales), nameof(PaymentsReport),
                nameof(SalesReport), nameof(ItemsPerCustomer), nameof(ItemSalesReport), nameof(SalesRepReport),
                nameof(SalesVatReport), nameof(AgeAnalysis), nameof(CustomerReport), nameof(InventoryReport)
            };

            if (report == null || !knownReports.Contains(report))
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(report, new
            {
                startDate = startDate?.ToString("yyyy-MM-dd"),
                endDate = endDate?.ToString("yyyy-MM-dd")
            });
        }
    }
}