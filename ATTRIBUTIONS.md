# Attributions

This file credits the people, libraries and tools behind the **FMCG Enterprise Management System** (Exclusive Distributors), a Work Integrated Learning (WIL) project.

Every source file (`.cs`, `.cshtml`, `.js`) also starts with a short header comment giving:

- **Purpose:** what the file is for.
- **Authors:** the contributors taken from the git history of that file ("new file, not yet committed" means it has not been committed yet).
- **Uses:** included only where the file uses a third-party library directly.

Generated EF Core migrations (`Migrations/`) and the bundled client libraries in `wwwroot/lib` keep their original headers and licences.

## Project team (from git history)

| Git account | Commits |
|---|---|
| Maseeha17 | 68 |
| iqran0906 | 65 |
| Sayali-St10458649 | 31 |
| Naseeha27 | 13 |
| ST10068525 | 2 |

## Frameworks and server-side libraries

| Library | Version | Licence | Used for | Link |
|---|---|---|---|---|
| ASP.NET Core MVC (.NET 8) | 8.0 | MIT | Web framework, controllers, Razor views, validation | https://learn.microsoft.com/aspnet/core |
| ASP.NET Core Identity | 8.0.28 | MIT | Login accounts, roles, passwords | https://learn.microsoft.com/aspnet/core/security/authentication/identity |
| Entity Framework Core (SQL Server) | 8.0.30 | MIT | Database access and migrations | https://learn.microsoft.com/ef/core |
| QuestPDF | 2026.9.0 | QuestPDF Community License | PDF exports (reports, payments) | https://www.questpdf.com |
| ClosedXML | 0.105.1 | MIT | Excel exports | https://github.com/ClosedXML/ClosedXML |

## Front-end libraries

| Library | Version | Licence | Used for | Link |
|---|---|---|---|---|
| Bootstrap | 5.1.0 | MIT | Layout, forms, tables, cards, pop-ups | https://getbootstrap.com |
| Bootstrap Icons | 1.11.3 (CDN) | MIT | Icons throughout the interface | https://icons.getbootstrap.com |
| jQuery | 3.6.0 | MIT | Required by jQuery Validation | https://jquery.com |
| jQuery Validation | 1.19.5 | MIT | Client-side form validation | https://jqueryvalidation.org |
| jQuery Validation Unobtrusive | 4.0.0 | MIT | Connects ASP.NET validation rules to jQuery Validation | https://github.com/aspnet/jquery-validation-unobtrusive |
| Chart.js | 4.4.1 (CDN) | MIT | Dashboard charts (income line chart, category doughnut) | https://www.chartjs.org |

## Design patterns and references

- **Repository, Strategy, Factory and Observer patterns**: `Repositories/`, `Strategies/` + `Factories/ExportFactory.cs`, and `Observers/` + `Factories/NotificationFactory.cs`.
- **Microsoft documentation**, used for MVC filters, model validation, error handling (`UseExceptionHandler`, `UseStatusCodePagesWithReExecute`) and Identity: https://learn.microsoft.com/aspnet/core
- **Dashboard chart colours**: checked for colour-blind safety and contrast against the dashboard background with a palette validation script.
