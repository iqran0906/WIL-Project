/*
 * Purpose: Site-wide script: checks that filter end dates are not before start dates.
 * Authors: Naseeha27 (from git history)
 */
// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Client-side check for any filter form with Start/End date fields
// (reports, invoices, quotes). The server checks the range again.
document.addEventListener("DOMContentLoaded", function () {
    const message = "The end date cannot be before the start date.";

    document.querySelectorAll("form").forEach(function (form) {
        const start = form.querySelector('input[name="startDate"]');
        const end = form.querySelector('input[name="endDate"]');
        if (!start || !end) return;

        function check() {
            // Stop the date pickers offering an impossible range
            end.min = start.value || "";
            start.max = end.value || "";

            const invalid = start.value && end.value && end.value < start.value;
            end.setCustomValidity(invalid ? message : "");
            return !invalid;
        }

        start.addEventListener("change", check);
        end.addEventListener("change", check);

        form.addEventListener("submit", function (e) {
            if (!check()) {
                e.preventDefault();
                end.reportValidity();
            }
        });

        check();
    });
});
