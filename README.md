# TransactionProject

[![.NET](https://img.shields.io/badge/.NET-10-blue)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-14-blueviolet)](https://learn.microsoft.com/dotnet/csharp/)

## Overview

TransactionProject is a Razor Pages-based inventory transaction management system. It provides secure role-based management for inbound and outbound transactions, product stock control, PDF and Excel export, and user management via ASP.NET Core Identity.

## Key Features

- CRUD for transactions and products
- Role-based authentication and authorization (Admin, Supplier) via ASP.NET Core Identity
- PDF reports using `QuestPDF`
- Excel exports using `EPPlus`
- AutoMapper for DTO/view-model mapping
- EF Core migrations and seed data for initial roles/users
- Clean separation: `Transaction.PL` (presentation), `Transaction.BLL` (business), `Transaction.DAL` (data)

## Tech Stack

- .NET 10
- C# 14
- ASP.NET Core (Razor Pages)
- Entity Framework Core (SQL Server)
- ASP.NET Core Identity
- QuestPDF (PDF generation)
- EPPlus (Excel export)
- AutoMapper

## Prerequisites

- .NET 10 SDK
- SQL Server (or LocalDB)
- Visual Studio 2022/2023 or __Visual Studio Code__
- Optional: `dotnet-ef` global tool for migrations

## Quick Start

1. Clone repository:

2. Update the connection string in `Transaction.PL/appsettings.json` (key: `ConnectionStrings:DevConn`) to point to your SQL Server instance.

3. Apply EF Core migrations and seed database:

4. Run the application:

Open the URL printed by the host (typically `https://localhost:5001`).

## Configuration

- Identity options (password policy, lockout, sign-in) are configured in `Transaction.PL/Program.cs`.
- QuestPDF license is configured in `Program.cs` via `QuestPDF.Settings.License`.
- Seed data and default roles/users are implemented in `Transaction.DAL/SeedData/IdentityContextSeedData.cs`.

## Database & Migrations

- Migrations are stored in `Transaction.DAL/Migrations`.
- Add a migration:
- Update database: