# FinanceTracker

> **Track your money. Own your future.**

FinanceTracker is a secure, privacy-first personal finance web application that helps individuals take full control of their financial health. Whether you want to track daily expenses, set monthly budgets, monitor your crypto portfolio or get a clear picture of your net worth — FinanceTracker brings it all together in one clean, intuitive interface.

---

## Team Members

| Name | Student Number | Role |
|------|---------------|------|
| Alex Yator | A00335392 | Backend Developer / Frontend Developer |
| Cherylpreet Bansal | A00336894 | Backend Developer / Frontend Developer |
| Noble Udecchukwu | A00335177 | Backend Developer / Frontend Developer |
| Audrey Goudi | A00335177 | Backend Developer / Frontend Developer |

---

## Project Overview

FinanceTracker is a full-stack personal finance application built with a modern **ASP.NET Core Web API** backend and a responsive **HTML/CSS/JavaScript** frontend. It was developed as a **Cambrian College Software Engineering Capstone Project** and is designed to be simple enough for everyday users while being powerful enough to give real financial insight.

### Who is it for?
- **Individuals** who want to understand where their money goes each month
- **Budget-conscious users** who need to set spending limits and get warned before exceeding them
- **Investors** who want to track their crypto portfolio alongside regular finances
- **Anyone** who wants a private, ad-free alternative to cloud-based finance apps

### What problem does it solve?
Most people have no clear picture of their financial health. They don't know how much they spend on groceries vs dining out, whether they're on track with their budget, or what their total net worth looks like across all their accounts. FinanceTracker solves this by putting all that information in one place — clearly, securely and without ads.

---
##  Features

| # | Feature | Description | Primary | Secondary |
|---|---------|-------------|---------|-----------|
| 1 | **Expense Tracking** | Log and categorise every transaction with default or custom categories. Filter by date range and category. Export to CSV. | Alex Yator | Cherylpreet Bansal |
| 2 | **Monthly Budgets & Alerts** | Set monthly spending limits per category. Real-time utilization tracking with Ok / Warning / Exceeded status badges and progress bars. | Cherylpreet Bansal | Noble Udecchukwu |
| 3 | **Income Tracking** | Log income by type (Salary, Freelance, Investment, etc.). Custom income types supported. Income deposited into accounts updates Net Worth automatically. | Noble Udecchukwu | Audrey Goudi |
| 4 | **Net Worth Dashboard** | Aggregates bank accounts, crypto portfolio and income into a single net worth figure. Displays monthly cash flow and savings rate. | Audrey Goudi | Alex Yator |
| 5 | **Crypto Portfolio** | Manually track digital assets (BTC, ETH, SOL, etc.) with average buy price, current mock price, total value and profit/loss calculation. | Alex Yator | Noble Udecchukwu |
| 6 | **Password Reset via Email** | Secure forgot-password flow using SendGrid. One-time reset links expire after 30 minutes. Password strength enforced on register and reset. | Alex Yator | Audrey Goudi |

---

## 🔄 Application Flow

### Step 1 — Register & Login
The user creates an account with a valid email and a strong password (min 8 characters, uppercase, lowercase and a number). On successful registration the user is automatically logged in and a JWT token is issued for the session.

```
┌─────────────────────────────────────┐
│         FinanceTracker              │
│   Track your money, own your future │
├─────────────────────────────────────┤
│  [ Sign In ]     [ Register ]       │
│                                     │
│  EMAIL                              │
│  ┌─────────────────────────────┐    │
│  │ you@example.com             │    │
│  └─────────────────────────────┘    │
│  PASSWORD                           │
│  ┌─────────────────────────────┐    │
│  │ ••••••••               👁   │    │
│  └─────────────────────────────┘    │
│                                     │
│  ┌─────────────────────────────┐    │
│  │         Sign In             │    │
│  └─────────────────────────────┘    │
│                                     │
│      Forgot your password?          │
│         Try Demo Mode               │
└─────────────────────────────────────┘
```

### Step 2 — Set Up Categories & Budgets
The user navigates to **Manage** to review default expense categories (Rent, Groceries, Transport, etc.) and add custom ones. They then go to **Budgets** to set monthly spending limits per category.

