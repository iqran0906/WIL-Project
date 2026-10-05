

using FMCGEnterpriseManagementSystem.Factories;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;
using FMCGEnterpriseManagementSystem.Helpers;
using FMCGEnterpriseManagementSystem.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    // Restricts payment functionality to authorized business users.
    [Authorize(Roles = "Administrator,Employee,SalesRepresentative")]
    public class PaymentsController : Controller
    {
        // Payment service handles the payment-related business operations.
        private readonly IPaymentService _paymentService;

        // ExportFactory selects the required export strategy, such as PDF or Excel.
        private readonly ExportFactory _exportFactory;
        private readonly IEmailApiClientService _emailApiClientService;

   // Dependency injection provides the payment service and export factory.
        public PaymentsController(IPaymentService paymentService, ExportFactory exportFactory, IEmailApiClientService emailApiClientService)
        {
            // Title: Factory Design Pattern in C#
            // Author: Code Maze
            // Date: 18-02-2019
            // Code version: C# / .NET
            // Availability: https://code-maze.com/factory-method/

            _paymentService = paymentService;
            _exportFactory = exportFactory;
            _emailApiClientService = emailApiClientService;
        }

        // Displays all recorded payments.
        public async Task<IActionResult> Index()
        {
            var payments = await _paymentService.GetAllPaymentsAsync();
            return View(payments);
        }

        // Displays invoices that are available for recording a payment.
        public async Task<IActionResult> SelectInvoice()
        {
            var invoices = await _paymentService.GetAvailableInvoicesAsync();
            return View(invoices);
        }

        // Loads the payment form for the selected invoice.
        public async Task<IActionResult> Create(int invoiceId)
        {
            try
            {
                var model =
                    await _paymentService.GetPaymentFormForInvoiceAsync(invoiceId);

                return View(model);
            }
            catch (InvalidOperationException ex)
            {
                // Displays the service error and returns the user to invoice selection.
                TempData["ErrorMessage"] = ex.Message;

                return RedirectToAction(nameof(SelectInvoice));
            }
        }

        // Processes and records a submitted payment.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PaymentViewModel model)
        {
            // Prevents invalid payment data from being submitted to the service.
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
                // Handles business-rule errors returned by the payment service.
                TempData["ErrorMessage"] = ex.Message;

                return RedirectToAction(nameof(SelectInvoice));
            }
        }

        // Displays the details of a specific payment.
        public async Task<IActionResult> View(int id)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(id);

            if (payment == null)
                return NotFound();

            return View(payment);
        }

        // Exports all payments as a PDF file.

        // POST: Payments/EmailPayment/5
        [HttpPost]
        [ValidateAntiForgeryToken]
      
        public async Task<IActionResult> EmailPayment(int id, string recipientEmail)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(id);
            if (payment == null) return NotFound();

            var fileName = $"Payment-{id}.pdf";
            var pdf = new Rotativa.AspNetCore.ViewAsPdf("View", payment) { FileName = fileName };
            var pdfBytes = await pdf.BuildFile(ControllerContext);

            var result = await _emailApiClientService.EmailPaymentAsync(id, recipientEmail, pdfBytes, fileName);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction(nameof(View), new { id });
        }

        public async Task<IActionResult> ExportPdf()
        {
            var payments = await _paymentService.GetAllPaymentsAsync();

            // The factory selects the PDF export strategy.
            var strategy = _exportFactory.GetStrategy(ExportType.Pdf);

            var result = strategy.Export(payments, "Payments");

            return FileExportHelper.ToFileResult(result);
        }

        // Exports all payments as an Excel file.
        public async Task<IActionResult> ExportExcel()
        {
            var payments = await _paymentService.GetAllPaymentsAsync();

            // The factory selects the Excel export strategy.
            var strategy = _exportFactory.GetStrategy(ExportType.Excel);

            var result = strategy.Export(payments, "Payments");

            return FileExportHelper.ToFileResult(result);
        }
    }
}