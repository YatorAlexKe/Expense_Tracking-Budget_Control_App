using FinanceTracker.Application.Interfaces;
using FinanceTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.API;

/// <summary>
/// Background service that runs on the 1st of every month
/// and sends a monthly summary email to all verified users.
/// </summary>
public class MonthlyReportJob : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<MonthlyReportJob> _logger;

    public MonthlyReportJob(IServiceProvider services, ILogger<MonthlyReportJob> logger)
    {
        _services = services;
        _logger   = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Monthly Report Job started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            var now     = DateTime.UtcNow;
            var nextRun = GetNextRunTime(now);
            var delay   = nextRun - now;

            _logger.LogInformation("Next monthly report scheduled for: {NextRun}", nextRun);

            // Wait until the 1st of next month
            await Task.Delay(delay, stoppingToken);

            if (!stoppingToken.IsCancellationRequested)
            {
                await SendMonthlyReportsAsync();
            }
        }
    }

    private static DateTime GetNextRunTime(DateTime now)
    {
        // Run at 8:00 AM UTC on the 1st of every month
        var next = new DateTime(now.Year, now.Month, 1, 8, 0, 0, DateTimeKind.Utc)
                       .AddMonths(1);
        return next;
    }

    private async Task SendMonthlyReportsAsync()
    {
        _logger.LogInformation("Sending monthly summary emails...");

        using var scope = _services.CreateScope();

        var db       = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var email    = scope.ServiceProvider.GetRequiredService<IEmailService>();

        // Get previous month
        var now       = DateTime.UtcNow;
        var lastMonth = now.AddMonths(-1);
        var month     = lastMonth.Month;
        var year      = lastMonth.Year;
        var monthName = lastMonth.ToString("MMMM");

        // Get all verified users
        var users = await db.Users
            .Where(u => u.IsEmailVerified)
            .ToListAsync();

        _logger.LogInformation("Sending reports to {Count} users for {Month} {Year}",
            users.Count, monthName, year);

        foreach (var user in users)
        {
            try
            {
                // Get expenses for last month
                var expenses = await db.Expenses
                    .Include(e => e.Category)
                    .Where(e => e.UserId == user.Id
                             && e.Date.Month == month
                             && e.Date.Year  == year)
                    .ToListAsync();

                // Get budgets for last month
                var budgets = await db.MonthlyBudgets
                    .Where(b => b.UserId == user.Id
                             && b.Month  == month
                             && b.Year   == year)
                    .ToListAsync();

                var totalSpending = expenses.Sum(e => e.Amount);
                var totalBudget   = budgets.Sum(b => b.BudgetAmount);
                var utilization   = totalBudget > 0
                    ? Math.Round(totalSpending / totalBudget * 100, 1)
                    : 0;

                var budgetStatus = utilization >= 100 ? "Exceeded"
                                 : utilization >= 80  ? "Warning"
                                 : "On Track";

                // Top 3 categories
                var topCategories = expenses
                    .GroupBy(e => e.Category?.Name ?? "Uncategorized")
                    .Select(g => new CategorySpendItem
                    {
                        Name   = g.Key,
                        Amount = g.Sum(e => e.Amount)
                    })
                    .OrderByDescending(c => c.Amount)
                    .Take(3)
                    .ToList();

                var data = new MonthlySummaryEmailData
                {
                    UserEmail      = user.Email,
                    MonthName      = monthName,
                    Year           = year,
                    TotalSpending  = totalSpending,
                    TotalBudget    = totalBudget,
                    TotalIncome    = 0, // stored in localStorage — not in DB yet
                    NetSavings     = -totalSpending, // expenses only for now
                    UtilizationPct = utilization,
                    BudgetStatus   = budgetStatus,
                    TopCategories  = topCategories
                };

                await email.SendMonthlySummaryEmailAsync(user.Email, data);
                _logger.LogInformation("Monthly summary sent to {Email}", user.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send monthly summary to {Email}", user.Email);
            }
        }

        _logger.LogInformation("Monthly summary emails completed.");
    }
}