```
┌─────────────────────────────────────┐
│  Manage                             │
├─────────────────────────────────────┤
│  EXPENSE CATEGORIES    [+ New]      │
│  ┌──────────────┐ ┌──────────────┐  │
│  │ Rent Default │ │Groceries Def │  │
│  └──────────────┘ └──────────────┘  │
│  ┌──────────────┐ ┌──────────────┐  │
│  │Transport Def │ │ Fuel  Default│  │
│  └──────────────┘ └──────────────┘  │
│                                     │
│  INCOME TYPES          [+ New]      │
│  ┌──────────────┐ ┌──────────────┐  │
│  │ Salary  Def  │ │Freelance Def │  │
│  └──────────────┘ └──────────────┘  │
└─────────────────────────────────────┘
```
### Step 3 — Log Income & Expenses
The user logs their income specifying the source, type and which bank account to deposit into — automatically updating their Net Worth. They then log daily expenses assigning each to a category.

```
┌─────────────────────────────────────┐
│  Add Income                         │
├─────────────────────────────────────┤
│  AMOUNT ($)                         │
│  ┌─────────────────────────────┐    │
│  │ 5800.00                     │    │
│  └─────────────────────────────┘    │
│  SOURCE                             │
│  ┌─────────────────────────────┐    │
│  │ Monthly Salary              │    │
│  └─────────────────────────────┘    │
│  TYPE          DEPOSIT INTO         │
│  ┌──────────┐  ┌───────────────┐    │
│  │ Salary ▼ │  │Chase Checking │    │
│  └──────────┘  └───────────────┘    │
│                                     │
│  [Cancel]          [Save]           │
└─────────────────────────────────────┘
```
### Step 4 — Monitor Net Worth & Dashboard
The user visits the **Net Worth** page to see total assets, monthly income vs expenses and savings rate. The **Dashboard** shows budget utilization per category with progress bars and status badges.

```
┌─────────────────────────────────────┐
│  Net Worth                          │
├─────────────────────────────────────┤
│        TOTAL NET WORTH              │
│          $105,431.30                │
│                                     │
│  Total Assets    $105,431.30        │
│  Monthly Income  $7,320.00          │
│  Monthly Expenses $654.38           │
│                                     │
│  ASSET BREAKDOWN                    │
│  Fidelity Brokerage   $54,780.50    │
│  Marcus Savings       $38,200.00    │
│  Chase Checking       $12,450.80    │
│                                     │
│  CASH FLOW                          │
│  Income      $7,320.00              │
│  Expenses   -$654.38                │
│  Net Flow   +$6,665.62              │
│  Savings Rate    91%                │
└─────────────────────────────────────┘
```

---

##  Task Board (Agile)

| Task | Status | Sprint |
|------|--------|--------|
| Set up Clean Architecture project structure |  Done | 1 |
| Implement JWT authentication & registration |  Done | 1 |
| Build expense CRUD endpoints |  Done | 1 |
| Build category management endpoints |  Done | 1 |
| Build monthly budget endpoints with utilization |  Done | 1 |
| Build dashboard monthly summary endpoint |  Done | 1 |
| Build crypto portfolio endpoints |  Done | 1 |
| CSV export for expenses |  Done | 1 |
| Migrate from InMemory to SQLite database |  Done | 2 |
| Frontend HTML/CSS/JS single-file UI |  Done | 2 |
| Net Worth dashboard with accounts & income |  Done | 2 |
| Forgot password flow with SendGrid email |  Done | 2 |
| Income tracking with account balance update |  Done | 2 |
| Default expense categories & income types |  Done | 2 |
| About page with mission & privacy policy |  Done | 2 |
| Password visibility toggle (eye icon) |  Done | 2 |
| Password strength validation on register |  Done | 3 |
| Last login timestamp tracking |  Done | 3 |
| Custom income type from income form |  Done | 3 |
| README documentation |  In Progress | 3 |
| Notifications (budget warnings, monthly summary) |  To Do | 3 |
| Deploy application to production |  To Do | 3 |

---
##  Console UI Mockups

