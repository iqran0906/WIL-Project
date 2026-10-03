/***************************************************************************************
*    Title: Global Search View Model
*    Author: ST10068525
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/ViewModels/GlobalSearchViewModel.cs
***************************************************************************************/

namespace FMCGEnterpriseManagementSystem.ViewModels
{
    public class GlobalSearchViewModel
    {
        public string Query { get; set; } = string.Empty;

        public List<SearchResultItem> Results { get; set; } = new();

        public int TotalResults => Results.Count;
    }

    public class SearchResultItem
    {
        // e.g. "Customer", "Invoice", "Product"
        public string Category { get; set; } = string.Empty;

        public string Icon { get; set; } = "bi-search";

        public string Title { get; set; } = string.Empty;

        public string? Subtitle { get; set; }

        // Extra label/value pairs shown under the result (used for customers)
        public Dictionary<string, string> Details { get; set; } = new();

        // Where the result links to
        public string Controller { get; set; } = string.Empty;

        public string Action { get; set; } = "Index";

        public Dictionary<string, string> RouteValues { get; set; } = new();

        public string LinkText { get; set; } = "Open";

        // True when the search text matched this record's number/code exactly
        public bool IsExactMatch { get; set; }

        // False when there is no page for the record itself (e.g. customers),
        // so the user should see the details on the results page instead
        public bool CanRedirect { get; set; } = true;
    }
}
