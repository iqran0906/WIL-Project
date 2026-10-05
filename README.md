# FMCG Enterprise Management System

## Exclusive Distributors

The **FMCG Enterprise Management System** is an ASP.NET Core MVC web application developed for **Exclusive Distributors (Pty) Ltd**, a Fast-Moving Consumer Goods (FMCG) business.

The purpose of the system is to bring the company's day-to-day business operations together into one centralised platform. Instead of managing customers, suppliers, products, inventory, quotations, invoices, payments and business reports separately, the system connects these processes so that information can move naturally between different areas of the business.

For example, a Sales Representative can work with a customer and create a quotation. The quotation can then be converted into an invoice, which initially enters a pending state. An Administrator can review and approve the invoice, at which point the relevant stock quantities are deducted from inventory. Payments can then be recorded against the approved invoice until the outstanding balance reaches zero and the completed invoice moves into invoice history.

The system also provides role-based access control, employee and Sales Representative administration, stock forecasting, notifications, reporting, document exports, email functionality and user account management. These features allow different members of the organisation to access and manage the information relevant to their responsibilities.

The project also includes a separate API component that supports email communication between the system and its users, as well as automated unit testing for key business processes.

This project was developed as a **group Work Integrated Learning (WIL) project**, providing the development team with practical experience in full-stack software development, database management, API integration, version control, automated testing, continuous integration, system integration, security and collaborative software development.

---

## Table of Contents

