// Purpose: Text of the short tip pop-up shown when each page opens.
// Authors: ST10068525 (new file, not yet committed)

namespace FMCGEnterpriseManagementSystem.Helpers
{
    // Short "what you can do here" messages shown in the pop-up
    // when a page opens. Keyed by the view file that was rendered.
    public static class PageMessageHelper
    {
        private static readonly Dictionary<string, string> Messages =
            new(StringComparer.OrdinalIgnoreCase)
            {
                // ACCOUNT
                ["/Views/Account/AccessDenied.cshtml"] =
                    "You do not have permission to view that page. Return to the dashboard to continue.",

                ["/Views/Shared/Error.cshtml"] =
                    "Use Go Back or Dashboard to continue working.",

                // HOME / DASHBOARD
                ["/Views/Home/Dashboard.cshtml"] =
                    "Welcome to your dashboard. Use the quick links to create customers, items, suppliers, quotes and invoices.",
                ["/Views/Home/Index.cshtml"] =
                    "Welcome to Exclusive Distributors. Use the menu on the left to get started.",
                ["/Views/Home/Privacy.cshtml"] =
                    "Here you can read how the system handles and protects your data.",
                ["/Views/Settings/Index.cshtml"] =
                    "Add or update the company profile and banking details, and set your own preferences.",
                ["/Views/Home/UserProfile.cshtml"] =
                    "Here you can view your profile. Click Edit My Details to update your information.",
                ["/Views/Profile/Edit.cshtml"] =
                    "You can now update your name, email and contact number. Click Save Changes when done.",
                ["/Views/Profile/ChangePassword.cshtml"] =
                    "Enter your current password, then choose and confirm a new password.",
                ["/Views/Home/Reports.cshtml"] =
                    "Choose a report to view sales, stock, customer and payment information.",

                // CUSTOMERS
                ["/Views/Home/CustomerList.cshtml"] =
                    "Here you can view all customers. Click New Customer to add one.",
                ["/Views/Home/AddCustomer.cshtml"] =
                    "You can now add a new customer by filling in the form and clicking Save Customer.",

                // INVENTORY
                ["/Views/Home/InventoryList.cshtml"] =
                    "Here you can view all stock items and their quantities. Click New Item to add one.",
                ["/Views/Home/AddItem.cshtml"] =
                    "You can now add a new stock item by filling in the form and clicking Save Item.",

                // EMPLOYEES
                ["/Views/Home/EmployeeList.cshtml"] =
                    "Here you can view all employees.",
                ["/Views/Employees/Index.cshtml"] =
                    "Here you can view, edit and deactivate employees, or add a new employee.",
                ["/Views/Employees/Create.cshtml"] =
                    "You can now add a new employee by filling in the form.",
                ["/Views/Employees/Edit.cshtml"] =
                    "You can now update this employee's details. Save when you are done.",

                // SALES REPRESENTATIVES
                ["/Views/Home/AddSalesRep.cshtml"] =
                    "You can now add a new sales rep by filling in the form and clicking Save Sales Rep.",
                ["/Views/SalesRepresentatives/Index.cshtml"] =
                    "Here you can view, edit and deactivate sales representatives, or add a new one.",
                ["/Views/SalesRepresentatives/Create.cshtml"] =
                    "You can now add a new sales representative by filling in the form.",
                ["/Views/SalesRepresentatives/Edit.cshtml"] =
                    "You can now update this sales representative's details. Save when you are done.",

                // SUPPLIERS
                ["/Views/Supplier/SupplierList.cshtml"] =
                    "Here you can view, edit, activate or deactivate suppliers, and see the products they supply.",
                ["/Views/Supplier/AddSupplier.cshtml"] =
                    "You can now add a new supplier by filling in the form and clicking Save Supplier.",
                ["/Views/Supplier/EditSupplier.cshtml"] =
                    "You can now update this supplier's details. Click Update Supplier to save.",
                ["/Views/Supplier/DeleteSupplier.cshtml"] =
                    "Check the supplier details below, then confirm if you want to delete this supplier.",
                ["/Views/Supplier/Products.cshtml"] =
                    "Here you can view the products supplied by this supplier.",

                // PRODUCTS
                ["/Views/Products/Index.cshtml"] =
                    "Here you can view, edit, activate or deactivate products, or create a new product.",
                ["/Views/Products/Create.cshtml"] =
                    "You can now add a new product by filling in the form.",
                ["/Views/Products/Edit.cshtml"] =
                    "You can now update this product's details. Save when you are done.",
                ["/Views/Products/Delete.cshtml"] =
                    "Check the product details, then confirm if you want to deactivate this product.",

                // FORECASTING
                ["/Views/Forecasting/Index.cshtml"] =
                    "Here you can see predicted demand and stock levels, reorder low stock, or export the forecast.",
                ["/Views/Forecasting/Reorder.cshtml"] =
                    "You can now reorder stock. Check the quantity and click Confirm & Order Stock.",

                // QUOTES
                ["/Views/Home/QuoteList.cshtml"] =
                    "Here you can view all quotes. Click New Quote to create one.",
                ["/Views/Home/CreateQuote.cshtml"] =
                    "You can now create a quote by choosing a customer, adding items and clicking Create Quote.",
                ["/Views/Quotes/Index.cshtml"] =
                    "Here you can view all quotes and their details, or create a new quote.",
                ["/Views/Quotes/Create.cshtml"] =
                    "You can now create a quote by choosing a customer, adding items and clicking Create Quote.",

                // INVOICES
                ["/Views/Home/InvoiceList.cshtml"] =
                    "Here you can view all invoices. Click New Invoice to create one.",
                ["/Views/Home/CreateInvoice.cshtml"] =
                    "You can now create an invoice by filling in the form. Preview it, then click Save Invoice.",
                ["/Views/Invoices/Index.cshtml"] =
                    "Here you can filter, view, download and delete invoices, or create a new invoice.",
                ["/Views/Invoices/Create.cshtml"] =
                    "You can now create an invoice by filling in the form and clicking Save Invoice.",
                ["/Views/Invoices/Details.cshtml"] =
                    "Here you can preview this invoice and its line items.",
                ["/Views/Invoices/Delete.cshtml"] =
                    "Check the invoice details, then confirm if you want to delete this invoice.",

                // PAYMENTS
                ["/Views/Payments/Index.cshtml"] =
                    "Here you can view all payments, record a new payment, or export to Excel or PDF.",
                ["/Views/Payments/Create.cshtml"] =
                    "You can now record a payment by selecting the invoice and entering the amount received.",
                ["/Views/Payments/View.cshtml"] =
                    "Here you can view the receipt for this payment.",

                // SEARCH
                ["/Views/Search/Index.cshtml"] =
                    "Here are the records that match your search. Click a result to open it.",

                // RECENT ACTIVITY
                ["/Views/Activity/Index.cshtml"] =
                    "Here you can see everything you have done in the system, newest first.",

                // NOTIFICATIONS
                ["/Views/Notifications/Index.cshtml"] =
                    "Here you can view your notifications and mark them as read.",

                // USER ACCOUNTS
                ["/Views/UserAccounts/Index.cshtml"] =
                    "Here you can manage user accounts: create, activate or deactivate users.",
                ["/Views/UserAccounts/Create.cshtml"] =
                    "You can now create a new user account by filling in the form and choosing a role.",

                // REPORTS
                ["/Views/Reports/Index.cshtml"] =
                    "Choose a report to view sales, stock, customer and payment information.",
                ["/Views/Reports/AgeAnalysis.cshtml"] =
                    "Here you can see outstanding customer balances grouped by how long they have been unpaid.",
                ["/Views/Reports/CustomerReport.cshtml"] =
                    "Here you can view a summary of all customers.",
                ["/Views/Reports/CustomerSales.cshtml"] =
                    "Here you can view sales per customer. Use the filter to narrow the results.",
                ["/Views/Reports/InventoryReport.cshtml"] =
                    "Here you can view current stock levels for all items.",
                ["/Views/Reports/InvoiceReport.cshtml"] =
                    "Here you can view invoices for a period. Use the filter to narrow the results.",
                ["/Views/Reports/ItemSalesReport.cshtml"] =
                    "Here you can see how much of each item has been sold. Use the filter to narrow the results.",
                ["/Views/Reports/ItemsPerCustomer.cshtml"] =
                    "Here you can see which items each customer has bought. Use the filter to narrow the results.",
                ["/Views/Reports/PaymentsReport.cshtml"] =
                    "Here you can view payments received. Use the filter to narrow the results.",
                ["/Views/Reports/QuoteReport.cshtml"] =
                    "Here you can view quotes for a period. Use the filter to narrow the results.",
                ["/Views/Reports/SalesRepReport.cshtml"] =
                    "Here you can view sales by sales representative. Use the filter to narrow the results.",
                ["/Views/Reports/SalesReport.cshtml"] =
                    "Here you can view total sales for a period. Use the filter to narrow the results.",
                ["/Views/Reports/SalesVatReport.cshtml"] =
                    "Here you can view sales and the VAT charged. Use the filter to narrow the results.",
            };

        public static string GetMessage(string? viewPath, string? pageTitle)
        {
            if (!string.IsNullOrWhiteSpace(viewPath)
                && Messages.TryGetValue(viewPath, out var message))
            {
                return message;
            }

            return string.IsNullOrWhiteSpace(pageTitle)
                ? "Welcome to Exclusive Distributors"
                : $"You are now viewing {pageTitle}";
        }
    }
}
