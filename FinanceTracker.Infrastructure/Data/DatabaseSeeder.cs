using FinanceTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        try
        {
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Check if data already exists
            if (await context.Users.AnyAsync())
                return; // Database already seeded

            // Demo User
            var demoUser = new User
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Email = "demo@financetracker.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Demo@1234"),
                CreatedAt = DateTime.UtcNow
            };

            await context.Users.AddAsync(demoUser);
            await context.SaveChangesAsync();

            // Categories
            var categories = new[]
            {
                new Category { Id = Guid.NewGuid(), Name = "Groceries", UserId = demoUser.Id },
                new Category { Id = Guid.NewGuid(), Name = "Transportation", UserId = demoUser.Id },
                new Category { Id = Guid.NewGuid(), Name = "Utilities", UserId = demoUser.Id },
                new Category { Id = Guid.NewGuid(), Name = "Entertainment", UserId = demoUser.Id },
                new Category { Id = Guid.NewGuid(), Name = "Health", UserId = demoUser.Id }
            };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();

            // Sample expenses
            var groceriesCategory = categories.First(c => c.Name == "Groceries");
            var transportCategory = categories.First(c => c.Name == "Transportation");

            var expenses = new[]
            {
                new Expense
                {
                    Id = Guid.NewGuid(),
                    Amount = 85.50m,
                    Date = DateTime.UtcNow.AddDays(-5),
                    Description = "Weekly grocery shopping",
                    CategoryId = groceriesCategory.Id,
                    UserId = demoUser.Id
                },
                new Expense
                {
                    Id = Guid.NewGuid(),
                    Amount = 45.00m,
                    Date = DateTime.UtcNow.AddDays(-3),
                    Description = "Gas",
                    CategoryId = transportCategory.Id,
                    UserId = demoUser.Id
                },
                new Expense
                {
                    Id = Guid.NewGuid(),
                    Amount = 120.00m,
                    Date = DateTime.UtcNow.AddDays(-1),
                    Description = "Farmers market produce",
                    CategoryId = groceriesCategory.Id,
                    UserId = demoUser.Id
                }
            };

            await context.Expenses.AddRangeAsync(expenses);
            await context.SaveChangesAsync();

            // Sample monthly budgets
            var now = DateTime.UtcNow;
            var budgets = new[]
            {
                new MonthlyBudget
                {
                    Id = Guid.NewGuid(),
                    UserId = demoUser.Id,
                    CategoryId = groceriesCategory.Id,
                    Month = now.Month,
                    Year = now.Year,
                    BudgetAmount = 400.00m
                },
                new MonthlyBudget
                {
                    Id = Guid.NewGuid(),
                    UserId = demoUser.Id,
                    CategoryId = transportCategory.Id,
                    Month = now.Month,
                    Year = now.Year,
                    BudgetAmount = 200.00m
                }
            };

            await context.MonthlyBudgets.AddRangeAsync(budgets);
            await context.SaveChangesAsync();

            // Sample crypto assets
            var cryptoAssets = new[]
            {
                new CryptoAsset
                {
                    Id = Guid.NewGuid(),
                    UserId = demoUser.Id,
                    Symbol = "BTC",
                    Quantity = 0.5m,
                    AveragePurchasePrice = 45000.00m,
                    PurchaseDate = DateTime.UtcNow.AddDays(-30)
                },
                new CryptoAsset
                {
                    Id = Guid.NewGuid(),
                    UserId = demoUser.Id,
                    Symbol = "ETH",
                    Quantity = 5.0m,
                    AveragePurchasePrice = 2500.00m,
                    PurchaseDate = DateTime.UtcNow.AddDays(-30)
                }
            };

            await context.CryptoAssets.AddRangeAsync(cryptoAssets);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database seeding failed: {ex.Message}");
            throw;
        }
    }
}
