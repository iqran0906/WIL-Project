using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using FMCGEnterpriseManagementSystem.Models;

namespace ExclusiveDistributors.Controllers
{
    public class EmployeesController : Controller
    {
        private static List<Employee> _employees = new List<Employee>();

        public IActionResult Index(string searchKeyword)
        {
            return View(_employees);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                _employees.Add(employee);
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }
    }
}