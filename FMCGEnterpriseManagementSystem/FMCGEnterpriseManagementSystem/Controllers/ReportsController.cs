using FMCGEnterpriseManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ReportsController : Controller
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> InvoiceReport(
            DateTime? startDate,
            DateTime? endDate)
        {
            var report = await _reportService
                .GetInvoiceReportAsync(startDate, endDate);

            SetDateFilters(startDate, endDate);

            return View(report);
        }

        [HttpGet]
        public async Task<IActionResult> QuoteReport(
            DateTime? startDate,
            DateTime? endDate)
        {
            var report = await _reportService
                .GetQuoteReportAsync(startDate, endDate);

            SetDateFilters(startDate, endDate);

            return View(report);
        }

        [HttpGet]
        public async Task<IActionResult> AgeAnalysis()
        {
            var report = await _reportService
                .GetAgeAnalysisReportAsync();

            return View(report);
        }

        [HttpGet]
        public async Task<IActionResult> CustomerReport()
        {
            var report = await _reportService
                .GetCustomerReportAsync();

            return View(report);
        }

        [HttpGet]
        public async Task<IActionResult> CustomerSales(
            DateTime? startDate,
            DateTime? endDate)
        {
            var report = await _reportService
                .GetCustomerSalesReportAsync(startDate, endDate);

            SetDateFilters(startDate, endDate);

            return View(report);
        }

        [HttpGet]
        public async Task<IActionResult> PaymentsReport(
            DateTime? startDate,
            DateTime? endDate)
        {
            var report = await _reportService
                .GetPaymentReportAsync(startDate, endDate);

            SetDateFilters(startDate, endDate);

            return View(report);
        }

        [HttpGet]
        public async Task<IActionResult> InventoryReport()
        {
            var report = await _reportService
                .GetInventoryReportAsync();

            return View(report);
        }

        [HttpGet]
        public async Task<IActionResult> SalesReport(
            DateTime? startDate,
            DateTime? endDate)
        {
            var report = await _reportService
                .GetSalesReportAsync(startDate, endDate);

            SetDateFilters(startDate, endDate);

            return View(report);
        }

        [HttpGet]
        public async Task<IActionResult> ItemsPerCustomer(
            DateTime? startDate,
            DateTime? endDate)
        {
            var report = await _reportService
                .GetItemsPerCustomerReportAsync(startDate, endDate);

            SetDateFilters(startDate, endDate);

            return View(report);
        }

        [HttpGet]
        public async Task<IActionResult> ItemSalesReport(
            DateTime? startDate,
            DateTime? endDate)
        {
            var report = await _reportService
                .GetItemSalesReportAsync(startDate, endDate);

            SetDateFilters(startDate, endDate);

            return View(report);
        }

        [HttpGet]
        public async Task<IActionResult> SalesRepReport(
            DateTime? startDate,
            DateTime? endDate)
        {
            var report = await _reportService
                .GetSalesRepReportAsync(startDate, endDate);

            SetDateFilters(startDate, endDate);

            return View(report);
        }

        [HttpGet]
        public async Task<IActionResult> SalesVatReport(
            DateTime? startDate,
            DateTime? endDate)
        {
            var report = await _reportService
                .GetSalesVatReportAsync(startDate, endDate);

            SetDateFilters(startDate, endDate);

            return View(report);
        }

        private void SetDateFilters(
            DateTime? startDate,
            DateTime? endDate)
        {
            ViewBag.StartDate = startDate?
                .ToString("yyyy-MM-dd");

            ViewBag.EndDate = endDate?
                .ToString("yyyy-MM-dd");
        }
    }
}