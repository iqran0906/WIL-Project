

using System.Globalization;
using System.Security.Claims;
using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FMCGEnterpriseManagementSystem.Services
{
    public class SearchService : ISearchService
    {
        // Maximum results returned per record type
        private const int MaxPerCategory = 10;

        private readonly ApplicationDbContext _context;

        public SearchService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<GlobalSearchViewModel> SearchAsync(string query, ClaimsPrincipal user)
        {
            var model = new GlobalSearchViewModel
            {
                Query = query?.Trim() ?? string.Empty
            };

            if (string.IsNullOrWhiteSpace(model.Query))
            {
                return model;
            }

            var term = model.Query;

            // A plain number could be a customer number, payment number
            // or the numeric part of an invoice number (ED00012 -> 12)
            var isNumber = int.TryParse(
                term,
                NumberStyles.None,
                CultureInfo.InvariantCulture,
                out var number);

            var paddedInvoiceNumber = isNumber
                ? $"ED{number:D5}"
                : null;

            // Same role rules as the controllers and sidebar
            var isAdministrator = user.IsInRole("Administrator");
            var isEmployee = user.IsInRole("Employee");
            var isSalesRepresentative = user.IsInRole("SalesRepresentative");

            var canUseOperations = isAdministrator || isEmployee;
            var canUseSales = isAdministrator || isEmployee || isSalesRepresentative;


            // ==================================================
            // CUSTOMERS
            // ==================================================

            if (canUseSales)
            {
                var customers = await _context.Customers
                    .AsNoTracking()
                    .Where(c =>
                        (isNumber && c.CustomerId == number) ||
                        c.Name.Contains(term) ||
                        c.Surname.Contains(term) ||
                        (c.Name + " " + c.Surname).Contains(term) ||
                        c.IdNumber.Contains(term) ||
                        c.Email.Contains(term) ||
                        c.CellNumber.Contains(term) ||
                        c.TelephoneNumber.Contains(term) ||
                        (c.VATNumber != null && c.VATNumber.Contains(term)))
                    .OrderBy(c => c.Name)
                    .ThenBy(c => c.Surname)
                    .Take(MaxPerCategory)
                    .ToListAsync();

                foreach (var c in customers)
                {
                    model.Results.Add(new SearchResultItem
                    {
                        Category = "Customers",
                        Icon = "bi-people",
                        Title = $"{c.Name} {c.Surname}",
                        Subtitle = $"Customer No. {c.CustomerId}"
                            + (c.IsActive ? string.Empty : " (Inactive)"),
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
                        .Where(d => !string.IsNullOrWhiteSpace(d.Value))
                        .ToDictionary(d => d.Key, d => d.Value),
                        Controller = "Invoices",
                        Action = "Index",
                        RouteValues = new Dictionary<string, string>
                        {
                            ["customerId"] = c.CustomerId.ToString()
                        },
                        LinkText = "View invoices",
                        IsExactMatch = isNumber && c.CustomerId == number,
                        CanRedirect = false
                    });
                }
            }


            // ==================================================
            // INVOICES
            // ==================================================

            if (canUseSales)
            {
                var invoices = await _context.Invoices
                    .AsNoTracking()
                    .Include(i => i.Customer)
                    .Where(i =>
                        i.InvoiceNumber.Contains(term) ||
                        (paddedInvoiceNumber != null && i.InvoiceNumber == paddedInvoiceNumber) ||
                        (i.BusinessName != null && i.BusinessName.Contains(term)) ||
                        (i.Customer.Name + " " + i.Customer.Surname).Contains(term))
                    .OrderByDescending(i => i.InvoiceDate)
                    .Take(MaxPerCategory)
                    .ToListAsync();

                foreach (var i in invoices)
                {
                    model.Results.Add(new SearchResultItem
                    {
                        Category = "Invoices",
                        Icon = "bi-receipt",
                        Title = $"Invoice {i.InvoiceNumber}",
                        Subtitle = $"{i.Customer.Name} {i.Customer.Surname} · "
                            + $"{i.InvoiceDate:dd MMM yyyy} · "
                            + $"{i.Total.ToString("C", CultureInfo.GetCultureInfo("en-ZA"))} · {i.Status}",
                        Controller = "Invoices",
                        Action = "Details",
                        RouteValues = new Dictionary<string, string>
                        {
                            ["id"] = i.InvoiceId.ToString()
                        },
                        LinkText = "Open invoice",
                        IsExactMatch =
                            string.Equals(i.InvoiceNumber, term, StringComparison.OrdinalIgnoreCase) ||
                            i.InvoiceNumber == paddedInvoiceNumber
                    });
                }
            }


            // ==================================================
            // QUOTES
            // ==================================================

            if (canUseSales)
            {
                var quotes = await _context.Quotes
                    .AsNoTracking()
                    .Include(q => q.Customer)
                    .Where(q =>
                        q.QuoteNumber.Contains(term) ||
                        (q.Customer.Name + " " + q.Customer.Surname).Contains(term))
                    .OrderByDescending(q => q.QuoteDate)
                    .Take(MaxPerCategory)
                    .ToListAsync();

                foreach (var q in quotes)
                {
                    model.Results.Add(new SearchResultItem
                    {
                        Category = "Quotes",
                        Icon = "bi-file-earmark-text",
                        Title = $"Quote {q.QuoteNumber}",
                        Subtitle = $"{q.Customer.Name} {q.Customer.Surname} · "
                            + $"{q.QuoteDate:dd MMM yyyy} · {q.Status}",
                        // There is no quote details page yet, so open the quotes list
                        Controller = "Quotes",
                        Action = "Index",
                        LinkText = "Go to quotes",
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

            if (canUseSales && isNumber)
            {
                var payment = await _context.Payments
                    .AsNoTracking()
                    .Include(p => p.Invoice)
                    .FirstOrDefaultAsync(p => p.PaymentId == number);

                if (payment != null)
                {
                    model.Results.Add(new SearchResultItem
                    {
                        Category = "Payments",
                        Icon = "bi-cash-coin",
                        Title = $"Payment No. {payment.PaymentId}",
                        Subtitle = $"Invoice {payment.Invoice.InvoiceNumber} · "
                            + $"{payment.PaymentDate:dd MMM yyyy} · "
                            + $"{payment.AmountPaid.ToString("C", CultureInfo.GetCultureInfo("en-ZA"))}",
                        Controller = "Payments",
                        Action = "View",
                        RouteValues = new Dictionary<string, string>
                        {
                            ["id"] = payment.PaymentId.ToString()
                        },
                        LinkText = "View receipt",
                        IsExactMatch = true
                    });
                }
            }


            // ==================================================
            // PRODUCTS / ITEMS
            // ==================================================

            if (canUseOperations)
            {
                var products = await _context.Products
                    .AsNoTracking()
                    .Include(p => p.Inventory)
                    .Where(p =>
                        p.ProductCode.Contains(term) ||
                        p.ProductName.Contains(term))
                    .OrderBy(p => p.ProductName)
                    .Take(MaxPerCategory)
                    .ToListAsync();

                foreach (var p in products)
                {
                    var stock = p.Inventory != null
                        ? $" · {p.Inventory.QuantityOnHand} in stock"
                        : string.Empty;

                    model.Results.Add(new SearchResultItem
                    {
                        Category = "Products",
                        Icon = "bi-box-seam",
                        Title = p.ProductName,
                        Subtitle = $"Item Code {p.ProductCode} · {p.Category}{stock}"
                            + (p.IsActive ? string.Empty : " (Inactive)"),
                        Controller = "Products",
                        Action = "Index",
                        RouteValues = new Dictionary<string, string>
                        {
                            ["search"] = p.ProductCode
                        },
                        LinkText = "View product",
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

            if (canUseOperations)
            {
                var suppliers = await _context.Suppliers
                    .AsNoTracking()
                    .Where(s =>
                        (isNumber && s.SupplierId == number) ||
                        s.CompanyName.Contains(term) ||
                        s.ContactPerson.Contains(term) ||
                        s.ContactNumber.Contains(term) ||
                        s.Email.Contains(term) ||
                        s.VATNumber.Contains(term))
                    .OrderBy(s => s.CompanyName)
                    .Take(MaxPerCategory)
                    .ToListAsync();

                foreach (var s in suppliers)
                {
                    model.Results.Add(new SearchResultItem
                    {
                        Category = "Suppliers",
                        Icon = "bi-truck",
                        Title = s.CompanyName,
                        Subtitle = $"Supplier No. {s.SupplierId} · {s.ContactPerson} · {s.ContactNumber}"
                            + (s.IsActive ? string.Empty : " (Inactive)"),
                        Controller = "Supplier",
                        Action = "Products",
                        RouteValues = new Dictionary<string, string>
                        {
                            ["id"] = s.SupplierId.ToString()
                        },
                        LinkText = "View supplier products",
                        IsExactMatch =
                            string.Equals(s.VATNumber, term, StringComparison.OrdinalIgnoreCase)
                    });
                }
            }


            // ==================================================
            // EMPLOYEES & SALES REPS (administrators only)
            // ==================================================

            if (isAdministrator)
            {
                var employees = await _context.Employees
                    .AsNoTracking()
                    .Where(e =>
                        e.EmployeeNumber.Contains(term) ||
                        e.FirstName.Contains(term) ||
                        e.LastName.Contains(term) ||
                        (e.FirstName + " " + e.LastName).Contains(term) ||
                        e.Email.Contains(term))
                    .OrderBy(e => e.FirstName)
                    .ThenBy(e => e.LastName)
                    .Take(MaxPerCategory)
                    .ToListAsync();

                foreach (var e in employees)
                {
                    model.Results.Add(new SearchResultItem
                    {
                        Category = "Employees",
                        Icon = "bi-person-badge",
                        Title = $"{e.FirstName} {e.LastName}",
                        Subtitle = $"Employee No. {e.EmployeeNumber} · {e.JobTitle}"
                            + (e.IsActive ? string.Empty : " (Inactive)"),
                        Controller = "Employees",
                        Action = "Index",
                        RouteValues = new Dictionary<string, string>
                        {
                            ["keyword"] = e.EmployeeNumber
                        },
                        LinkText = "View employee",
                        IsExactMatch = string.Equals(
                            e.EmployeeNumber,
                            term,
                            StringComparison.OrdinalIgnoreCase)
                    });
                }

                var salesReps = await _context.SalesRepresentatives
                    .AsNoTracking()
                    .Include(sr => sr.Employee)
                    .Where(sr =>
                        sr.SalesRepCode.Contains(term) ||
                        (sr.Area != null && sr.Area.Contains(term)) ||
                        (sr.Employee.FirstName + " " + sr.Employee.LastName).Contains(term))
                    .OrderBy(sr => sr.SalesRepCode)
                    .Take(MaxPerCategory)
                    .ToListAsync();

                foreach (var sr in salesReps)
                {
                    model.Results.Add(new SearchResultItem
                    {
                        Category = "Sales Representatives",
                        Icon = "bi-briefcase",
                        Title = $"{sr.Employee.FirstName} {sr.Employee.LastName}",
                        Subtitle = $"Rep Code {sr.SalesRepCode}"
                            + (string.IsNullOrWhiteSpace(sr.Area) ? string.Empty : $" · {sr.Area}")
                            + (sr.IsActive ? string.Empty : " (Inactive)"),
                        Controller = "SalesRepresentatives",
                        Action = "Edit",
                        RouteValues = new Dictionary<string, string>
                        {
                            ["id"] = sr.SalesRepresentativeId.ToString()
                        },
                        LinkText = "Open sales rep",
                        IsExactMatch = string.Equals(
                            sr.SalesRepCode,
                            term,
                            StringComparison.OrdinalIgnoreCase)
                    });
                }
            }

            return model;
        }
    }
}