1. [Project Overview](#project-overview)
2. [Problem Statement](#problem-statement)
3. [Solution](#solution)
4. [Objectives](#objectives)
5. [Main Features](#main-features)
   - [1. Authentication, User Accounts and Role Management](#1-authentication-user-accounts-and-role-management)
   - [2. Customer Management](#2-customer-management)
   - [3. Supplier Management](#3-supplier-management)
   - [4. Product Management](#4-product-management)
   - [5. Inventory Management](#5-inventory-management)
   - [6. Stock Batch Management](#6-stock-batch-management)
   - [7. Quotation Management](#7-quotation-management)
   - [8. Invoice Management](#8-invoice-management)
   - [9. Payment Management](#9-payment-management)
   - [10. Employee Management](#10-employee-management)
   - [11. Sales Representative Management](#11-sales-representative-management)
6. [Business Workflow](#business-workflow)
7. [VAT Rules](#vat-rules)
8. [Forecasting](#forecasting)
9. [Dashboard and Reporting](#dashboard-and-reporting)
10. [Technology Stack](#technology-stack)
11. [System Architecture](#system-architecture)
12. [Dependency Injection](#dependency-injection)
13. [Design Patterns](#design-patterns)
14. [Project Structure](#project-structure)
15. [Database](#database)
16. [Entity Framework Core](#entity-framework-core)
17. [Security](#security)
18. [Notifications](#notifications)
19. [Email API](#email-api)
20. [PDF and Excel Exports](#pdf-and-excel-exports)
21. [Getting Started](#getting-started)
22. [Clone the Repository](#clone-the-repository)
23. [Database Configuration](#database-configuration)
24. [Applying Database Migrations](#applying-database-migrations)
25. [Email Configuration](#email-configuration)
26. [Running the Application](#running-the-application)
27. [Default Roles](#default-roles)
28. [Git and Collaboration Workflow](#git-and-collaboration-workflow)
29. [Shared Model and Migration Considerations](#shared-model-and-migration-considerations)
30. [Testing and Integration](#testing-and-integration)
31. [Team Responsibilities](#team-responsibilities)
32. [Integration Between Modules](#integration-between-modules)
33. [Example End-to-End Scenario](#example-end-to-end-scenario)
34. [Future Improvements](#future-improvements)
35. [Academic and WIL Context](#academic-and-wil-context)
36. [Conclusion](#conclusion)
37. [License](#license)

---

# Project Overview

Exclusive Distributors operates within the Fast-Moving Consumer Goods (FMCG) industry, where the business needs to manage information relating to customers, suppliers, products, inventory, sales, payments and employees efficiently.

The **FMCG Enterprise Management System** was developed to provide a single centralised system through which these business processes can be managed and connected.

Rather than functioning as a collection of independent CRUD modules, the application integrates the different areas of the business. Information created in one module can be used throughout later stages of the business workflow.

The core sales process follows the journey from an initial customer quotation through to a completed and paid invoice:

```text
Customer
   ↓
Quotation
   ↓
Convert to Invoice
   ↓
Pending Invoice
   ↓
Administrator Approval
   ↓
Inventory Deduction
   ↓
Payment(s)
   ↓
Partially Paid / Paid
   ↓
Invoice History
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

Exclusive Distributors operates within the Fast-Moving Consumer Goods (FMCG) industry, where multiple business activities must be managed simultaneously. These activities include maintaining customer and supplier information, managing products and inventory, preparing quotations and invoices, recording payments, monitoring stock levels and providing management with useful operational information.

When these processes are handled separately or rely heavily on manual administration, business information can become fragmented and difficult to manage.

This can create several operational challenges, including:

* Customer and supplier information being stored or managed separately from related transactions.
* Product information becoming disconnected from current inventory levels.
* Difficulty monitoring available stock, reorder levels and stock batches.
* Quotations not being clearly connected to the invoices generated from them.
* Stock being updated inconsistently when sales take place.
* Difficulty tracking outstanding, partially paid and fully paid invoices.
* Limited visibility of historical or inactive business records.
* Employees having access to functionality that is not relevant to their responsibilities.
* Important business events, such as low stock or new transactions, being overlooked.
* Management information requiring data to be gathered manually from different areas.
* Difficulty maintaining a clear record of the progression of a transaction from quotation through to payment.

These challenges are particularly important in an FMCG environment because products and stock quantities can change frequently and the business needs reliable information to support day-to-day operations.

Exclusive Distributors therefore required a centralised system capable of connecting these business processes while maintaining appropriate access control, data consistency and historical information.

The system needed to support the complete flow of information between customers, quotations, invoices, inventory and payments while also providing supporting functionality such as reporting, forecasting, notifications, document exports and user administration.
---

# Solution

The **FMCG Enterprise Management System** addresses these challenges by providing Exclusive Distributors with a centralised platform that connects the organisation's main operational, sales and administrative processes.

Rather than treating customers, products, inventory, quotations, invoices and payments as unrelated records, the system allows information to flow between these modules as part of a connected business process.

A typical sales transaction follows this process:

1. A customer is registered and can be assigned to a Sales Representative.
2. A quotation is created for the customer using products available within the system.
3. The quotation can be reviewed and edited while it remains a current quotation.
4. When the customer proceeds with the sale, the quotation can be converted into an invoice.
5. The converted quotation is retained in quotation history, preserving the relationship between the original quotation and the resulting invoice.
6. The newly created invoice enters a **Pending** state.
7. An Administrator reviews the invoice and approves it.
8. When the invoice changes from **Pending** to **Approved**, the corresponding product quantities are deducted from inventory.
9. Payments can then be recorded against the approved invoice.
10. The system calculates the amount already paid and the remaining outstanding balance.
11. Multiple payments can be recorded until the invoice is fully paid.
12. Once the outstanding balance reaches zero, the completed invoice is displayed in **Invoice History**.
13. Transaction information can contribute to reports, dashboard information, notifications and other operational functionality.

The system also provides supporting administrative functionality for managing:

* Employees
* Sales Representatives
* User accounts
* Roles and permissions
* Customers
* Suppliers
* Products
* Inventory
* Stock information

Instead of permanently removing important business information in every situation, the application preserves records where appropriate through active, inactive, discontinued, deactivated and historical states. This allows previous business information to remain available without cluttering the current operational views.

Role-based authorisation ensures that users only have access to the functionality appropriate to their responsibilities. Administrators have access to sensitive administrative functionality, while Employees and Sales Representatives are provided with access according to the requirements of their roles.

The application further supports business operations through:

* Dashboard analytics
* Operational reports
* Stock forecasting
* Notifications
* Email communication through a separate API
* PDF and Excel exports
* User profile and account functionality
* Password recovery through email
* Automated unit testing
* Git-based collaborative development and integration

Together, these features create an integrated enterprise management system rather than a collection of independent data-entry pages.
---

# Objectives

The primary objective of the **FMCG Enterprise Management System** is to provide Exclusive Distributors with a centralised platform for managing and connecting its main business operations.

The objectives of the system are to:

* Centralise customer, supplier, product, inventory, employee and sales information.
* Provide secure user authentication and account management.
* Implement role-based authorisation for Administrators, Employees and Sales Representatives.
* Manage customer information and associate customers with Sales Representatives.
* Maintain supplier and product information.
* Monitor inventory quantities, reorder levels and stock information.
* Support stock batch and expiry-related information.
* Create and manage customer quotations.
* Convert quotations into invoices while retaining the original quotation in quotation history.
* Provide an invoice approval process before stock and payment processing takes place.
* Automatically deduct the relevant stock quantities when an invoice is approved.
* Record multiple payments against approved invoices.
* Calculate outstanding invoice balances and distinguish between approved, partially paid and fully paid invoices.
* Maintain invoice history for completed and fully paid transactions.
* Preserve inactive, deactivated, discontinued and historical records where appropriate.
* Provide operational reports and dashboard information to support business decision-making.
* Provide stock forecasting to assist with inventory planning.
* Generate notifications for relevant business events.
* Support email communication through a separate API component.
* Provide password recovery through email.
* Allow applicable business information and documents to be exported to PDF and Excel formats.
* Apply server-side validation and appropriate security controls.
* Use a layered architecture to separate presentation, business logic and data access responsibilities.
* Apply software design patterns where appropriate to improve maintainability and separation of concerns.
* Use Entity Framework Core and SQL Server for relational data management.
* Use automated unit testing to verify important business rules and workflows.
* Use Git and GitHub to support collaborative development, feature branching and system integration.
* Support continuous integration through GitHub Actions.
* Provide practical experience with the development, integration, testing and deployment of a multi-module enterprise application.

---

# Main Features

## 1. Authentication, User Accounts and Role Management

The system uses **ASP.NET Core Identity** for authentication, user account management and role-based authorisation.

Users are required to authenticate before accessing protected areas of the application. ASP.NET Core Identity manages the underlying user accounts and provides secure password hashing rather than storing passwords as plain text.

The system uses three primary roles:

* **Administrator**
* **Employee**
* **Sales Representative**

Access to system functionality is controlled according to these roles.

### Administrator

Administrators have the highest level of access and are responsible for administrative and management functionality. This includes areas such as:

* Employee management
* Sales Representative management
* User account management
* Reports
* System-wide administrative functionality

### Employee

Employees can access operational functionality required for day-to-day business activities, including applicable areas such as:

* Customers
* Suppliers
* Products
* Inventory
* Forecasting
* Quotations
* Invoices
* Payments
* Notifications

Employees do not automatically receive access to administrator-only user and employee administration functionality.

### Sales Representative

Sales Representatives are primarily provided with access to sales-related functionality, including applicable areas such as:

* Customers
* Quotations
* Invoices
* Payments
* Notifications

This allows Sales Representatives to work with customer and sales information without being given unnecessary access to administrative functionality.

### User Account Management

Administrator functionality is provided for managing system user accounts and their associated access.

Employee and Sales Representative information can therefore operate alongside the application's Identity-based authentication and role system.

### Login and Access Control

The authentication workflow includes:

* Secure login
* Logout
* Role-based access control
* Access-denied handling
* Authentication cookies
* Server-side user validation
* User profile functionality

Controller actions are protected using ASP.NET Core authorisation mechanisms where appropriate.

For example:

```csharp
[Authorize(Roles = "Administrator")]
```

---

## 2. Customer Management

The Customer Management module provides a central location for maintaining customer information used throughout the sales process.

Authorised users can:

* Add new customers
* View customer information
* Edit existing customer information
* Search and filter customer records
* Assign customers to Sales Representatives
* Record customer contact information
* Record the customer's preferred payment method
* Deactivate customer records
* Restore previously deactivated customers

Customer information includes the details required by the business for customer administration and subsequent sales transactions.

Supported payment methods include:

* EFT
* Cash
* COD
* Up-Front Payment

### Sales Representative Assignment

Customers can be associated with a Sales Representative.

This creates a relationship between customer management and the sales process and allows the business to identify the representative responsible for a particular customer.

Conceptually:

```text
Sales Representative
        ↓
     Customer
        ↓
     Quotation
        ↓
      Invoice
        ↓
      Payment
```

---

## 3. Supplier Management

The Supplier Management module allows authorised users to maintain information about the suppliers used by Exclusive Distributors.

Supplier records provide the business with a central location for storing supplier information and maintaining the relationship between suppliers and the products they provide.

Authorised users can:

* Add new suppliers
* View supplier information
* Edit supplier information
* Search supplier records
* Maintain supplier contact details
* Deactivate suppliers
* Reactivate previously deactivated suppliers

### Supplier and Product Relationship

Suppliers can be associated with products within the system.

This creates the following relationship:

```text
Supplier
    ↓
 Product
    ↓
Inventory
```
---

## 4. Product Management

The Product Management module maintains the master information for the products sold by Exclusive Distributors.

Products are maintained separately from inventory because product information and stock information represent different business concepts.

A **Product** describes the item being sold, while **Inventory** represents the current stock position of that product.

Product information includes details such as:

* Product code
* Product name
* Description
* Category
* Supplier
* Cost excluding VAT
* Cost including VAT
* Selling price
* Active status

Authorised users can:

* Add new products
* View product information
* Edit existing products
* Search and filter products
* Associate products with suppliers
* Maintain product pricing information
* Discontinue products
* Reactivate previously discontinued products

### Product Pricing

The system maintains product cost and selling-price information that can subsequently be used throughout the sales workflow.

Financial values are stored using decimal values to provide appropriate precision for currency-related information.

### Selling Price Warning

When creating or editing a product, the system checks whether the entered selling price is lower than the product's cost excluding VAT.

If:

```text
Selling Price < Cost Excluding VAT
```
---

## 5. Inventory Management

The Inventory Management module maintains information about the stock held by Exclusive Distributors.

Inventory is linked to products through `ProductId`, allowing the system to maintain product master information separately from stock quantities that change during normal business operations.

Inventory information includes details such as:

* Product
* Quantity on hand
* Reorder level
* Notes
* Stock-related information

Authorised users can view and manage inventory information and monitor the current stock position of products.

### Quantity on Hand

The **Quantity on Hand** represents the current quantity of a product available within inventory.

This value can change as stock-related transactions take place.

One of the most important integrations in the system occurs between **Invoices and Inventory**.

The process is:

```text
Quotation
     ↓
Convert to Invoice
     ↓
Pending Invoice
     ↓
Administrator Approval
     ↓
Inventory Quantity Deducted
```
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

The Quotation Management module allows authorised users to prepare quotations for customers before a sale progresses to the invoice stage.

A quotation represents a proposed transaction and contains the customer, relevant products, quantities and pricing information required for the proposed sale.

Authorised users can:

* Create quotations
* View quotation details
* Edit current quotations
* Search and filter quotations
* Generate quotation documents
* Send applicable quotation information through the email functionality
* Convert quotations into invoices
* View previously converted quotations in Quotation History

### Quotation Workflow

The quotation process forms the beginning of the application's main sales workflow.

The final workflow is:

```text
Customer
   ↓
Quotation Created
   ↓
Quotation Reviewed / Edited
   ↓
Convert to Invoice
   ↓
Pending Invoice
```
---

## 8. Invoice Management

The Invoice Management module manages confirmed sales transactions after they progress beyond the quotation stage.

Invoices can be created through the quotation-to-invoice workflow and contain the customer, products, quantities, pricing and financial information required for the transaction.

The invoice workflow is connected directly to inventory and payment management, making it one of the central components of the system.

### Invoice Workflow

A newly created invoice does not immediately deduct stock or accept payments.

The invoice first enters a **Pending** state and must be approved by an Administrator.

The workflow is:

```text
Quotation
    ↓
Convert to Invoice
    ↓
Pending Invoice
    ↓
Administrator Approval
    ↓
Approved Invoice
    ↓
Inventory Deduction
    ↓
Payment(s)
    ↓
Partially Paid / Paid
    ↓
Invoice History
```
---

## 9. Payment Management

The Payment Management module records payments received against approved invoices and allows the business to track the outstanding balance of each transaction.

Payments are directly associated with invoices, creating a one-to-many relationship:

```text
Invoice
   ↓
Payments
   ├── Payment 1
   ├── Payment 2
   └── Payment 3
```

The payment status is determined from the relationship between the invoice total and the amount paid.

The main payment states are:

* **Unpaid**
* **Partially Paid**
* **Paid**

This means the payment status does not need to be manually guessed by the user. It can be calculated from the payment information stored in the system.

---

## 10. Employee Management

The Employee Management module allows Administrators to maintain information about employees working within Exclusive Distributors.

Employee information is maintained separately from authentication information while still allowing the employee administration process to work alongside the application's Identity-based user account system.

Administrators can:

* Add employees

* View employee information

* Edit employee information

* Search employee records

* Maintain employee contact and employment information

* Store next-of-kin information

* Deactivate employees

* View former employees

### Employee Numbers

Employee records use employee numbers to provide a consistent internal identifier.

The application automatically generates the required employee number when a new employee is created, reducing the need for users to manually determine the next available identifier.

### Next-of-Kin Information

Employee records can include next-of-kin information.

This allows relevant employee and emergency-contact information to be maintained within the employee administration area.

### Active and Former Employees

Employees who are no longer active within the organisation do not need to be permanently removed from the system.

Instead, the application distinguishes between:

```text

Active Employees

       ↓

   Deactivate

       ↓

Former Employees

```

This allows historical employee information to be retained while keeping the current employee list focused on active employees.

### Role-Based Access

Employee Management is restricted to:

* Administrators

This prevents normal Employees and Sales Representatives from accessing sensitive employee-administration functionality.

---

## 11. Sales Representative Management

The Sales Representative Management module allows Administrators to maintain the representatives responsible for customer and sales activities.

Sales Representatives form part of the wider sales workflow because customers can be associated with a particular representative.

Administrators can:

* Add Sales Representatives

* View Sales Representative information

* Edit Sales Representative information

* Search Sales Representative records

* Deactivate Sales Representatives

* View inactive Sales Representatives

### Sales Representative Numbers

Sales Representative records use automatically generated identifiers following the application's Sales Representative numbering convention.

For example:

```text

SR-001

SR-002

SR-003

```

Automatic generation provides a consistent identifier without requiring the Administrator to manually determine the next number.

### Customer Relationship

A Sales Representative can be associated with customers.

Conceptually:

```text

Sales Representative

        ↓

     Customer

        ↓

     Quotation

        ↓

      Invoice

        ↓

      Payment

```

This allows customer and transaction information to remain connected to the relevant sales responsibility.

### Active and Inactive Sales Representatives

Sales Representatives can be deactivated when they are no longer active without unnecessarily removing their historical information.

The application distinguishes between:

```text

Active Sales Representatives

            ↓

        Deactivate

            ↓

Inactive Sales Representatives

```

This preserves existing customer and transaction relationships while keeping the current list focused on active representatives.

### Role-Based Access

Sales Representative administration is restricted to:

* Administrators

This is separate from the **Sales Representative user role**, which determines the functionality a Sales Representative can access after logging into the system.

---

# Business Workflow

The core business workflow connects the main sales, inventory and payment modules.

```text

┌──────────────┐

│   Customer   │

└──────┬───────┘

       │

       ▼

┌──────────────┐

│  Quotation   │

│   Pending    │

└──────┬───────┘

       │

       │ Convert

       ▼

┌──────────────┐

│   Invoice    │

│   Pending    │

└──────┬───────┘

       │

       │ Administrator Approval

       ▼

┌──────────────┐

│   Approved   │

│   Invoice    │

└──────┬───────┘

       │

       ├──────────────────┐

       │                  │

       ▼                  ▼

┌──────────────┐   ┌──────────────┐

│  Inventory   │   │   Payment    │

│  Deduction   │   │   (s)       │

└──────────────┘   └──────┬───────┘

                          │

                          ▼

                   ┌──────────────┐

                   │ Partially    │

                   │ Paid / Paid  │

                   └──────┬───────┘

                          │

                          ▼

                   ┌──────────────┐

                   │   Invoice    │

                   │   History    │

                   └──────────────┘

```

The workflow demonstrates that the application's modules are connected rather than operating as independent CRUD pages.

A quotation can progress into an invoice without the transaction having to be manually recreated. The invoice then enters a Pending state and must be approved by an Administrator.

Inventory is deducted during the transition from Pending to Approved.

Payments can then be recorded against the approved invoice. Multiple payments are supported until the outstanding balance reaches zero, after which the invoice is displayed in Invoice History.

The original converted quotation is also retained in Quotation History, providing traceability across the transaction.

---

# VAT Rules

The application applies VAT according to the configured product/business rules.

Where a product or applicable category is treated as non-VAT:

```text

[NONE]

```

no VAT is applied.

For applicable VAT items, the system uses:

```text

15%

```

VAT and financial calculations are handled within the application's business logic rather than requiring users to manually calculate transaction totals.

Currency-related values use decimal data types to provide appropriate precision for financial information.

Where configured within the database model, financial values use precision such as:

```text

decimal(18,2)

```

This is more appropriate for financial calculations than floating-point data types.

---

# Forecasting

The system includes stock forecasting functionality to assist users with inventory planning.

Forecasting uses available product and inventory information to provide an indication of future stock requirements and products that may require attention.

The implementation uses a **Moving Average Forecast Strategy** rather than a machine-learning model.

The forecasting process considers applicable inventory information such as:

* Current stock

* Reorder level

* Product information

* Available inventory data

The system provides a **30-day stock forecast** to assist with identifying potential stock requirements.

Conceptually:

```text

Inventory Data

      ↓

Moving Average Strategy

      ↓

30-Day Forecast

      ↓

Stock Planning Information

```

The forecasting functionality forms part of the operational area of the application and is available to:

* Administrators

* Employees

The implementation is intentionally a heuristic forecasting approach and does not claim to provide machine-learning-based demand prediction.

The Strategy Pattern allows forecasting logic to remain separated from the controllers and user interface, providing a foundation for alternative forecasting approaches in future versions.

---

# Dashboard and Reporting

The system includes dashboard and reporting functionality to provide users with useful operational information derived from the data stored within the application.

## Dashboard

The Dashboard provides users with a central starting point after authentication.

Dashboard information is generated from system data and provides quick access to relevant business information and application modules.

Depending on the user's role, navigation and available functionality are restricted according to the application's authorisation rules.

Dashboard analytics provide a higher-level view of information maintained across areas such as customers, products, inventory and sales-related functionality.

## Reports

The system includes reporting functionality for reviewing operational and business information.

Reports are restricted to the appropriate administrative role because they may contain wider business information than normal operational users require.

Reporting functionality can use information from areas such as:

* Customers

* Quotations

* Invoices

* Payments

* Sales

* Tax/VAT-related information

* Inventory

Reports allow information already stored within the system to be presented in a more useful format without requiring users to manually gather records from multiple modules.

---

# Technology Stack

| Area                 | Technology                            |
| -------------------- | ------------------------------------- |
| Programming Language | C#                                    |
| Web Framework        | ASP.NET Core MVC                      |
| Runtime              | .NET 8                                |
| ORM / Data Access    | Entity Framework Core                 |
| Database             | Microsoft SQL Server                  |
| Development Database | SQL Server LocalDB/ Local development database                |
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
| Cloud Platform       | Microsoft Azure                       |

---

# System Architecture

The application follows a layered architecture that separates presentation, business logic and data access responsibilities.

The main MVC application follows the general flow:

```text

User

  ↓

Razor View

  ↓

Controller

  ↓

Service

  ↓

Repository

  ↓

ApplicationDbContext

  ↓

SQL Server / Azure SQL

```

For functionality requiring the separate API, the application can additionally communicate through HTTP:

```text

MVC Application

      ↓

HTTP Request

      ↓

Web API

      ↓

API Service

      ↓

External / Email Service

```

This separation prevents individual controllers or Views from becoming responsible for the entire application workflow.

---

## Views

The Views are responsible for the user interface.

The application uses Razor Views to display information and provide forms through which users can interact with the system.

Examples include:

* Dashboard
* Customer pages
* Supplier pages
* Product pages
* Inventory pages
* Employee pages
* Sales Representative pages
* Quotation pages
* Invoice pages
* Payment pages
* Reports
* Forecasting
* User account pages

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

* Customer operations
* Product operations
* Inventory operations
* Invoice approval
* Payment calculations
* Quote-to-invoice conversion
* Notifications
* Email communication
* Reporting
* Forecasting
* Export operations
* User administration

This keeps business rules separate from the user interface.

---

## Repositories

Repositories handle data-access operations.

They communicate with Entity Framework Core and the application's `ApplicationDbContext`.

Examples include repositories for:

* Customers
* Suppliers
* Products
* Inventory
* Quotations
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

The application uses ASP.NET Core's built-in Dependency Injection system.

Controllers and services depend on interfaces where appropriate rather than manually creating every implementation.

For example:

```text

Controller

    ↓

IInvoiceService

    ↓

InvoiceService

    ↓

IInvoiceRepository

    ↓

InvoiceRepository

```

Dependencies are registered with the application's service container and supplied to classes through constructor injection.

This reduces tight coupling and makes components easier to replace, maintain and unit test.

---

# Design Patterns

The project applies software design patterns where they provide a practical benefit to the system.

## Repository Pattern

The Repository Pattern separates data-access operations from application business logic.

For example:

```text

IProductRepository

        ↓

ProductRepository

        ↓

ApplicationDbContext

```

This allows services to work through repository abstractions instead of directly performing all Entity Framework operations.

---

## Strategy Pattern

The Strategy Pattern is used where functionality can be represented by interchangeable implementations.

It is applicable to functionality such as forecasting and export-related operations.

For example:

```text

Forecast Strategy

       ↓

Moving Average Strategy

```

and where applicable:

```text

Export Strategy

     ├── PDF

     └── Excel

```

This structure allows an implementation to be changed or extended without requiring the entire consuming workflow to be rewritten.

---

## Factory Pattern

Factory-based components are used where the application needs to select or construct the appropriate implementation for a particular operation.

For export-related functionality, this can allow the application to select the appropriate export implementation according to the required document type.

Conceptually:

```text

Export Request

      ↓

Export Factory

      ↓

Appropriate Export Strategy

```

This centralises implementation-selection logic instead of distributing it throughout multiple controllers.

---

## Observer-Style Notification Components

Notification functionality includes observer-style components designed to respond to relevant business events.

Conceptually:

```text

Business Event

      ↓

Notification Subject

      ↓

Observer(s)

      ↓

System / Email Notification

```

This supports separation between the business event itself and the actions that may occur in response to that event.

---

## Dependency Injection

Dependency Injection is used throughout the system to connect controllers, services, repositories and supporting components.

Although Dependency Injection is provided directly by ASP.NET Core, its use supports the wider design objective of reducing tight coupling between components.
---

# Project Structure

The solution contains the main MVC application, separate API and automated test project.

A simplified representation is:

```text

WIL-Project/

│

├── FMCGEnterpriseManagementSystem/

│   ├── Controllers/

│   ├── Data/

│   ├── DTOs/

│   ├── Enums/

│   ├── Factories/

│   ├── Helpers/

│   ├── Models/

│   ├── Repositories/

│   │   └── Interfaces/

│   ├── Services/

│   │   └── Interfaces/

│   ├── Strategies/

│   │   └── Interfaces/

│   ├── ViewModels/

│   ├── Views/

│   ├── wwwroot/

│   ├── Program.cs

│   └── appsettings.json

│

├── FMCGEnterpriseManagementSystem.API/

│   ├── Controllers/

│   ├── DTOs/

│   ├── Services/

│   └── Program.cs

│

├── FMCGEnterpriseManagementSystem.Tests/

│   ├── InvoiceTests.cs

│   ├── PaymentTests.cs

│   ├── QuoteTests.cs

│   ├── QuoteToInvoiceTests.cs

│   ├── ForecastingTests.cs

│   ├── SalesRepresentativeTests.cs

│   └── UserAccountTests.cs

│

├── Task 2 - Meeting Minutes/

│

├── README.md

└── [Solution File]

```

The project uses interfaces within the relevant layers to separate contracts from implementations.

For example:

```text

Repositories/

├── Interfaces/

│   └── IProductRepository.cs

└── ProductRepository.cs

```

and:

```text

Services/

├── Interfaces/

│   └── IProductService.cs

└── ProductService.cs

```

This makes the solution easier to navigate and supports Dependency Injection and automated testing.

---

# Database

The application uses a relational Microsoft SQL Server database with Entity Framework Core acting as the ORM between the application and the database.

During development, the application can use a local SQL Server database.

For the hosted environment, the application is designed to use **Azure SQL Database**.

Database schema changes are managed through **Entity Framework Core migrations**.

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

The application uses the following primary roles:

```text

Administrator

Employee

Sales Representative

```

Access to controller actions can be restricted using ASP.NET Core authorisation.

For example:

```csharp

[Authorize(Roles = "Administrator")]

```

or:

```csharp

[Authorize(Roles = "Administrator,Employee")]

```

This provides server-side access control rather than relying only on hidden menu items.

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

Before running the project locally, ensure that the required development tools are installed.

These include:

* Visual Studio

* A compatible .NET SDK

* Microsoft SQL Server / Local development SQL Server instance

* Git

* Entity Framework Core tooling where required

* Appropriate email configuration if email functionality is being tested

The exact SDK version should match the target framework configured within the project's `.csproj` files.

---

# Clone the Repository

Clone the repository using Git:

```bash

git clone https://github.com/iqran0906/WIL-Project.git

```

Move into the repository:

```bash

cd WIL-Project

```

The solution can then be opened in Visual Studio.

For normal development work, team members should use the agreed development workflow rather than making uncoordinated changes directly to the release branch.

---

# Database Configuration

The database connection is configured through the application's configuration system.

A local SQL Server configuration can be used during development, while the deployed application can use the hosted Azure SQL configuration.

Sensitive production connection information must not be committed directly to source control.

Production connection strings should be configured through the hosting environment.

---

# Applying Database Migrations

After configuring the development database, existing Entity Framework Core migrations can be applied using the appropriate EF Core tooling.

For example:

```bash

dotnet ef database update

```

Where Package Manager Console is being used in Visual Studio, the equivalent migration commands can also be used according to the configured startup and data project.

Because database entities are shared between multiple modules, migrations should be coordinated rather than independently recreated by each team member.

---

# Email Configuration

Email functionality requires the appropriate SMTP configuration.

Sensitive credentials such as SMTP usernames, passwords or application passwords should not be committed directly to the repository.

For development, secure local configuration such as .NET User Secrets can be used where supported by the project.

For hosted environments, credentials should be configured through the hosting platform's application settings or secret-management facilities.

---

# Running the Application

For local development, the solution can be run from Visual Studio.

Because the system contains both the MVC application and separate API, both components may need to be running when testing functionality that depends on API communication.

Visual Studio can be configured with multiple startup projects so that the MVC and API projects start together.

The normal development process is:

```text

Configure Database

       ↓

Apply Migrations

       ↓

Start MVC Application

       ↓

Start API

       ↓

Login

       ↓

Test Required Workflow

```

For the deployed version, the application and required services must use the hosted configuration rather than development-only localhost addresses.

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

Git and GitHub were used throughout the project to support collaborative development.

The team used feature branches so that work on individual modules could be developed without every team member editing the integrated code simultaneously.

The primary integration branch was:

```text

development

```

The release/deployment branch was:

```text

main

```

The overall workflow was:

```text

Feature Branch

      ↓

Development and Testing

      ↓

Commit

      ↓

Push to GitHub

      ↓

Pull Request / Review

      ↓

development

      ↓

Integration and Testing

      ↓

main

      ↓

Deployment
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
| **Naseeha** | ApplicationDbContext, EF Core migrations, database integration, and stock/batch data                               |
| **Maseeha** | Customer-facing CRUD/UI, employee and Sales Representative UI and model work, notifications and user-facing interactions, Product, Inventory, Supplier                        |
| **Imran**   | Quotation, Invoice and Payment workflows, related ViewModels/UI and financial/report presentation components                                    |

Although each member had primary ownership of particular modules, the final application depends on these modules working together.

---

# Integration Between Modules

The application was designed so that its major modules work together.

The primary operational relationship can be represented as:

```text

Supplier

   ↓

Product

   ↓

Inventory

   │

   └────────────────────┐

                        │

Customer                │

   ↓                    │

Quotation               │

   ↓                    │

Invoice ────────────────┘

   ↓

Approval

   ↓

Inventory Deduction

   ↓

Payment(s)

   ↓

Reports / Dashboard / History

```

Customer information flows through the sales process:

```text

Customer

   ↓

Quotation

   ↓

Invoice

   ↓

Payment

```

Product and stock information flows through the operational process:

```text

Supplier

   ↓

Product

   ↓

Inventory

   ↓

Invoice Approval

   ↓

Stock Deduction

```

This interconnected structure means that changes to one module can affect another.

Integration therefore required the team to consider more than whether each individual page worked independently.
---

# Example End-to-End Scenario

A typical transaction can progress through the system as follows.

### Step 1 — Customer

A customer is registered in the system.

For example:

```text

Customer:

ABC Retail Store

```

The customer can be assigned to a Sales Representative and the appropriate payment method can be recorded.

---

### Step 2 — Quotation

An authorised sales user creates a quotation for the customer.

The quotation contains the relevant products, quantities and pricing information.

The quotation initially remains part of the current quotation workflow.

---

### Step 3 — Quote-to-Invoice Conversion

When the transaction proceeds, the quotation is converted into an invoice.

The quotation is then marked as:

```text

Invoiced

```

and is retained within **Quotation History**.

The resulting invoice is created with:

```text

Status = Pending

```

---

### Step 4 — Invoice Approval

An Administrator reviews the pending invoice.

When the Administrator approves the invoice:

```text

Pending → Approved

```

the transaction is allowed to continue into the stock and payment stages.

---

### Step 5 — Inventory Deduction

The relevant inventory quantities are deducted when the invoice is approved.

For example:

```text

Quantity on Hand Before: 100

Invoice Quantity:          10

Quantity on Hand After:    90

```

This ensures that the inventory module reflects the approved sale.

---

### Step 6 — Payment

A payment can be recorded against the approved invoice.

For example:

```text

Invoice Total: R10,000.00

Payment:        R4,000.00

Amount Due:     R6,000.00

```

The invoice is therefore partially paid.

---

### Step 7 — Additional Payment

Additional payments can be recorded.

For example:

```text

Invoice Total:  R10,000.00

Total Paid:     R10,000.00

Amount Due:          R0.00

```

The invoice is now fully paid.

The system prevents another normal payment from being initiated against an invoice that has already been settled.

---

### Step 8 — Invoice History

Once the approved invoice has an outstanding balance of zero, it is displayed in **Invoice History**.

The transaction is therefore preserved rather than deleted.

The overall transaction can be traced as:

```text

Customer

   ↓

Quotation

   ↓

Quotation History

   │

   └──────→ Invoice

               ↓

            Approval

               ↓

        Inventory Deduction

               ↓

           Payment(s)

               ↓

          Invoice History

```
---

# Future Improvements

The current system provides a foundation that can be extended further.

Potential future improvements include:

* More advanced demand forecasting algorithms
* Machine-learning-based stock prediction
* More detailed management dashboards
* Additional notification channels
* SMS notifications
* More extensive audit logging
* Additional reporting and analytics filters
* Customer payment-history dashboards
* Supplier performance analysis
* Automated scheduled reports
* More advanced batch and expiry-management rules
* Expanded public or partner API functionality
* Additional integration and end-to-end automated tests
* Automated UI testing
* Advanced deployment pipelines
* Cloud-based application monitoring and alerting
* Performance and scalability optimisation
* Additional security hardening for production use

These improvements can build on the current layered architecture without requiring the entire system to be redesigned.

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

The application connects customers, suppliers, products, inventory, quotations, invoices and payments into a structured business workflow.

A customer transaction can progress from quotation through quote-to-invoice conversion, invoice approval, inventory deduction and multiple payments before the completed transaction is retained within invoice history.

Supporting functionality includes authentication, role-based authorisation, employee and Sales Representative administration, notifications, email communication, password recovery, PDF and Excel exports, reporting, dashboard analytics and stock forecasting.

The application uses a layered architecture to separate presentation, business logic and data-access responsibilities.

Repositories, services, strategies, factories, Dependency Injection, Entity Framework Core, ASP.NET Core Identity and a separate API component provide a structured technical foundation for the system.

The solution also includes automated unit testing and continuous-integration practices to support software quality during development and integration.

Most importantly, the project demonstrates the practical application of software-engineering principles within a realistic business scenario and reflects the collaborative development, integration, testing and deployment experience gained through the Work Integrated Learning project.

---

# License

This project was developed as an **academic Work Integrated Learning project**.

It is not intended for commercial redistribution or production use without further development, testing, security review and deployment configuration.

Copyright © 2026 Exclusive Distributors WIL Development Team.
