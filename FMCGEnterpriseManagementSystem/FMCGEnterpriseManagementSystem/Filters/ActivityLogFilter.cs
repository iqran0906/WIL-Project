/***************************************************************************************
*    Title: Global MVC Activity Logging Filter
*    Author: ST10068525
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Filters/ActivityLoggingFilter.cs
***************************************************************************************/

/***************************************************************************************
*    Title: Filters in ASP.NET Core MVC
*    Author: Microsoft
*    Date: 2026
*    Code version: ASP.NET Core MVC
*    Availability: https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/filters
***************************************************************************************/

using System.Security.Claims;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace FMCGEnterpriseManagementSystem.Filters
{
    // Records successful user actions (saves, deletes, exports, ...) in the Recent Activity log.
    // Runs for every controller action; only the actions listed below are recorded.
    public class ActivityLogFilter : IAsyncActionFilter
    {
        // "Controller.Action" -> (area shown on the Recent Activity page, what happened)
        private static readonly Dictionary<string, (string Category, string Description)> TrackedActions =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["Account.Logout"] = ("Account", "Logged out"),
                ["Profile.Edit"] = ("Account", "Updated my details"),
                ["Profile.ChangePassword"] = ("Account", "Changed my password"),
                ["Settings.Index"] = ("Settings", "Updated company settings"),

                ["Customers.Create"] = ("Customers", "Added a customer"),
                ["Customers.Update"] = ("Customers", "Updated a customer"),
                ["Customers.Delete"] = ("Customers", "Deleted a customer"),

                ["Supplier.AddSupplier"] = ("Suppliers", "Added a supplier"),
                ["Supplier.EditSupplier"] = ("Suppliers", "Updated a supplier"),
                ["Supplier.DeactivateSupplier"] = ("Suppliers", "Deactivated a supplier"),
                ["Supplier.ActivateSupplier"] = ("Suppliers", "Activated a supplier"),
                ["Supplier.DeleteSupplierConfirmed"] = ("Suppliers", "Deleted a supplier"),

                ["Products.Create"] = ("Products", "Added a product"),
                ["Products.Edit"] = ("Products", "Updated a product"),
                ["Products.Delete"] = ("Products", "Deactivated a product"),   // POST DeleteConfirmed is routed as "Delete"
                ["Products.Activate"] = ("Products", "Activated a product"),

                ["Inventory.AdjustStock"] = ("Inventory", "Adjusted stock"),
                ["Forecasting.Reorder"] = ("Inventory", "Placed a stock reorder"),
                ["Forecasting.ExportCsv"] = ("Inventory", "Exported the stock forecast"),

                ["Employees.Create"] = ("Employees", "Added an employee"),
                ["Employees.Edit"] = ("Employees", "Updated an employee"),
                ["Employees.Deactivate"] = ("Employees", "Deactivated an employee"),

                ["SalesRepresentatives.Create"] = ("Sales Reps", "Added a sales representative"),
                ["SalesRepresentatives.Edit"] = ("Sales Reps", "Updated a sales representative"),
                ["SalesRepresentatives.Deactivate"] = ("Sales Reps", "Deactivated a sales representative"),

                ["UserAccounts.Create"] = ("User Accounts", "Created a user account"),
                ["UserAccounts.Activate"] = ("User Accounts", "Activated a user account"),
                ["UserAccounts.Deactivate"] = ("User Accounts", "Deactivated a user account"),

                ["Quotes.Create"] = ("Quotes", "Created a quote"),
                ["Quotes.Delete"] = ("Quotes", "Deleted a quote"),

                ["Invoices.Create"] = ("Invoices", "Created an invoice"),
                ["Invoices.UpdateStatus"] = ("Invoices", "Changed an invoice status"),
                ["Invoices.Delete"] = ("Invoices", "Deleted an invoice"),        // POST DeleteConfirmed is routed as "Delete"
                ["Invoices.Download"] = ("Invoices", "Downloaded an invoice"),

                ["Payments.Create"] = ("Payments", "Recorded a payment"),
                ["Payments.ExportPdf"] = ("Payments", "Exported payments (PDF)"),
                ["Payments.ExportExcel"] = ("Payments", "Exported payments (Excel)"),

                ["Notifications.MarkAsRead"] = ("Notifications", "Marked a notification as read"),

                ["Reports.Export"] = ("Reports", "Exported a report"),
            };

        private readonly IActivityService _activityService;
        private readonly ITempDataDictionaryFactory _tempDataFactory;

        public ActivityLogFilter(IActivityService activityService, ITempDataDictionaryFactory tempDataFactory)
        {
            _activityService = activityService;
            _tempDataFactory = tempDataFactory;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var executed = await next();

            if (context.ActionDescriptor is not ControllerActionDescriptor action ||
                !TrackedActions.TryGetValue($"{action.ControllerName}.{action.ActionName}", out var activity))
            {
                return;
            }

            var user = context.HttpContext.User;
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null || !Succeeded(context, executed))
            {
                return;
            }

            await _activityService.LogAsync(
                userId,
                user.Identity?.Name,
                activity.Category,
                activity.Description,
                GetDetails(context, action));
        }

        // Only record actions that actually worked
        private bool Succeeded(ActionExecutingContext context, ActionExecutedContext executed)
        {
            if (executed.Exception != null && !executed.ExceptionHandled)
            {
                return false;
            }

            // The action reported a problem (e.g. "Cannot deactivate your own account")
            var tempData = _tempDataFactory.GetTempData(context.HttpContext);
            if (tempData.Peek("ErrorMessage") != null)
            {
                return false;
            }

            return executed.Result switch
            {
                // Saves redirect afterwards; a re-shown form means validation failed
                RedirectToActionResult or RedirectResult or LocalRedirectResult or RedirectToRouteResult => true,

                // Exports and downloads
                FileResult => true,

                // API actions (customers)
                ObjectResult { StatusCode: null or (>= 200 and < 300) } => true,
                StatusCodeResult { StatusCode: >= 200 and < 300 } => true,

                _ => false
            };
        }

        private string? GetDetails(ActionExecutingContext context, ControllerActionDescriptor action)
        {
            var request = context.HttpContext.Request;

            // Which report / format was exported
            if (action.ControllerName == "Reports" && action.ActionName == "Export")
            {
                // "SalesReport" -> "Sales Report"
                var report = System.Text.RegularExpressions.Regex.Replace(
                    request.Query["report"].ToString(), "(?<=[a-z])(?=[A-Z])", " ");
                var format = request.Query["format"].ToString().ToUpperInvariant();
                var period = request.Query.ContainsKey("startDate") || request.Query.ContainsKey("endDate")
                    ? $" ({request.Query["startDate"]} to {request.Query["endDate"]})"
                    : string.Empty;

                return $"{report} as {format}{period}";
            }

            // Use the page's own success message when it has one, e.g. "Quote QT00003 created successfully."
            var tempData = _tempDataFactory.GetTempData(context.HttpContext);
            if (tempData.Peek("SuccessMessage") is string message && !string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            // Otherwise the record's id, if the URL has one
            return context.RouteData.Values.TryGetValue("id", out var id) && id != null
                ? $"Record #{id}"
                : null;
        }
    }
}