### Screen 1 — Login / Auth Screen
```
╔═════════════════════════════════════════╗
║           FinanceTracker                ║
║   Track your money, own your future.    ║
╠═════════════════════════════════════════╣
║  [ Sign In ]        [ Register ]        ║
╠═════════════════════════════════════════╣
║                                         ║
║  EMAIL                                  ║
║  ┌───────────────────────────────────┐  ║
║  │  you@example.com                  │  ║
║  └───────────────────────────────────┘  ║
║                                         ║
║  PASSWORD                               ║
║  ┌───────────────────────────────────┐  ║
║  │  ••••••••                    👁   │  ║
║  └───────────────────────────────────┘  ║
║                                         ║
║  ┌───────────────────────────────────┐  ║
║  │            Sign In                │  ║
║  └───────────────────────────────────┘  ║
║                                         ║
║         Forgot your password?           ║
║                                         ║
║  ┌───────────────────────────────────┐  ║
║  │         Try Demo Mode             │  ║
║  └───────────────────────────────────┘  ║
╠═════════════════════════════════════════╣
║  FinanceTracker  •  Finance@track.com   ║
║   Encrypted     Real-time     No Ads    ║
╚═════════════════════════════════════════╝
```
### Screen 2 — Dashboard
```
╔═════════════════════════════════════════╗
║   Finance    [Net Worth] [Dashboard]    ║
║               [Income]   [Expenses]     ║
║               [Budgets]  [Accounts]     ║
║               [Crypto]   [Categories]   ║
╠═════════════════════════════════════════╣
║  Dashboard        March 2026 Overview   ║
╠══════════════╦══════════╦═══════════════╣
║Total Spending║Tot Budget║  Utilization  ║
║   $654.38    ║ $1,050   ║    62.3%      ║
╠══════════════╩══════════╩═══════════════╣
║  TOP SPENDING CATEGORIES                ║
║  1. Groceries           $241.70         ║
║  2. Utilities           $120.00         ║
║  3. Shopping             $75.30         ║
╠═════════════════════════════════════════╣
║  BUDGET UTILIZATION                     ║
║  Groceries   ████████░░░░  60%      Ok  ║
║  Dining Out  ████░░░░░░░░  44%      Ok  ║
║  Transport   ████░░░░░░░░  41%      Ok  ║
║  Subscript.  █████████░░░  92%     Warn ║
╚═════════════════════════════════════════╝
```

### Screen 3 — Expense Entry
```
╔═════════════════════════════════════════╗
║  Add Expense                            ║
╠═════════════════════════════════════════╣
║                                         ║
║  AMOUNT ($)                             ║
║  ┌───────────────────────────────────┐  ║
║  │  89.00                            │  ║
║  └───────────────────────────────────┘  ║
║                                         ║
║  DATE                                   ║
║  ┌───────────────────────────────────┐  ║
║  │  2026-03-16                       │  ║
║  └───────────────────────────────────┘  ║
║                                         ║
║  DESCRIPTION                            ║
║  ┌───────────────────────────────────┐  ║
║  │  Restaurant dinner                │  ║
║  └───────────────────────────────────┘  ║
║                                         ║
║  CATEGORY                               ║
║  ┌───────────────────────────────────┐  ║
║  │  Dining Out                    ▼  │  ║
║  └───────────────────────────────────┘  ║
║                                         ║
║       [Cancel]          [Save]          ║
╚═════════════════════════════════════════╝
```
### Screen 4 — Net Worth
```
╔═════════════════════════════════════════╗
║  Net Worth                              ║
║  Your complete financial picture        ║
╠═════════════════════════════════════════╣
║                                         ║
║           TOTAL NET WORTH               ║
║            $105,431.30                  ║
║                                         ║
╠══════════════╦══════════╦═══════════════╣
║ Total Assets ║ Monthly  ║   Monthly     ║
║ $105,431.30  ║ Income   ║   Expenses    ║
║              ║$7,320.00 ║   $654.38     ║
╠══════════════╩══════════╩═══════════════╣
║          ASSET BREAKDOWN                ║
║     Fidelity Brokerage   $54,780.50     ║
║     Marcus Savings        $38,200.00    ║
║     Chase Checking        $12,450.80    ║
╠═════════════════════════════════════════╣
║  CASH FLOW THIS MONTH                   ║
║  Income             +$7,320.00          ║
║  Expenses            -$654.38           ║
║  ─────────────────────────────          ║
║  Net Cash Flow      +$6,665.62          ║
║  Savings Rate            91%            ║
╚═════════════════════════════════════════╝
```
##  Project Structure

