using System.Diagnostics;

using FMCGEnterpriseManagementSystem.Models;

using Microsoft.AspNetCore.Mvc;



namespace FMCGEnterpriseManagementSystem.Controllers

{

    public class HomeController : Controller

    {

        private readonly ILogger<HomeController> _logger;



        public HomeController(ILogger<HomeController> logger)

        {

            _logger = logger;

        }


        public IActionResult Index()

        {

            return View();

        }

        public IActionResult Login()
        {
            return View();
        }


        public IActionResult UserProfile()
        {
            return View();
        }


        public IActionResult Privacy()

        {

            return View();

        }


        public IActionResult Dashboard()

        {

            return View();

        }



        public IActionResult CustomerList()

        {

            return View();

        }

        public IActionResult Notifications()
        {
            return View();
        }



        public IActionResult AddCustomer()

        {

            return View();

        }



        public IActionResult AddSupplier()

        {

            return View();

        }



        public IActionResult AddSalesRep()

        {

            return View();

        }



        public IActionResult AddItem()

        {

            return View();

        }


        public IActionResult CreateInvoice()

        {

            return View();

        }


        public IActionResult CreateQuote()

        {

            return View();

        }


        public IActionResult AddEmployee()

        {

            return View();

        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

        public IActionResult Error()

        {

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

        }

    }

}