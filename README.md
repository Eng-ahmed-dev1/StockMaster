# 🏢 Transaction Management System

<div align="center">

![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)
![Bootstrap](https://img.shields.io/badge/Bootstrap-7952B3?style=for-the-badge&logo=bootstrap&logoColor=white)

### A comprehensive inventory and transaction management system built with ASP.NET Core MVC

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg)](http://makeapullrequest.com)

</div>

---

## 📋 Table of Contents

- [Overview](#-overview)
- [Features](#-features)
- [Technology Stack](#-technology-stack)
- [Getting Started](#-getting-started)
- [Installation](#-installation)
- [Configuration](#-configuration)
- [Database Schema](#-database-schema)
- [User Roles](#-user-roles)
- [Security](#-security)
- [License](#-license)
- [Contact](#-contact)

---

## 🌟 Overview

**Transaction Management System** is an enterprise-grade MVC web application for streamlined inventory management, supplier relationships, and transaction tracking.

### Key Highlights

- ✅ **Role-Based Access Control**: Admin and User roles
- ✅ **Real-Time Inventory**: Automatic stock updates
- ✅ **PDF Reports**: Professional reports with QuestPDF
- ✅ **Audit Trail**: Complete transaction history
- ✅ **Responsive Design**: Mobile-friendly UI
- ✅ **Secure Authentication**: ASP.NET Core Identity

---

## ✨ Features

### 🔐 Authentication & Authorization
- Secure user registration and login
- Role-based access (Admin/User)
- Password management with strength requirements
- Account lockout after 5 failed attempts
- Profile management with photo upload

### 📦 Product Management
- Complete CRUD operations
- Unique SKU validation
- Real-time stock level tracking
- Product search and filtering
- Transaction history per product

### 🔄 Transaction Management
- **Inbound Transactions**: Receiving inventory (increases stock)
- **Outbound Transactions**: Selling inventory (decreases stock)
- Automatic stock level adjustments
- Transaction editing with stock recalculation
- Export to PDF (All, Selected, Single)

### 👥 Supplier Management
- Supplier registration and profiles
- Contact information management
- Transaction history per supplier
- Active/Inactive status tracking

### 📊 PDF Reports
- Professional document generation with QuestPDF
- Statistical summaries (Inbound, Outbound, Total)
- Color-coded transaction types
- Multiple export options

---

## 🛠 Technology Stack

| Category | Technologies |
|----------|-------------|
| **Framework** | .NET 6.0+, ASP.NET Core MVC |
| **Language** | C# 10.0+ |
| **ORM** | Entity Framework Core 6.0+ |
| **Database** | SQL Server 2019+ |
| **Authentication** | ASP.NET Core Identity |
| **Frontend** | HTML5, CSS3, JavaScript, Bootstrap 5, jQuery |
| **PDF Generation** | QuestPDF |
| **Mapping** | AutoMapper |

---

## 🚀 Getting Started

### Prerequisites

- **.NET SDK** 6.0 or higher
- **SQL Server** 2019+ or SQL Server Express
- **Visual Studio** 2022 (recommended) or VS Code
- **Git**

### Verify Installation

```bash
dotnet --version  # Should show 6.0.x or higher
```

---

## 📥 Installation

### Step 1: Clone Repository

```bash
git clone https://github.com/yourusername/transaction-management-system.git
cd transaction-management-system
```

### Step 2: Restore Packages

```bash
dotnet restore
```

### Step 3: Configure Database

Edit `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=TransactionManagementDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}
```

### Step 4: Install EF Tools

```bash
dotnet tool install --global dotnet-ef
```

### Step 5: Create Database

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Step 6: Seed Initial Data

Create `DbInitializer.cs`:

```csharp
public static class DbInitializer
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<SystemUsers>>();

        // Create roles
        string[] roleNames = { "Admin", "User" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Create admin user
        var adminEmail = "ahmed@gmail.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            var admin = new SystemUsers
            {
                UserName = "ahmed@gmail.com",
                Email = "ahmed@gmail.com",
                FullName = "Ahmed Alaa",
                EmailConfirmed = true,
                IsActive = true
            };

            var result = await userManager.CreateAsync(admin, "P@ssw0rd");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}
```

Add to `Program.cs`:

```csharp
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await DbInitializer.Initialize(services);
}
```

### Step 7: Run Application

```bash
dotnet run
# Or press F5 in Visual Studio
```

### Step 8: Access Application

Navigate to: `https://localhost:5001`

**Default Admin Credentials:**
```
Email: ahmed@gmail.com
Password: P@ssw0rd
```

**⚠️ Important**: Change the password after first login!

---

## ⚙️ Configuration

### Application Settings

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=TransactionManagementDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Identity": {
    "Password": {
      "RequireDigit": true,
      "RequireLowercase": true,
      "RequireUppercase": true,
      "RequireNonAlphanumeric": true,
      "RequiredLength": 6
    },
    "Lockout": {
      "DefaultLockoutTimeSpan": "00:05:00",
      "MaxFailedAccessAttempts": 5
    }
  }
}
```

---

## 🗄️ Database Schema

### Main Tables

**SystemUsers (AspNetUsers)**
- Id, UserName, Email, PasswordHash
- FullName, PhoneNumber, Address, City, Country
- ProfilePicture, IsActive, LastLoginDate

**Products**
- ProductId (PK), ProductName, SKU (Unique)
- Price, StockLevel, Description

**Transactions**
- TransactionId (PK), ProductId (FK)
- CreatedBy (FK), SupplierId (FK)
- Quantity, TransactionType, TransactionDate

### Relationships

```
Products (1) ──→ (N) Transactions
SystemUsers (1) ──→ (N) Transactions [CreatedBy]
SystemUsers (1) ──→ (N) Transactions [SupplierId]
```

---

## 👥 User Roles

### Admin Role
- ✅ Manage all products
- ✅ Create/Edit/Delete any transaction
- ✅ View all suppliers
- ✅ Export all reports
- ✅ Create Inbound and Outbound transactions

### User/Supplier Role
- ✅ View all products
- ✅ Create Inbound transactions (for self)
- ✅ View own transactions
- ✅ Edit own profile
- ❌ Cannot create Outbound transactions
- ❌ Cannot view other users' data

---

## 🔒 Security Features

### Authentication
- ASP.NET Core Identity with PBKDF2 password hashing
- Secure cookie-based authentication
- Session timeout management

### Authorization
- Role-based access control
- Claims-based identity
- Controller and action-level authorization

### Data Protection
- SQL injection prevention (EF Core parameterized queries)
- XSS protection (Razor automatic encoding)
- CSRF protection with anti-forgery tokens
- HTTPS enforcement

### Password Security
- Minimum 6 characters
- Requires uppercase, lowercase, digit, special character
- Account lockout after 5 failed attempts (5 minutes)

---

## 📸 Screenshots

### Dashboard
<div align="center">
<img src="https://github.com/Eng-ahmed-dev1/TransactionHandling_Project/blob/d5702a0e7576b09efb3e098c58980d0464dc602c/Images/Light-ModeTransactionDashBoard.png" alt="Dashboard">
</div>

### Product Management
<div align="center">
<img src="https://github.com/Eng-ahmed-dev1/TransactionHandling_Project/blob/d5702a0e7576b09efb3e098c58980d0464dc602c/Images/Light-ModeProductDashBoard.png" alt="Products">
</div>

### Profile
<div align="center">
<img src="" alt="Transactions">
</div>

### PDF Export
<div align="center">
<img src="https://github.com/Eng-ahmed-dev1/TransactionHandling_Project/blob/d5702a0e7576b09efb3e098c58980d0464dc602c/Images/PDF.png" alt="PDF Report">
</div>

---

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/YourFeature`)
3. Commit changes (`git commit -m 'Add YourFeature'`)
4. Push to branch (`git push origin feature/YourFeature`)
5. Open a Pull Request

---

## 📜 License

This project is licensed under the **MIT License**.

```
Copyright (c) 2024 Ahmed Alaa

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
```

---

## 📞 Contact

### Ahmed Alaa - Full Stack .NET Developer

<div align="center">


[![GitHub](https://img.shields.io/badge/GitHub-100000?style=for-the-badge&logo=github&logoColor=white)](https://github.com/Eng-ahmed-dev1)
[![LinkedIn](https://img.shields.io/badge/LinkedIn-0077B5?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/ahmed-alaa-b256a2389/)
[![Telegram](https://img.shields.io/badge/Telegram-2CA5E0?style=for-the-badge&logo=telegram&logoColor=white)](https://t.me/devAhmedl)

</div>

| Platform | Link |
|----------|------|
| 🐙 **GitHub** | [Eng-ahmed-dev1](https://github.com/Eng-ahmed-dev1) |
| 💼 **LinkedIn** | [Ahmed Alaa](https://www.linkedin.com/in/ahmed-alaa-b256a2389/) |
| 💬 **Telegram** | [@devAhmedl](https://t.me/devAhmedl) |

---

<div align="center">

## ⭐ Star This Repository

If you find this project useful, please give it a star!

[![GitHub stars](https://img.shields.io/github/stars/Eng-ahmed-dev1/transaction-management-system?style=social)](https://github.com/Eng-ahmed-dev1/transaction-management-system)

---

### Made with ❤️ by [Ahmed Alaa](https://github.com/Eng-ahmed-dev1)

**© 2024 Transaction Management System. All Rights Reserved.**

</div>