```
FinanceTracker/
│
├── FinanceTracker.sln
├── README.md
├── finance-tracker-ui.html              ← Single-file frontend
│
├── FinanceTracker.Domain/               ← Entities only. No dependencies.
│   └── Entities/
│       ├── User.cs
│       ├── Category.cs
│       ├── Expense.cs
│       ├── MonthlyBudget.cs
│       └── CryptoAsset.cs
│
├── FinanceTracker.Application/          ← Business logic. Depends on Domain only.
│   ├── DTOs/           Dtos.cs
│   ├── Interfaces/     IInterfaces.cs
│   ├── Mappings/       MappingProfile.cs
│   ├── Common/         Exceptions.cs
│   └── Services/
│       ├── AuthService.cs
│       ├── CategoryService.cs
│       ├── ExpenseService.cs
│       ├── BudgetService.cs
│       ├── DashboardService.cs
│       └── CryptoService.cs
│
├── FinanceTracker.Infrastructure/       ← EF Core, Repositories, SendGrid
│   ├── Data/           AppDbContext.cs
│   ├── Migrations/
│   ├── Repositories/   Repositories.cs
│   └── Services/
│       ├── EmailService.cs
│       └── MockCryptoPriceService.cs
│
└── FinanceTracker.API/                  ← ASP.NET Core host
    ├── Controllers/
    ├── Middleware/      ExceptionMiddleware.cs
    ├── Program.cs
    ├── appsettings.Example.json         ← Safe to commit
    └── appsettings.json                 ← Gitignored (contains secrets)
```

---

## Setup & Installation

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Git](https://git-scm.com/download/win)
- [VS Code](https://code.visualstudio.com) or Visual Studio 2022
- [Live Server Extension](https://marketplace.visualstudio.com/items?itemName=ritwickdey.LiveServer) for VS Code

### Step 1 — Clone the repository
```bash
git clone https://github.com/YatorAlexKe/Expense_Tracking-Budget_Control_App.git
cd Expense_Tracking-Budget_Control_App
```
### Step 2 — Configure appsettings.json
Copy the example config and fill in your values:
```bash
copy FinanceTracker.API/appsettings.Example.json FinanceTracker.API/appsettings.json
```

Then open `appsettings.json` and add your SendGrid API key and verified sender email.

### Step 3 — Install EF Core tools
```bash
dotnet tool install --global dotnet-ef
```

### Step 4 — Restore packages and run migrations
```bash
dotnet restore
cd FinanceTracker.API
dotnet ef database update --project ../FinanceTracker.Infrastructure --startup-project .
```

### Step 5 — Run the API
```bash
dotnet run
```
API is available at: `http://localhost:5000`
Swagger UI is available at: `http://localhost:5000/swagger`

### Step 6 — Open the frontend
Right click `finance-tracker-ui.html` in VS Code → **Open with Live Server**

---

## Environment Variables

| Key | Description |
|-----|-------------|
| `SendGrid:ApiKey` | Your SendGrid API key (starts with SG.) |
| `SendGrid:FromEmail` | Your verified sender email address |
| `SendGrid:FromName` | Display name for outgoing emails |
| `ClientUrl` | Frontend base URL e.g. http://localhost:5500 |
| `Jwt:Key` | Secret key for JWT token signing (min 32 chars) |

---

## Sources

| # | Source | Used For |
|---|--------|----------|
| 1 | [Microsoft ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core) | Clean Architecture setup, JWT authentication, EF Core migrations |
| 2 | [SendGrid C# Documentation](https://docs.sendgrid.com/for-developers/sending-email/v3-csharp-code-example) | Email service integration for password reset |
| 3 | [Entity Framework Core Docs](https://learn.microsoft.com/en-us/ef/core) | SQLite database setup, migrations, repository pattern |
| 4 | [BCrypt.Net-Next NuGet Package](https://github.com/BcryptNet/bcrypt.net) | Secure password hashing on register and reset |
| 5 | [JWT.io](https://jwt.io/introduction) | Understanding JWT structure and claims for authentication |
| 6 | [Lucide Icons](https://lucide.dev) | SVG icons used throughout the frontend UI |
| 7 | Claude AI (Anthropic) | Used to assist with code generation, architecture decisions, debugging and UI design throughout the project |

---

## License

This project was built for educational purposes as part of the Cambrian College Software Engineering program — Capstone 2026.

---

*Built by the FinanceTracker Team — Cambrian College, Sudbury, Ontario 🇨🇦*
