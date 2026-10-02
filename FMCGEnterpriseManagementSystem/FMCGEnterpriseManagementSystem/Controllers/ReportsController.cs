// Purpose: Reports: shows each report with date filters and exports them to PDF / Excel.
// Authors: iqran0906 (from git history)

using FMCGEnterpriseManagementSystem.Enums;
using FMCGEnterpriseManagementSystem.Factories;
using FMCGEnterpriseManagementSystem.Helpers;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ReportsController : Controller
    {
        private readonly IReportService _reportService;
        private readonly ExportFactory _exportFactory;
        private readonly ILogger<ReportsController> _logger;

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
            ExportType? exportType = format?.ToLowerInvariant() switch
            {
                "pdf" => ExportType.Pdf,
                "excel" => ExportType.Excel,
                _ => null
            };

            if (exportType == null)
            {
                TempData["ErrorMessage"] = "Please choose PDF or Excel as the export format.";
                return RedirectToReport(report, startDate, endDate);
            }

            if (!IsValidDateRange(startDate, endDate))
            {
                TempData["ErrorMessage"] = "The report was not exported because the end date is before the start date.";
                return RedirectToReport(report, startDate, endDate);
            }

            var type = exportType.Value;
            var period = DescribePeriod(startDate, endDate);

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

        private async Task<IActionResult> DatedReport<T>(
            DateTime? startDate,
            DateTime? endDate,
            Func<Task<IEnumerable<T>>> loadReport)
        {
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            // Don't run the report on an impossible range - show the error instead
            if (!IsValidDateRange(startDate, endDate))
            {
                ViewBag.DateError = "The end date cannot be before the start date.";
                return View(Enumerable.Empty<T>());
            }

            return View(await loadReport());
        }

        private static bool IsValidDateRange(DateTime? startDate, DateTime? endDate) =>
            !(startDate.HasValue && endDate.HasValue && startDate.Value.Date > endDate.Value.Date);

        private static string DescribePeriod(DateTime? startDate, DateTime? endDate) =>
            (startDate, endDate) switch
            {
                (null, null) => string.Empty,
                ({ } s, null) => $" from {s:yyyy-MM-dd}",
                (null, { } e) => $" to {e:yyyy-MM-dd}",
                ({ } s, { } e) => $" {s:yyyy-MM-dd} to {e:yyyy-MM-dd}"
            };

        private IActionResult ToFile<T>(IEnumerable<T> data, string title, ExportType type) =>
            FileExportHelper.ToFileResult(_exportFactory.GetStrategy(type).Export(data, title));

        private IActionResult RedirectToReport(string? report, DateTime? startDate, DateTime? endDate)
        {
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
