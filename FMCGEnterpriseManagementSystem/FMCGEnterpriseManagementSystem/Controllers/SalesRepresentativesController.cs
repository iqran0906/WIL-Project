using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using FMCGEnterpriseManagementSystem.Models;

namespace ExclusiveDistributors.Controllers
{
    public class SalesRepresentativesController : Controller
    {
        private static List<SalesRepresentative> _salesReps = new List<SalesRepresentative>();

        public IActionResult Index(string searchKeyword)
        {
            return View(_salesReps);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SalesRepresentative salesRep)
        {
            if (ModelState.IsValid)
            {
                _salesReps.Add(salesRep);
                return RedirectToAction(nameof(Index));
            }
            return View(salesRep);
        }
    }
}