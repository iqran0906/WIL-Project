IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Customers] (
    [CustomerID] nvarchar(450) NOT NULL,
    [CompanyName] nvarchar(max) NOT NULL,
    [ContactPerson] nvarchar(max) NOT NULL,
    [ContactNumber] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [PhysicalAddress] nvarchar(max) NOT NULL,
    [DeliveryAddress] nvarchar(max) NOT NULL,
    [CustomerGroup] nvarchar(max) NOT NULL,
    [PaymentTerms] nvarchar(max) NOT NULL,
    [PaymentMethod] nvarchar(max) NOT NULL,
    [VATNumber] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Customers] PRIMARY KEY ([CustomerID])
);
GO

CREATE TABLE [Inventories] (
    [InventoryID] nvarchar(450) NOT NULL,
    [ProductID] nvarchar(max) NOT NULL,
    [QuantityOnHand] int NOT NULL,
    [ReorderLevel] int NOT NULL,
    [ExpiryDate] datetime2 NOT NULL,
    CONSTRAINT [PK_Inventories] PRIMARY KEY ([InventoryID])
);
GO

CREATE TABLE [Products] (
    [ProductID] nvarchar(450) NOT NULL,
    [SupplierID] nvarchar(max) NOT NULL,
    [ProductCode] nvarchar(max) NOT NULL,
    [ProductName] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [Category] nvarchar(max) NOT NULL,
    [CostExVat] decimal(18,2) NOT NULL,
    [CostIncVat] decimal(18,2) NOT NULL,
    [SellingPrice] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY ([ProductID])
);
GO

CREATE TABLE [Roles] (
    [RoleID] nvarchar(450) NOT NULL,
    [RoleName] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY ([RoleID])
);
GO

CREATE TABLE [Suppliers] (
    [SupplierID] nvarchar(450) NOT NULL,
    [CompanyName] nvarchar(max) NOT NULL,
    [ContactPerson] nvarchar(max) NOT NULL,
    [ContactNumber] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [PhysicalAddress] nvarchar(max) NOT NULL,
    [CreditLimit] decimal(18,2) NOT NULL,
    [CreditTerms] nvarchar(max) NOT NULL,
    [VATNumber] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Suppliers] PRIMARY KEY ([SupplierID])
);
GO

CREATE TABLE [Users] (
    [UserID] nvarchar(450) NOT NULL,
    [RoleID] nvarchar(max) NOT NULL,
    [Username] nvarchar(max) NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([UserID])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260708152353_InitialFoundation', N'8.0.28');
GO

COMMIT;
GO

