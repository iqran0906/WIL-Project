# FMCG Enterprise Management System

## Exclusive Distributors

The **FMCG Enterprise Management System** is an ASP.NET Core MVC web application developed for **Exclusive Distributors (Pty) Ltd**, a Fast-Moving Consumer Goods (FMCG) business.

The purpose of the system is to bring the company's day-to-day business operations together into one centralised platform. Instead of managing customers, products, inventory, quotations, invoices, payments and business reports separately, the system connects these processes so that information can move naturally from one part of the business to another.

For example, a sales representative can work with a customer, create a quotation, have the quotation approved, convert it into an invoice, record payments against that invoice and ultimately have the corresponding stock deducted from inventory.

The system also provides role-based access, reporting, forecasting, notifications and document exporting, allowing different members of the organisation to work with the information relevant to their responsibilities.

This project was developed as a **group Work Integrated Learning (WIL) project**, giving the development team practical experience in software development, database management, version control, system integration, security and collaborative development.

---

## Table of Contents

1. [Project Overview](#project-overview)
2. [Problem Statement](#problem-statement)
3. [Solution](#solution)
4. [Objectives](#objectives)
5. [Main Features](#main-features)
6. [Business Workflow](#business-workflow)
7. [Forecasting](#forecasting)
8. [Technology Stack](#technology-stack)
9. [System Architecture](#system-architecture)
10. [Design Patterns](#design-patterns)
11. [Project Structure](#project-structure)
12. [Database](#database)
13. [Security](#security)
14. [Notifications](#notifications)
15. [Email API](#email-api)
16. [PDF and Excel Exports](#pdf-and-excel-exports)
17. [Getting Started](#getting-started)
18. [Configuration](#configuration)
19. [Running the Application](#running-the-application)
20. [Git and Collaboration Workflow](#git-and-collaboration-workflow)
21. [Testing and Integration](#testing-and-integration)
22. [Team Responsibilities](#team-responsibilities)
23. [Future Improvements](#future-improvements)
24. [License](#license)

---

# Project Overview

Exclusive Distributors operates in the FMCG industry, where the business needs to manage a large amount of information relating to products, customers, suppliers, inventory and sales.

The **FMCG Enterprise Management System** was designed to provide a single system where these processes can be managed.

The application follows the journey of a sale from the initial customer interaction through to payment:

```text
Customer
   ↓
Quotation
   ↓
Quotation Approval
   ↓
Invoice
   ↓
Invoice Confirmation
   ↓
Inventory Updated
   ↓
Payment(s)
   ↓
Payment Status
```

The system also supports the operational activities surrounding this workflow, including:

* Customer management
* Supplier management
* Product management
* Inventory management
* Stock batch management
* Quotation management
* Invoice management
* Payment management
* Employee management
* Sales representative management
* Notifications
* Email communication
* PDF exports
* Excel exports
* Operational reports
* Stock forecasting
* Authentication and authorisation

The overall goal is to provide a system where information entered in one part of the application can be used by other parts of the business without unnecessary duplication.

---

# Problem Statement

FMCG businesses deal with a large amount of information every day.

Customers need to be managed, products need to be tracked, suppliers need to be recorded, stock levels need to be monitored and sales documents need to be created and followed through to payment.

When these processes are handled separately, several problems can occur.

For example:

* Customer information can become difficult to find.
* Stock information may not accurately reflect sales.
* Quotations and invoices can become disconnected.
* Payment information can be difficult to track.
* Employees may have access to information they do not require.
* Business reports may require information to be collected manually.
* Low stock may not be identified early enough.
* Important business events may not be communicated to the relevant users.

These problems can make it harder for a business to maintain accurate information and make informed operational decisions.

---

# Solution

The FMCG Enterprise Management System addresses these challenges by providing a **centralised business management platform**.

The system connects the main business processes together.

For example:

1. A customer is registered in the system.
2. A quotation can be created for that customer.
3. The quotation can go through an approval process.
4. Once approved, it can be converted into an invoice.
5. The invoice contains the relevant products and pricing information.
6. Confirming the invoice deducts the sold quantities from inventory.
7. One or more payments can then be recorded against the invoice.
8. The system determines whether the invoice is unpaid, partially paid or fully paid.
9. Relevant records can be exported or included in reports.
10. Notifications can be sent when important business events occur.

This creates a connected workflow rather than a collection of unrelated CRUD pages.

---

# Objectives

The main objectives of the system are to:

* Centralise FMCG business information.
* Improve the management of customers and suppliers.
* Maintain accurate product and inventory information.
* Track stock quantities and reorder levels.
* Manage quotations and their approval process.
* Create and manage invoices.
* Track multiple payments against invoices.
* Provide role-based access to system functionality.
* Notify users about important business events.
* Allow business documents to be exported.
* Provide operational reports.
* Provide basic stock forecasting.
* Apply appropriate software engineering principles.
* Demonstrate practical database and application development.
* Provide experience with collaborative Git-based development.

---

# Main Features

## 1. Authentication and Role Management

The system uses **ASP.NET Core Identity** to manage user authentication.

Users log into the system using their accounts, and access to functionality is controlled using roles.

The main roles are:

* **Administrator**
* **Employee**
* **Sales Representative**

Role-based authorisation prevents every user from automatically having access to every area of the application.

This allows the system to reflect the responsibilities of different employees within the organisation.

---

## 2. Customer Management

The customer module allows the business to maintain customer information in one place.

Users can manage customer records and associate customers with Sales Representatives.

This relationship helps connect customer information with the sales process.

Customer information can subsequently be used by other modules such as:

* Quotations
* Invoices
* Payments
* Reports
* Notifications

---

## 3. Supplier Management

Supplier information is maintained separately from customer information.

Suppliers can be associated with products, allowing the business to maintain information about where products originate from.

Supplier and product information can then be used as part of inventory and purchasing-related processes.

---

## 4. Product Management

The Product module stores the master information about products sold by Exclusive Distributors.

Product information includes information such as:

* Product code
* Product name
* Description
* Category
* Cost excluding VAT
* Cost including VAT
* Selling price
* Active/inactive status

Products are kept separate from inventory because a product and its stock state represent different concepts.

The **Product** record describes what an item is, while **Inventory** describes how much of that item is currently available.

---

## 5. Inventory Management

The Inventory module manages the stock currently held by the business.

Inventory includes information such as:

* Product
* Quantity on hand
* Reorder level
* Notes
* Stock batches
* Expiry dates

Stock can also be tracked using individual batches.

This is useful for FMCG products because different batches of the same product may have different quantities and expiry dates.

Inventory is also connected to the invoice workflow.

When an invoice is confirmed, the quantities sold can be deducted from inventory.

---

## 6. Stock Batch Management

Stock batches allow inventory to be tracked at a more detailed level.

A batch can contain:

* Batch number
* Quantity
* Received date
* Expiry date

This provides more control over stock than simply storing one total quantity for each product.

It also provides the foundation for monitoring products that have expiry dates.

---

## 7. Quotation Management

The quotation module allows users to create quotations for customers.

A quotation represents a proposed sale before the sale is finalised.

The quotation process includes an approval stage.

The general workflow is:

```text
Customer
   ↓
Quotation Created
   ↓
Quotation Reviewed
   ↓
Quotation Approved
   ↓
Invoice Created
```

Keeping quotations separate from invoices allows the business to distinguish between proposed sales and confirmed sales.

---

## 8. Invoice Management

Invoices are created as part of the sales workflow.

An invoice contains the products being sold, quantities, prices and applicable VAT.

The system also links an invoice to its originating quotation where applicable.

This makes the sales process traceable.

For example:

```text
Quotation ED00001
       ↓
Invoice ED00001
```

The system uses a document numbering convention consisting of:

```text
ED + five-digit number
```

Examples include:

```text
ED00001
ED00002
ED00003
```

When an invoice is confirmed, the corresponding inventory quantities are deducted.

---

## 9. Payment Management

The payment module manages payments made against invoices.

The system supports **multiple payments against one invoice**.

For example:

```text
Invoice Total:     R10,000

Payment 1:          R3,000
Payment 2:          R2,000
Payment 3:          R5,000

Total Paid:        R10,000
```

The payment status is determined from the relationship between the invoice total and the amount paid.

The main payment states are:

* **Unpaid**
* **Partially Paid**
* **Paid**

This means the payment status does not need to be manually guessed by the user. It can be calculated from the payment information stored in the system.

---

## 10. Employee Management

Employee information is integrated with the application's Identity and role system.

Employees can be associated with roles and responsibilities within the organisation.

This allows the system to distinguish between different types of users while still maintaining one central user-management structure.

---

## 11. Sales Representative Management

Sales Representatives are integrated into the customer and sales workflow.

Customers can be associated with Sales Representatives, allowing the business to identify which representative is responsible for a particular customer.

This information can also be used when working with sales-related reports and workflows.

---

# Business Workflow

The core sales workflow can be represented as follows:

```text
┌──────────┐
│ Customer │
└────┬─────┘
     │
     ▼
┌─────────────┐
│ Quotation   │
└────┬────────┘
     │
     ▼
┌─────────────┐
│ Approval    │
└────┬────────┘
     │
     ▼
┌─────────────┐
│ Invoice     │
└────┬────────┘
     │
     ├───────────────┐
     │               │
     ▼               ▼
┌─────────────┐   ┌─────────────┐
│ Inventory   │   │ Payments    │
│ Deduction   │   │ One or many │
└─────────────┘   └──────┬──────┘
                         │
                         ▼
                  ┌──────────────┐
                  │ Payment      │
                  │ Status       │
                  └──────────────┘
```

This workflow is one of the most important aspects of the application because it demonstrates how the different modules work together rather than functioning independently.

---

# VAT Rules

The application uses a business rule for calculating VAT.

A VAT category of:

```text
[NONE]
```

means that no VAT is charged.

For other applicable VAT categories, the system applies:

```text
15%
```

VAT calculations are handled within the application's business logic rather than being calculated manually by users.

Currency-related values use:

```text
decimal(18,2)
```

to provide predictable precision for financial values.

---

# Forecasting

The application includes a basic stock forecasting feature.

The forecasting functionality uses a:

**Moving Average Forecast Strategy**

The forecast considers product and inventory information such as:

* Current stock
* Reorder level
* Product information
* Available inventory data

The system produces a **30-day stock forecast**.

The forecasting implementation is intentionally a heuristic approach.

It is **not a machine-learning model**.

The Moving Average approach provides a relatively simple way of using existing business data to estimate future stock requirements.

This can help users identify products that may require attention before stock becomes critically low.

---

# Technology Stack

| Area                 | Technology                            |
| -------------------- | ------------------------------------- |
| Programming Language | C#                                    |
| Web Framework        | ASP.NET Core MVC                      |
| Runtime              | .NET 8                                |
| ORM / Data Access    | Entity Framework Core                 |
| Database             | Microsoft SQL Server                  |
| Development Database | SQL Server LocalDB                    |
| Hosted Database      | Azure SQL Database                    |
| Authentication       | ASP.NET Core Identity                 |
| Email                | Gmail SMTP                            |
| PDF Generation       | QuestPDF                              |
| Excel Generation     | ClosedXML                             |
| API Communication    | HTTP / REST-style API                 |
| Frontend             | Razor Views / HTML / CSS / JavaScript |
| Version Control      | Git                                   |
| Repository Hosting   | GitHub                                |
| IDE                  | Visual Studio 2022                    |

---

# System Architecture

The application uses a layered architecture.

The main flow is:

```text
View
  ↓
Controller
  ↓
Service
  ↓
Repository
  ↓
ApplicationDbContext
  ↓
SQL Server
```

Each layer has a specific responsibility.

This prevents controllers from becoming responsible for every part of the application's logic.

---

## Views

The Views are responsible for the user interface.

The application uses Razor Views to display information and provide forms through which users can interact with the system.

Examples include:

* Customer pages
* Product pages
* Inventory pages
* Quotation pages
* Invoice pages
* Payment pages
* Notification-related pages

Views should primarily focus on presentation rather than database operations or complex business logic.

---

## Controllers

Controllers receive requests from users and coordinate the response.

A controller typically:

1. Receives a request.
2. Validates the request.
3. Calls the appropriate service.
4. Receives the result.
5. Returns a View or redirects to another action.

Controllers do not directly handle all business rules.

---

## Services

Services contain business logic.

For example, services can handle:

* VAT calculations
* Invoice numbering
* Payment calculations
* Inventory updates
* Notification logic
* Export operations
* Forecasting logic

This keeps business rules separate from the user interface.

---

## Repositories

Repositories handle data-access operations.

They communicate with Entity Framework Core and the application's `ApplicationDbContext`.

Examples include repositories for:

* Customers
* Products
* Inventory
* Invoices
* Payments
* Notifications

Repositories help separate database operations from business logic.

---

## ApplicationDbContext

`ApplicationDbContext` is the central Entity Framework Core database context.

It represents the application's database structure and provides access to the relevant database entities.

It also manages relationships between entities and works with EF Core migrations to maintain database schema changes.

---

## ViewModels and DTOs

The application uses ViewModels and DTOs to prevent database models from being unnecessarily exposed between different layers.

### ViewModels

ViewModels are primarily used to prepare data specifically for Views.

For example, a form may require information from several database entities but only need selected fields for the user interface.

### DTOs

DTOs are used to transfer data between different parts of the application, particularly when communicating with the separate Email API.

This keeps the API contract clear and avoids sending unnecessary information.

---

# Dependency Injection

ASP.NET Core's dependency injection system is used throughout the application.

Controllers and services receive the interfaces they require instead of manually creating their dependencies.

For example:

```text
Controller
    ↓
INotificationService
    ↓
NotificationService
    ↓
INotificationRepository
```

This approach makes the application easier to maintain and test.

It also means that implementations can be changed without requiring large changes throughout the application.

---

# Design Patterns

The project uses several software design patterns.

## Repository Pattern

The Repository Pattern separates database operations from business logic.

For example:

```text
IProductRepository
        ↓
ProductRepository
        ↓
ApplicationDbContext
```

This keeps data-access responsibilities in the repository layer.

---

## Strategy Pattern

The Strategy Pattern is used where the application needs interchangeable approaches.

It is used for areas such as:

* Forecasting
* PDF exports
* Excel exports

For example:

```text
Export Strategy
      │
      ├── PDF Export
      │
      └── Excel Export
```

This allows additional export formats to be introduced without rewriting the entire export system.

---

## Factory Pattern

The `ExportFactory` selects the appropriate export strategy.

Conceptually:

```text
ExportFactory
      │
      ├── PDF → PDF Export Strategy
      │
      └── Excel → Excel Export Strategy
```

This keeps the selection logic in one location.

---

## Dependency Injection

Dependency Injection is used to connect controllers, services and repositories through interfaces.

This reduces tight coupling between components.

---

# Project Structure

The main project is structured as follows:

```text
FMCGEnterpriseManagementSystem/
│
├── Controllers/
│
├── Data/
│   └── ApplicationDbContext.cs
│
├── DTOs/
│
├── Enums/
│
├── Factories/
│
├── Helpers/
│
├── Models/
│
├── Repositories/
│   └── Interfaces/
│
├── Services/
│   └── Interfaces/
│
├── Strategies/
│   └── Interfaces/
│
├── ViewModels/
│
├── Views/
│
├── wwwroot/
│
├── Program.cs
│
└── appsettings.json
```

The project follows a consistent convention where interfaces are placed in an `Interfaces` folder within their relevant layer.

For example:

```text
Repositories/
├── Interfaces/
│   └── IProductRepository.cs
│
└── ProductRepository.cs
```

and:

```text
Services/
├── Interfaces/
│   └── IProductService.cs
│
└── ProductService.cs
```

This makes the project easier for team members to navigate.

---

# Database

The application uses a relational SQL Server database.

Entity Framework Core is used as the ORM between the C# application and SQL Server.

Database schema changes are managed through **EF Core migrations**.

---

## Key Database Relationships

| Relationship                    | Type                                               |
| ------------------------------- | -------------------------------------------------- |
| Supplier → Product              | One-to-many                                        |
| Product → Inventory             | Product/inventory relationship through `ProductId` |
| Inventory → StockBatch          | One-to-many                                        |
| Sales Representative → Customer | One-to-many                                        |
| Invoice → Payment               | One-to-many                                        |
| Invoice → Quotation             | Linked relationship                                |

---

## Product and Inventory Separation

Product and Inventory are intentionally separate.

### Product

The Product entity represents the product itself.

It contains information such as:

```text
ProductCode
ProductName
Description
Category
Cost
SellingPrice
IsActive
```

### Inventory

Inventory represents the current stock state.

It contains information such as:

```text
ProductId
QuantityOnHand
ReorderLevel
Notes
```

This separation prevents master product information from being mixed with stock information.

---

## Inventory and Stock Batches

Inventory can contain multiple stock batches.

For example:

```text
Product: Chocolate Bar

Batch 1
Quantity: 100
Expiry: 2027-01-01

Batch 2
Quantity: 150
Expiry: 2027-03-01
```

This makes the system more suitable for FMCG stock management where expiry dates can be important.

---

# Entity Framework Core

Entity Framework Core is responsible for mapping the application's C# entities to database tables.

The application uses migrations to manage database schema changes.

Typical migration workflow:

```bash
dotnet ef migrations add MigrationName
```

followed by:

```bash
dotnet ef database update
```

Because the application is developed by multiple team members and several features share the same database models, migrations must be coordinated carefully.

A migration should not be created independently without considering changes made by the other branches.

---

# Security

Security is an important part of the system because the application manages business and financial information.

## ASP.NET Core Identity

ASP.NET Core Identity is used for:

* User accounts
* Authentication
* Password hashing
* User management
* Role management

Passwords are not stored as plain text.

---

## Role-Based Authorisation

Access is controlled through roles.

The main roles are:

```text
Administrator
Employee
Sales Representative
```

Controllers and actions can restrict access to particular roles.

For example:

```csharp
[Authorize(Roles = "Administrator,Employee")]
```

This prevents users from accessing functionality outside their responsibilities.

---

## Server-Side Validation

Input is validated on the server using ASP.NET Core model validation and `ModelState`.

This is important because client-side validation alone cannot be trusted.

Invalid information should be rejected before it is written to the database.

---

## Entity Framework Parameterised Queries

Database operations are performed through Entity Framework Core.

This reduces the need for manually constructed SQL queries and helps protect against SQL injection.

---

## Anti-Forgery Protection

Forms use ASP.NET Core's anti-forgery protection where appropriate.

This helps protect state-changing requests from Cross-Site Request Forgery (CSRF).

---

## HTTPS

HTTPS is used where configured or deployed.

This protects communication between the browser and the application from being transmitted as unencrypted HTTP traffic.

---

# Notifications

The application includes a notification feature designed to keep users informed about important business events.

The planned notification events include:

* Low inventory
* Expired quotations for relevant customers
* New invoices
* New customers
* New products/items

The notification functionality is designed around the application's existing layered architecture rather than placing all notification logic inside controllers.

The notification area includes components such as:

```text
NotificationsController
INotificationService
NotificationService
INotificationRepository
NotificationRepository
NotificationFactory
NotificationType
NotificationDto
NotificationViewModel
```

The notification design also supports observer-style components for responding to events.

Examples include:

```text
INotificationSubject
INotificationObserver
InventoryNotificationSubject
PaymentNotificationSubject
EmailNotificationObserver
SystemAlertObserver
```

The overall intention is:

```text
Business Event
      ↓
Notification Logic
      ↓
Notification Created
      ↓
Email Notification
      ↓
Relevant User
```

For example, when inventory reaches or falls below its reorder level, the system can identify the event and trigger the relevant notification process.

The notification functionality is intended to reduce the need for users to manually check every module for important changes.

---

# Email API

The application includes a separate API component for email functionality.

This was included to satisfy the project's API requirement while also providing a practical business feature.

The MVC application communicates with the separate API over HTTP.

The general flow is:

```text
MVC Application
      ↓
HTTP Request
      ↓
Email API
      ↓
IEmailService
      ↓
Gmail SMTP
      ↓
Recipient
```

The application provides email functionality for records such as:

* Invoices
* Quotations
* Payments
* Notifications

The relevant pages contain an **Email** action that allows a record to be sent through the email functionality.

---

## DTOs

DTOs are used for communication between the MVC application and the Email API.

This provides a clear contract between the two applications.

Rather than sending an entire database entity, only the required information is transferred.

---

## Error Handling

The API includes centralised error handling so that common problems can return clear and consistent responses.

Examples include:

* Invalid requests
* Missing records
* Email/SMTP failures
* Unexpected application errors

The Email API is a separate component from the MVC application and can therefore be developed and maintained independently.

---

# PDF and Excel Exports

The system supports exporting business information into common document formats.

## PDF

PDF generation is implemented using **QuestPDF**.

PDF exports can be used for business documents such as invoices and other applicable records.

The PDF generation logic is separated from the controller through the application's export strategy architecture.

---

## Excel

Excel exports are implemented using **ClosedXML**.

Excel is useful for business records that need to be reviewed, filtered or further analysed.

The export architecture allows PDF and Excel functionality to use separate strategies while sharing a common export structure.

---

# Getting Started

## Prerequisites

Before running the project, ensure the following are installed:

* **.NET 8 SDK**
* **Visual Studio 2022 or later**
* **SQL Server LocalDB** or another SQL Server instance
* **Git**
* A Gmail account with an App Password if email functionality is required

Visual Studio's SQL Server LocalDB installation can be used for development.

---

# Clone the Repository

Clone the project using Git:

```bash
git clone [REPOSITORY-URL]
```

Then move into the project directory:

```bash
cd FMCGEnterpriseManagementSystem
```

The solution can then be opened in Visual Studio.

---

# Database Configuration

The application's database connection is configured through `appsettings.json` or an appropriate environment-specific configuration file.

A LocalDB development connection can follow this general structure:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=FMCGEnterpriseManagementSystem;Trusted_Connection=True;"
}
```

The exact connection string should match the developer's local SQL Server configuration.

Sensitive production connection information should not be committed to source control.

---

# Applying Database Migrations

After configuring the database, apply the existing Entity Framework Core migrations:

```bash
dotnet ef database update
```

If the `dotnet ef` command is not installed globally, it can be installed with:

```bash
dotnet tool install --global dotnet-ef
```

The database should be updated before attempting to use features that depend on newly introduced tables.

---

# Email Configuration

Email functionality requires SMTP credentials.

Sensitive credentials should **not** be placed directly into committed configuration files.

For local development, .NET User Secrets can be used.

For example:

```bash
dotnet user-secrets set "Email:Username" "your-address@gmail.com"
```

and:

```bash
dotnet user-secrets set "Email:Password" "your-app-password"
```

The exact configuration keys should match the application's actual email settings.

For Gmail, an **App Password** should be used rather than storing a normal Gmail account password in the application.

---

# Running the Application

The application can be started from Visual Studio by pressing:

```text
F5
```

or:

```text
Ctrl + F5
```

It can also be started through the command line:

```bash
dotnet run
```

If the Email API is required, both the MVC application and API project should be running.

In Visual Studio, multiple startup projects can be configured so that both projects start together.

---

# Default Roles

The system uses the following main roles:

```text
Administrator
Employee
Sales Representative
```

The exact method used to create or seed the first administrator account depends on the current project configuration.

When setting up a new development environment, developers should ensure that an administrator account exists before testing role-restricted functionality.

---

# Git and Collaboration Workflow

Because this was developed as a group WIL project, Git and GitHub were used to allow multiple developers to work on different features.

Each major feature was developed on its own branch.

Examples include:

```text
feature/invoices-management
feature/quotes
feature/payments
feature/notifications
feature/exports
```

The general workflow was:

```text
Development
     ↑
     │
Feature Branch
     │
     ↓
Development Work
     │
     ↓
Commit
     │
     ↓
Push
     │
     ↓
Pull Request / Merge
     │
     ↓
Development Branch
```

---

## Recommended Git Workflow

### 1. Update the development branch

Before beginning new work, get the latest development changes.

```bash
git checkout development
git pull
```

### 2. Create or switch to the feature branch

```bash
git checkout feature/your-feature
```

### 3. Develop and test

Make the required changes and test them locally.

### 4. Commit changes

Commits should describe what was actually changed.

For example:

```text
Add inventory low-stock notification
```

rather than:

```text
changes
```

### 5. Push the branch

```bash
git push
```

### 6. Merge completed work

Once the feature has been tested, it can be merged into the development branch according to the team's agreed process.

---

# Shared Model and Migration Considerations

Because the system contains many connected modules, developers cannot always treat their feature as completely isolated.

For example, an Invoice feature may depend on:

* Customer
* Product
* Inventory
* Quotation
* Payment

Similarly, Notifications may depend on events generated by:

* Inventory
* Quotes
* Invoices
* Customers
* Products

This means that merging code is only one part of integration.

After merging, the team should also check:

* Shared models
* Entity relationships
* ViewModels
* Services
* Interfaces
* Database migrations
* Dependency injection
* Controller routes
* Cross-feature workflows

---

# Testing and Integration

Testing is performed throughout development rather than only at the end.

Each feature should be tested individually before being integrated into the development branch.

Examples include:

### Customer Testing

* Add customer
* Edit customer
* View customer
* Delete customer
* Assign Sales Representative

### Product Testing

* Add product
* Edit product
* View product
* Manage product information

### Inventory Testing

* Add stock
* Update quantity
* Set reorder level
* Add stock batch
* Track expiry date
* Verify stock deduction after invoice confirmation

### Quotation Testing

* Create quotation
* Edit quotation
* Approve quotation
* Verify quotation-to-invoice workflow

### Invoice Testing

* Create invoice
* Verify product information
* Calculate totals
* Calculate VAT
* Confirm invoice
* Verify inventory deduction
* Export invoice

### Payment Testing

* Record payment
* Record multiple payments
* Verify unpaid status
* Verify partially paid status
* Verify paid status

### Notification Testing

* Trigger low-stock event
* Trigger quotation-related notification
* Trigger invoice-related notification
* Trigger customer notification
* Trigger product notification
* Verify email delivery

### Security Testing

* Test login
* Test invalid credentials
* Test role restrictions
* Test unauthorised access
* Test server-side validation

---

# Team Responsibilities

The project was developed collaboratively, with each team member taking responsibility for specific areas.

| Team Member | Main Responsibilities                                                                                                                           |
| ----------- | ----------------------------------------------------------------------------------------------------------------------------------------------- |
| **Iqra**    | Authentication and Identity, roles and authorisation, employee and Sales Representative administration, reports, security and final integration |
| **Sayali**  | Backend/API work, application-to-API communication, notification/email integration and supporting backend services                              |
| **Naseeha** | ApplicationDbContext, EF Core migrations, database integration, Product, Inventory, Supplier and stock/batch data                               |
| **Maseeha** | Customer-facing CRUD/UI, employee and Sales Representative UI and model work, notifications and user-facing interactions                        |
| **Imran**   | Quotation, Invoice and Payment workflows, related ViewModels/UI and financial/report presentation components                                    |

Although each member had primary ownership of particular modules, the final application depends on these modules working together.

---

# Integration Between Modules

One of the key aspects of the project is that the modules are not completely independent.

For example:

```text
Product
   ↓
Inventory
   ↓
Quotation
   ↓
Invoice
   ↓
Payment
   ↓
Reports / Notifications
```

Customer information also flows through the sales process:

```text
Customer
   ↓
Quotation
   ↓
Invoice
   ↓
Payment
```

This interconnected structure means that changes in one module can affect another module.

The team therefore needed to consider integration whenever shared models, database relationships or business rules were changed.

---

# Example End-to-End Scenario

A typical business transaction can look like this:

### Step 1 — Customer

A customer is registered in the system.

```text
Customer:
ABC Retail Store
```

The customer can be associated with a Sales Representative.

---

### Step 2 — Quotation

A Sales Representative creates a quotation for the customer.

The quotation contains the required products and quantities.

---

### Step 3 — Approval

The quotation is reviewed and approved.

Only after approval can the sales process continue to the invoice stage.

---

### Step 4 — Invoice

The approved quotation is converted into an invoice.

The invoice contains:

* Product code
* Product description
* Quantity
* Unit price
* Discount
* VAT
* Total

The invoice is linked to the original quotation.

---

### Step 5 — Inventory

Once the invoice is confirmed, the sold quantities are deducted from inventory.

For example:

```text
Before:
Quantity on hand = 100

Sold:
Quantity = 10

After:
Quantity on hand = 90
```

If the remaining stock reaches the reorder level, the notification functionality can identify the low-stock event.

---

### Step 6 — Payment

The customer makes a payment.

The payment is recorded against the invoice.

If the customer pays in multiple instalments, each payment can be recorded separately.

---

### Step 7 — Payment Status

The system compares the total payments with the invoice total.

For example:

```text
Invoice total:  R10,000
Paid:           R4,000

Status:         Partially Paid
```

Once the full amount has been received:

```text
Invoice total:  R10,000
Paid:          R10,000

Status:         Paid
```

---

### Step 8 — Documents and Reporting

The relevant invoice, quotation or payment can be exported where required.

The information generated by these transactions can also contribute to operational reporting.

---

# Future Improvements

The system provides the foundation for further development.

Potential future improvements include:

* More advanced forecasting algorithms
* Improved stock demand prediction
* More detailed dashboards
* Additional notification channels
* SMS notifications
* More detailed audit logging
* Advanced reporting filters
* Customer payment history dashboards
* Supplier performance reporting
* Automated scheduled reports
* More advanced batch and expiry management
* Expanded API functionality
* Automated testing
* Improved deployment automation
* Cloud-based monitoring and logging

These improvements could be introduced without completely redesigning the application because the current layered architecture separates the main areas of responsibility.

---

# Academic and WIL Context

This application was developed as part of a **Work Integrated Learning (WIL)** project.

The project provided practical experience in applying software development concepts to a realistic business scenario.

The development process involved more than simply creating individual pages.

The team had to consider:

* Requirements analysis
* Database design
* Entity relationships
* MVC architecture
* Business logic
* Authentication
* Authorisation
* API integration
* Email communication
* Document generation
* Forecasting
* Validation
* Error handling
* Git branching
* Merge conflicts
* Feature integration
* Testing
* Team collaboration

The project therefore demonstrates how multiple software engineering concepts can be combined into one functioning business application.

---

# Conclusion

The **FMCG Enterprise Management System** provides Exclusive Distributors with a centralised platform for managing important FMCG business operations.

The application connects customers, products, inventory, quotations, invoices and payments into a single workflow while also providing supporting functionality such as notifications, email communication, exports, reporting and forecasting.

The system was designed using a layered architecture to keep presentation, business logic and data access separate.

The use of repositories, services, strategies, factories, dependency injection, Entity Framework Core and ASP.NET Core Identity provides a structured foundation for maintaining and extending the application.

Most importantly, the project demonstrates the practical application of software engineering principles within a real-world business scenario and reflects the collaborative development experience gained through the WIL project.

---

# License

This project was developed as an **academic Work Integrated Learning project**.

It is not intended for commercial redistribution or production use without further development, testing, security review and deployment configuration.

Copyright © 2026 Exclusive Distributors WIL Development Team.
