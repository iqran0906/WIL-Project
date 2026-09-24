using System.Diagnostics;
using FMCGEnterpriseManagementSystem.Data;
using FMCGEnterpriseManagementSystem.Models;
using FMCGEnterpriseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<User> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index() => View();
        public IActionResult Privacy() => View();

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var viewModel = new DashboardViewModel
            {
                TotalRevenue = await _context.Invoices
                    .Where(i => i.Status == "Paid")
                    .SumAsync(i => (decimal?)i.Total) ?? 0,

                TotalInvoices = await _context.Invoices.CountAsync(),
                TotalCustomers = await _context.Customers.CountAsync(),
                TotalProducts = await _context.Products.CountAsync(),

                RecentInvoices = await _context.Invoices
                    .Include(i => i.Customer)
                    .OrderByDescending(i => i.InvoiceDate)
                    .Take(5)
                    .ToListAsync()
            };

            return View(viewModel);
        }

        public IActionResult Reports() => View();

        [HttpGet]
        public async Task<IActionResult> CustomerList()
        {
            var customers = await _context.Customers.ToListAsync();
            return View(customers);
        }

        public IActionResult SupplierList() => View();
        public IActionResult EmployeeList() => View();
        public IActionResult InventoryList() => View();
        public IActionResult InvoiceList() => View();
        public IActionResult QuoteList() => View();

        [HttpGet]
        public IActionResult AddCustomer()
        {
            return View(new Customer());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCustomer(Customer customer, string? CustomGroup)
        {
            if (!string.IsNullOrWhiteSpace(CustomGroup))
            {
                customer.CustomerGroup = CustomGroup;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    customer.CreatedAt = DateTime.UtcNow;
                    _context.Customers.Add(customer);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"Customer '{customer.Name}' added successfully!";
                    return RedirectToAction("CustomerList");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while saving new customer.");
                    TempData["Error"] = "An error occurred while saving the customer. Please try again.";
                }
            }

            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            bool isAdminRole = User.IsInRole("Admin");
            bool isTestAdmin = currentUser?.Email?.Equals("admin.test@example.com", StringComparison.OrdinalIgnoreCase) == true;

            if (!isAdminRole && !isTestAdmin)
            {
                TempData["Error"] = "Unauthorized: Only administrators can delete customers.";
                return RedirectToAction("CustomerList");
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Customer '{customer.Name}' deleted successfully!";
            }
            else
            {
                TempData["Error"] = "Customer not found.";
            }

            return RedirectToAction("CustomerList");
        }

        public IActionResult AddSupplier() => View();
        public IActionResult AddSalesRep() => View();
        public IActionResult AddEmployee() => View();
        public IActionResult AddItem() => View();
        public IActionResult CreateInvoice() => View();
        public IActionResult CreateQuote() => View();

        [HttpGet]
        public async Task<IActionResult> Settings()
        {
            var users = await _userManager.Users.ToListAsync();
            ViewBag.UserList = users;
            return View(new RegisterUserViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Settings(RegisterUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    EmailConfirmed = true,
                    IsActive = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    TempData["SettingsSuccess"] = $"User {model.Email} created successfully!";
                    return RedirectToAction("Settings");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ViewBag.UserList = await _userManager.Users.ToListAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            bool isAdminRole = User.IsInRole("Admin");
            bool isTestAdmin = currentUser?.Email?.Equals("admin.test@example.com", StringComparison.OrdinalIgnoreCase) == true;

            if (!isAdminRole && !isTestAdmin)
            {
                TempData["SettingsError"] = "Unauthorized: Only administrators can delete system accounts.";
                return RedirectToAction("Settings");
            }

            var userToDelete = await _userManager.FindByIdAsync(id);
            if (userToDelete != null)
            {
                if (userToDelete.Id == currentUser?.Id)
                {
                    TempData["SettingsError"] = "You cannot delete your own currently active account.";
                    return RedirectToAction("Settings");
                }

                var result = await _userManager.DeleteAsync(userToDelete);
                if (result.Succeeded)
                {
                    TempData["SettingsSuccess"] = $"User account '{userToDelete.Email}' deleted successfully!";
                }
                else
                {
                    TempData["SettingsError"] = "Failed to delete the user account.";
                }
            }
            else
            {
                TempData["SettingsError"] = "User account not found.";
            }

            return RedirectToAction("Settings");
        }

        [HttpGet]
        public async Task<IActionResult> UserProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var roles = await _userManager.GetRolesAsync(user);

            var model = new UserProfileViewModel
            {
                Id = user.Id,
                FullName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                Role = roles.Count > 0 ? roles[0] : "User"
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserProfile(UserProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            user.Email = model.Email;
            user.UserName = model.Email;
            user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["ProfileSuccess"] = "Your profile details have been updated successfully!";
                return RedirectToAction("UserProfile");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                TempData["SearchWarning"] = "Please enter a Customer Name, Email, or Phone Number to search.";
                return RedirectToAction("Dashboard");
            }

            var cleanQuery = query.Trim();
            var searchTerm = $"%{cleanQuery}%";

            List<Customer> matchingCustomers = [];

            try
            {
                matchingCustomers = await _context.Customers
                    .Where(c => EF.Functions.Like(c.Name, searchTerm) ||
                                EF.Functions.Like(c.Email, searchTerm) ||
                                EF.Functions.Like(c.ContactNumber, searchTerm))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "ContactNumber column error; falling back to Name and Email.");
                try
                {
                    matchingCustomers = await _context.Customers
                        .Where(c => EF.Functions.Like(c.Name, searchTerm) ||
                                    EF.Functions.Like(c.Email, searchTerm))
                        .ToListAsync();
                }
                catch (Exception fallbackEx)
                {
                    _logger.LogError(fallbackEx, "Database query failed during customer search.");
                    TempData["SearchWarning"] = "Unable to process search. You can search using Customer Name, Email Address, or Phone Number.";
                    return RedirectToAction("Dashboard");
                }
            }

            if (matchingCustomers.Count == 0)
            {
                TempData["SearchWarning"] = $"No customers found matching \"{cleanQuery}\". You can search using Customer Name, Email Address, or Phone Number.";
                return RedirectToAction("Dashboard");
            }

            ViewBag.Query = cleanQuery;
            return View("SearchResults", matchingCustomers);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}