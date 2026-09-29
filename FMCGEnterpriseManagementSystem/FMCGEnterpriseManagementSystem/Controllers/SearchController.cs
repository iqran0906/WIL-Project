// Purpose: Global search bar: jumps to a record or shows grouped search results.
// Authors: ST10068525 (new file, not yet committed)

using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    [Authorize]
    public class SearchController : Controller
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        // GET: /Search?q=ED00012
        [HttpGet]
        public async Task<IActionResult> Index(string? q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return RedirectToAction("Dashboard", "Home");
            }

            var model = await _searchService.SearchAsync(q, User);

            // Go straight to the record when the entry clearly identifies one
            var target = GetDirectMatch(model);

            if (target != null)
            {
                return RedirectToAction(
                    target.Action,
                    target.Controller,
                    target.RouteValues);
            }

            return View(model);
        }

        private static SearchResultItem? GetDirectMatch(GlobalSearchViewModel model)
        {
            // One exact number/code match, e.g. an invoice number or item code
            var exactMatches = model.Results
                .Where(r => r.IsExactMatch)
                .ToList();

            if (exactMatches.Count == 1)
            {
                return exactMatches[0].CanRedirect
                    ? exactMatches[0]
                    : null;
            }

            // Otherwise, only one result of any kind
            if (exactMatches.Count == 0
                && model.Results.Count == 1
                && model.Results[0].CanRedirect)
            {
                return model.Results[0];
            }

            return null;
        }
    }
}
