# FinanceTracker API

A **Clean Architecture** ASP.NET Core 8 Web API for personal finance tracking, including expense management, monthly budgets, a dashboard summary, CSV export, and a manual crypto portfolio tracker.

---

## Project Structure

```
FinanceTracker/
│
├── FinanceTracker.sln
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
│   ├── Interfaces/     IInterfaces.cs   (repository + service contracts)
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
├── FinanceTracker.Infrastructure/       ← EF Core, Repositories, external services.
│   ├── Data/           AppDbContext.cs
│   ├── Repositories/   Repositories.cs
│   └── Services/       MockCryptoPriceService.cs
│
└── FinanceTracker.API/                  ← ASP.NET Core host. Depends on all layers.
    ├── Controllers/
    │   ├── AuthController.cs
    │   ├── CategoriesController.cs      ← Full annotated example
    │   ├── ExpensesController.cs
    │   ├── BudgetsController.cs
    │   ├── DashboardController.cs
    │   └── CryptoController.cs
    ├── Middleware/      ExceptionMiddleware.cs
    ├── Program.cs
    └── appsettings.json
```

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- Any IDE: Visual Studio 2022, VS Code, or Rider

---

## Quick Start (In-Memory Database)

```bash
# 1. Clone / copy the solution
cd FinanceTracker

# 2. Restore packages
dotnet restore

# 3. Run the API project
cd FinanceTracker.API
dotnet run

# 4. Open Swagger UI
# http://localhost:5000  (Swagger is served at the root)
```

---

## Switching to SQL Server

1. Open `FinanceTracker.API/appsettings.json` and update the connection string.
2. In `Program.cs`, replace:
   ```csharp
   options.UseInMemoryDatabase("FinanceTrackerDb")
   ```
   with:
   ```csharp
   options.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
   ```
3. Add a migration and update the database:
   ```bash
   cd FinanceTracker.API
   dotnet ef migrations add InitialCreate --project ../FinanceTracker.Infrastructure
   dotnet ef database update
   ```

---

## Typical Usage Flow

### 1. Register
```
POST /api/auth/register
{ "email": "user@example.com", "password": "secret123" }
```

### 2. Login → copy the JWT token
```
POST /api/auth/login
{ "email": "user@example.com", "password": "secret123" }
```

### 3. Authorize in Swagger
Click the **Authorize 🔒** button in Swagger UI and paste your token.

### 4. Create a category
```
POST /api/categories
{ "name": "Groceries" }
```

### 5. Add an expense
```
POST /api/expenses
{ "amount": 75.50, "date": "2026-02-21", "description": "Weekly shop", "categoryId": "<guid>" }
```

### 6. Set a monthly budget
```
POST /api/budgets
{ "categoryId": "<guid>", "month": 2, "year": 2026, "budgetAmount": 400 }
```
Response includes `status: "Ok" | "Warning" | "Exceeded"` based on current spend.

### 7. Dashboard
```
GET /api/dashboard/monthly-summary
```

### 8. Export expenses as CSV
```
GET /api/expenses/export?from=2026-02-01&to=2026-02-28
```

### 9. Track crypto
```
POST /api/crypto
{ "symbol": "BTC", "quantity": 0.5, "averagePurchasePrice": 62000, "purchaseDate": "2024-01-15" }

GET /api/crypto/portfolio-summary
```

---

## Architecture Notes

| Principle | Implementation |
|---|---|
| Clean Architecture | Domain ← Application ← Infrastructure → API |
| No business logic in controllers | Controllers only call service methods |
| Data isolation | Every query is scoped to `UserId` from JWT |
| Global error handling | `ExceptionMiddleware` maps domain exceptions to HTTP codes |
| Async/await | All I/O methods are fully async |
| DTOs | Request/response objects never expose domain entities |
| AutoMapper | Maps entities to DTOs in `MappingProfile` |

---

## Extending the App

- **Real crypto prices**: Replace `MockCryptoPriceService` with an `HttpClient`-based implementation calling the CoinGecko API. Register it as `IHttpClientFactory` in `Program.cs`.
- **Email notifications**: Add an `IEmailService` interface to Application; implement in Infrastructure.
- **Recurring expenses**: Add a `RecurringExpense` entity and a background job (`IHostedService`).
- **Multi-currency**: Add a `Currency` field to `Expense` and inject an exchange rate service.
