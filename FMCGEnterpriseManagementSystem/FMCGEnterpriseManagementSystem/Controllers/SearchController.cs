
// Title: Controller action return types in ASP.NET Core MVC
// Authors: Microsoft
// Date: 27-04-2026
// Code version: ASP.NET Core 10.0
// Availability: https://learn.microsoft.com/aspnet/core/mvc/controllers/actions

using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    // Restricts the global search feature to authenticated users.
    [Authorize]
    public class SearchController : Controller
    {
        private readonly ISearchService _searchService;

        // Injects the search service used to perform the global search.
        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        // GET: /Search?q=ED00012
        [HttpGet]
        public async Task<IActionResult> Index(string? q)
        {
            // If no search term was entered, return the user to the dashboard.
            if (string.IsNullOrWhiteSpace(q))
            {
                return RedirectToAction("Dashboard", "Home");
            }

            // Performs the search using the current user's permissions/context.
            var model = await _searchService.SearchAsync(q, User);

            // Go straight to the record when the entry clearly identifies one.
            var target = GetDirectMatch(model);

            if (target != null)
            {
                // Redirects directly to the matching record when possible.
                return RedirectToAction(
                    target.Action,
                    target.Controller,
                    target.RouteValues);
            }

            // Displays grouped search results when there is no single direct match.
            return View(model);
        }

        private static SearchResultItem? GetDirectMatch(GlobalSearchViewModel model)
        {
            // Find results that were identified as exact number/code matches.
            var exactMatches = model.Results
                .Where(r => r.IsExactMatch)
                .ToList();

            // Redirect only when exactly one exact match can be opened.
            if (exactMatches.Count == 1)
            {
                return exactMatches[0].CanRedirect
                    ? exactMatches[0]
                    : null;
            }

            // If there is only one general result, redirect to it when possible.
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