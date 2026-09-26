using FMCGEnterpriseManagementSystem.Factories;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;
using FMCGEnterpriseManagementSystem.Helpers;
using FMCGEnterpriseManagementSystem.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    [Authorize(Roles = "Administrator,Employee,SalesRepresentative")]
    public class PaymentsController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly ExportFactory _exportFactory;

        public PaymentsController(IPaymentService paymentService, ExportFactory exportFactory)
        {
            _paymentService = paymentService;
            _exportFactory = exportFactory;
        }

        public async Task<IActionResult> Index()
        {
            var payments = await _paymentService.GetAllPaymentsAsync();
            return View(payments);
        }

        public async Task<IActionResult> Create(int invoiceId)
        {
            var model = await _paymentService.GetPaymentFormForInvoiceAsync(invoiceId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PaymentViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _paymentService.RecordPaymentAsync(model);
                TempData["SuccessMessage"] = "Payment recorded successfully.";
                return RedirectToAction("Index");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var refreshed = await _paymentService.GetPaymentFormForInvoiceAsync(model.InvoiceId);
                return View(refreshed);
            }
        }

        public async Task<IActionResult> View(int id)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(id);

            if (payment == null)
                return NotFound();

            return View(payment);
        }

        public async Task<IActionResult> ExportPdf()
        {
            var payments = await _paymentService.GetAllPaymentsAsync();
            var strategy = _exportFactory.GetStrategy(ExportType.Pdf);
            var result = strategy.Export(payments, "Payments");
            return FileExportHelper.ToFileResult(result);
        }

        public async Task<IActionResult> ExportExcel()
        {
            var payments = await _paymentService.GetAllPaymentsAsync();
            var strategy = _exportFactory.GetStrategy(ExportType.Excel);
            var result = strategy.Export(payments, "Payments");
            return FileExportHelper.ToFileResult(result);
        }
    }
}