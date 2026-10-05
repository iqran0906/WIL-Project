// Title: Asynchronous programming with async and await
// Author: Microsoft
// Date: 01-10-2026
// Code version: C# / .NET 10
// Availability: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

using System.Globalization;
using System.Security.Claims;
using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FMCGEnterpriseManagementSystem.Services
{
    // Provides global search functionality across multiple areas
    // of the FMCG Enterprise Management System.
    public class SearchService : ISearchService
    {
        // Maximum number of results returned for each category.
        private const int MaxPerCategory = 10;

        // Provides access to the application's database.
        private readonly ApplicationDbContext _context;

        // Constructor receives the database context through dependency injection.
        public SearchService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Searches the system based on the supplied query and the
        // permissions of the currently logged-in user.
        public async Task<GlobalSearchViewModel> SearchAsync(
            string query,
            ClaimsPrincipal user)
        {
            // Creates the search result model and stores the cleaned query.
            var model = new GlobalSearchViewModel
            {
                Query = query?.Trim() ?? string.Empty
            };

            // If the search box is empty, return an empty result model.
            if (string.IsNullOrWhiteSpace(model.Query))
            {
                return model;
            }

            // Stores the cleaned search term for use in the database queries.
            var term = model.Query;

            // A numeric search term could represent a customer number,
            // payment number, or the numeric part of an invoice number.
            var isNumber = int.TryParse(
                term,
                NumberStyles.None,
                CultureInfo.InvariantCulture,
                out var number);

            // Converts a numeric search value such as 12 into
            // the invoice format ED00012.
            var paddedInvoiceNumber = isNumber
                ? $"ED{number:D5}"
                : null;

            // Determines the roles of the current user.
            var isAdministrator = user.IsInRole("Administrator");
            var isEmployee = user.IsInRole("Employee");
            var isSalesRepresentative = user.IsInRole("SalesRepresentative");

            // Operations data is available to administrators and employees.
            var canUseOperations = isAdministrator || isEmployee;

            // Sales data is available to administrators, employees
            // and sales representatives.
            var canUseSales =
                isAdministrator ||
                isEmployee ||
                isSalesRepresentative;


            // ==================================================
            // CUSTOMERS
            // ==================================================

            // Customer information is only searched when the user
            // has permission to access sales functionality.
            if (canUseSales)
            {
                // Searches customers using several possible fields.
                var customers = await _context.Customers
                    .AsNoTracking()
                    .Where(c =>
                        // Searches by customer number when the term is numeric.
                        (isNumber && c.CustomerId == number) ||

                        // Searches by customer name and surname.
                        c.Name.Contains(term) ||
                        c.Surname.Contains(term) ||
                        (c.Name + " " + c.Surname).Contains(term) ||

                        // Searches customer identification/contact information.
                        c.IdNumber.Contains(term) ||
                        c.Email.Contains(term) ||
                        c.CellNumber.Contains(term) ||
                        c.TelephoneNumber.Contains(term) ||

                        // Searches by VAT number where one exists.
                        (c.VATNumber != null && c.VATNumber.Contains(term)))

                    // Sorts customer results alphabetically.
                    .OrderBy(c => c.Name)
                    .ThenBy(c => c.Surname)

                    // Limits the number of returned customer results.
                    .Take(MaxPerCategory)

                    // Executes the query asynchronously.
                    .ToListAsync();

                // Converts each customer into a global search result.
                foreach (var c in customers)
                {
                    model.Results.Add(new SearchResultItem
                    {
                        // Identifies the category displayed in the search results.
                        Category = "Customers",

                        // Bootstrap icon used for customer results.
                        Icon = "bi-people",

                        // Displays the customer's full name.
                        Title = $"{c.Name} {c.Surname}",

                        // Displays the customer number and active/inactive status.
                        Subtitle = $"Customer No. {c.CustomerId}"
                            + (c.IsActive ? string.Empty : " (Inactive)"),

                        // Provides additional customer information.
                        Details = new Dictionary<string, string>
                        {
                            ["ID Number"] = c.IdNumber,
                            ["Cell"] = c.CellNumber,
                            ["Telephone"] = c.TelephoneNumber,
                            ["Email"] = c.Email,
                            ["Group"] = c.CustomerGroup,
                            ["Payment Terms"] = c.PaymentTerms,
                            ["Delivery Address"] = c.DeliveryAddress
                        }

                        // Removes fields that do not contain values.
                        .Where(d => !string.IsNullOrWhiteSpace(d.Value))

                        // Converts the filtered collection back into a dictionary.
                        .ToDictionary(d => d.Key, d => d.Value),

                        // Defines where the search result would navigate.
                        Controller = "Invoices",
                        Action = "Index",

                        // Passes the customer ID to the destination.
                        RouteValues = new Dictionary<string, string>
                        {
                            ["customerId"] = c.CustomerId.ToString()
                        },

                        LinkText = "View invoices",

                        // A numeric customer number is considered an exact match.
                        IsExactMatch = isNumber && c.CustomerId == number,

                        // Prevents direct redirection for this result.
                        CanRedirect = false
                    });
                }
            }


            // ==================================================
            // INVOICES
            // ==================================================

            // Invoice searches are available to users with sales permissions.
            if (canUseSales)
            {
                // Searches invoices by invoice number, numeric invoice number,
                // business name, or customer name.
                var invoices = await _context.Invoices
                    .AsNoTracking()

                    // Includes the related customer information.
                    .Include(i => i.Customer)

                    .Where(i =>
                        i.InvoiceNumber.Contains(term) ||
                        (paddedInvoiceNumber != null &&
                         i.InvoiceNumber == paddedInvoiceNumber) ||
                        (i.BusinessName != null &&
                         i.BusinessName.Contains(term)) ||
                        (i.Customer.Name + " " + i.Customer.Surname)
                            .Contains(term))

                    // Shows the newest invoices first.
                    .OrderByDescending(i => i.InvoiceDate)

                    // Limits the number of invoice results.
                    .Take(MaxPerCategory)

                    // Executes the database query asynchronously.
                    .ToListAsync();

                // Converts invoice records into search result items.
                foreach (var i in invoices)
                {
                    model.Results.Add(new SearchResultItem
                    {
                        Category = "Invoices",
                        Icon = "bi-receipt",

                        // Displays the invoice number.
                        Title = $"Invoice {i.InvoiceNumber}",

                        // Displays customer, date, total and status.
                        Subtitle = $"{i.Customer.Name} {i.Customer.Surname} · "
                            + $"{i.InvoiceDate:dd MMM yyyy} · "
                            + $"{i.Total.ToString(
                                "C",
                                CultureInfo.GetCultureInfo("en-ZA"))} · "
                            + $"{i.Status}",

                        Controller = "Invoices",
                        Action = "Details",

                        // Passes the invoice ID to the details page.
                        RouteValues = new Dictionary<string, string>
                        {
                            ["id"] = i.InvoiceId.ToString()
                        },

                        LinkText = "Open invoice",

                        // Determines whether the entered value exactly
                        // matches the invoice number.
                        IsExactMatch =
                            string.Equals(
                                i.InvoiceNumber,
                                term,
                                StringComparison.OrdinalIgnoreCase) ||
                            i.InvoiceNumber == paddedInvoiceNumber
                    });
                }
            }


            // ==================================================
            // QUOTES
            // ==================================================

            // Quotes are only searched when the user can access sales data.
            if (canUseSales)
            {
                // Searches quotes by quote number or customer name.
                var quotes = await _context.Quotes
                    .AsNoTracking()
                    .Include(q => q.Customer)
                    .Where(q =>
                        q.QuoteNumber.Contains(term) ||
                        (q.Customer.Name + " " + q.Customer.Surname)
                            .Contains(term))

                    // Shows the newest quotes first.
                    .OrderByDescending(q => q.QuoteDate)

                    // Limits the number of quote results.
                    .Take(MaxPerCategory)

                    // Executes the query asynchronously.
                    .ToListAsync();

                // Converts each quote into a search result.
                foreach (var q in quotes)
                {
                    model.Results.Add(new SearchResultItem
                    {
                        Category = "Quotes",
                        Icon = "bi-file-earmark-text",

                        // Displays quote number and customer.
                        Title = $"Quote {q.QuoteNumber}",
                        Subtitle = $"{q.Customer.Name} {q.Customer.Surname} · "
                            + $"{q.QuoteDate:dd MMM yyyy} · {q.Status}",

                        // There is no quote details page yet,
                        // so the result opens the main quotes page.
                        Controller = "Quotes",
                        Action = "Index",
                        LinkText = "Go to quotes",

                        // Checks whether the search term exactly matches
                        // the quote number.
                        IsExactMatch = string.Equals(
                            q.QuoteNumber,
                            term,
                            StringComparison.OrdinalIgnoreCase)
                    });
                }
            }


            // ==================================================
            // PAYMENTS (by payment number)
            // ==================================================

            // Payment searches only occur when the search term is numeric
            // and the user has sales permissions.
            if (canUseSales && isNumber)
            {
                // Finds the payment using its numeric payment ID.
                var payment = await _context.Payments
                    .AsNoTracking()

                    // Includes the invoice associated with the payment.
                    .Include(p => p.Invoice)

                    // Retrieves the first matching payment.
                    .FirstOrDefaultAsync(p => p.PaymentId == number);

                // Only creates a result when a matching payment exists.
                if (payment != null)
                {
                    model.Results.Add(new SearchResultItem
                    {
                        Category = "Payments",
                        Icon = "bi-cash-coin",

                        // Displays the payment number.
                        Title = $"Payment No. {payment.PaymentId}",

                        // Displays invoice number, payment date and amount.
                        Subtitle = $"Invoice {payment.Invoice.InvoiceNumber} · "
                            + $"{payment.PaymentDate:dd MMM yyyy} · "
                            + $"{payment.AmountPaid.ToString(
                                "C",
                                CultureInfo.GetCultureInfo("en-ZA"))}",

                        Controller = "Payments",
                        Action = "View",

                        // Passes the payment ID to the payment page.
                        RouteValues = new Dictionary<string, string>
                        {
                            ["id"] = payment.PaymentId.ToString()
                        },

                        LinkText = "View receipt",

                        // A payment found by its ID is an exact match.
                        IsExactMatch = true
                    });
                }
            }


            // ==================================================
            // PRODUCTS / ITEMS
            // ==================================================

            // Product searches require operations permissions.
            if (canUseOperations)
            {
                // Searches products using the product code or product name.
                var products = await _context.Products
                    .AsNoTracking()

                    // Includes current inventory information.
                    .Include(p => p.Inventory)

                    .Where(p =>
                        p.ProductCode.Contains(term) ||
                        p.ProductName.Contains(term))

                    // Sorts products alphabetically.
                    .OrderBy(p => p.ProductName)

                    // Limits the number of results.
                    .Take(MaxPerCategory)

                    // Executes the query asynchronously.
                    .ToListAsync();

                // Converts products into global search results.
                foreach (var p in products)
                {
                    // Displays the current stock quantity when inventory exists.
                    var stock = p.Inventory != null
                        ? $" · {p.Inventory.QuantityOnHand} in stock"
                        : string.Empty;

                    model.Results.Add(new SearchResultItem
                    {
                        Category = "Products",
                        Icon = "bi-box-seam",

                        // Displays the product name.
                        Title = p.ProductName,

                        // Displays product code, category, stock
                        // and active/inactive status.
                        Subtitle = $"Item Code {p.ProductCode} · "
                            + $"{p.Category}{stock}"
                            + (p.IsActive ? string.Empty : " (Inactive)"),

                        Controller = "Products",
                        Action = "Index",

                        // Uses the product code as the search parameter.
                        RouteValues = new Dictionary<string, string>
                        {
                            ["search"] = p.ProductCode
                        },

                        LinkText = "View product",

                        // Checks whether the search term exactly matches
                        // the product code.
                        IsExactMatch = string.Equals(
                            p.ProductCode,
                            term,
                            StringComparison.OrdinalIgnoreCase)
                    });
                }
            }


            // ==================================================
            // SUPPLIERS
            // ==================================================

            // Supplier information is available to operations users.
            if (canUseOperations)
            {
                // Searches suppliers by ID, company details,
                // contact details and VAT number.
                var suppliers = await _context.Suppliers
                    .AsNoTracking()
                    .Where(s =>
                        (isNumber && s.SupplierId == number) ||
                        s.CompanyName.Contains(term) ||
                        s.ContactPerson.Contains(term) ||
                        s.ContactNumber.Contains(term) ||
                        s.Email.Contains(term) ||
                        s.VATNumber.Contains(term))

                    // Sorts suppliers alphabetically.
                    .OrderBy(s => s.CompanyName)

                    // Limits the returned results.
                    .Take(MaxPerCategory)

                    // Executes the query asynchronously.
                    .ToListAsync();

                // Converts supplier records into search results.
                foreach (var s in suppliers)
                {
                    model.Results.Add(new SearchResultItem
                    {
                        Category = "Suppliers",
                        Icon = "bi-truck",

                        // Displays supplier name.
                        Title = s.CompanyName,

                        // Displays supplier number and contact details.
                        Subtitle = $"Supplier No. {s.SupplierId} · "
                            + $"{s.ContactPerson} · {s.ContactNumber}"
                            + (s.IsActive ? string.Empty : " (Inactive)"),

                        Controller = "Supplier",
                        Action = "Products",

                        // Passes the supplier ID to the destination.
                        RouteValues = new Dictionary<string, string>
                        {
                            ["id"] = s.SupplierId.ToString()
                        },

                        LinkText = "View supplier products",

                        // Treats a matching VAT number as an exact match.
                        IsExactMatch =
                            string.Equals(
                                s.VATNumber,
                                term,
                                StringComparison.OrdinalIgnoreCase)
                    });
                }
            }


            // ==================================================
            // EMPLOYEES & SALES REPS (administrators only)
            // ==================================================

            // Employee and sales representative information is restricted
            // to administrators.
            if (isAdministrator)
            {
                // Searches employees by employee number,
                // name, surname or email.
                var employees = await _context.Employees
                    .AsNoTracking()
                    .Where(e =>
                        e.EmployeeNumber.Contains(term) ||
                        e.FirstName.Contains(term) ||
                        e.LastName.Contains(term) ||
                        (e.FirstName + " " + e.LastName).Contains(term) ||
                        e.Email.Contains(term))

                    // Sorts employees alphabetically.
                    .OrderBy(e => e.FirstName)
                    .ThenBy(e => e.LastName)

                    // Limits the number of results.
                    .Take(MaxPerCategory)

                    // Executes the query asynchronously.
                    .ToListAsync();

                // Converts employees into search results.
                foreach (var e in employees)
                {
                    model.Results.Add(new SearchResultItem
                    {
                        Category = "Employees",
                        Icon = "bi-person-badge",

                        // Displays employee name.
                        Title = $"{e.FirstName} {e.LastName}",

                        // Displays employee number, job title
                        // and active/inactive status.
                        Subtitle = $"Employee No. {e.EmployeeNumber} · "
                            + $"{e.JobTitle}"
                            + (e.IsActive ? string.Empty : " (Inactive)"),

                        Controller = "Employees",
                        Action = "Index",

                        // Uses employee number as the search parameter.
                        RouteValues = new Dictionary<string, string>
                        {
                            ["keyword"] = e.EmployeeNumber
                        },

                        LinkText = "View employee",

                        // Determines whether the employee number
                        // exactly matches the search term.
                        IsExactMatch = string.Equals(
                            e.EmployeeNumber,
                            term,
                            StringComparison.OrdinalIgnoreCase)
                    });
                }

                // Searches sales representatives by rep code,
                // area or employee name.
                var salesReps = await _context.SalesRepresentatives
                    .AsNoTracking()

                    // Loads the employee associated with the sales representative.
                    .Include(sr => sr.Employee)

                    .Where(sr =>
                        sr.SalesRepCode.Contains(term) ||
                        (sr.Area != null && sr.Area.Contains(term)) ||
                        (sr.Employee.FirstName + " " + sr.Employee.LastName)
                            .Contains(term))

                    // Sorts results by sales representative code.
                    .OrderBy(sr => sr.SalesRepCode)

                    // Limits the number of returned results.
                    .Take(MaxPerCategory)

                    // Executes the query asynchronously.
                    .ToListAsync();

                // Converts sales representatives into search results.
                foreach (var sr in salesReps)
                {
                    model.Results.Add(new SearchResultItem
                    {
                        Category = "Sales Representatives",
                        Icon = "bi-briefcase",

                        // Displays the representative's employee name.
                        Title = $"{sr.Employee.FirstName} {sr.Employee.LastName}",

                        // Displays the representative code, area
                        // and active/inactive status.
                        Subtitle = $"Rep Code {sr.SalesRepCode}"
                            + (string.IsNullOrWhiteSpace(sr.Area)
                                ? string.Empty
                                : $" · {sr.Area}")
                            + (sr.IsActive ? string.Empty : " (Inactive)"),

                        Controller = "SalesRepresentatives",
                        Action = "Edit",

                        // Passes the sales representative ID.
                        RouteValues = new Dictionary<string, string>
                        {
                            ["id"] = sr.SalesRepresentativeId.ToString()
                        },

                        LinkText = "Open sales rep",

                        // Checks whether the search term exactly matches
                        // the sales representative code.
                        IsExactMatch = string.Equals(
                            sr.SalesRepCode,
                            term,
                            StringComparison.OrdinalIgnoreCase)
                    });
                }
            }

            // Returns all matching search results.
            return model;
        }
    }
}