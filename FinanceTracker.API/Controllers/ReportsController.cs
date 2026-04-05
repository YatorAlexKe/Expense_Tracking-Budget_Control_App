using FinanceTracker.Application.Interfaces;
using FinanceTracker.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IEmailService _email;
    private readonly ILogger<ReportsController> _logger;

    public ReportsController(AppDbContext db, IEmailService email, ILogger<ReportsController> logger)
    {
        _db     = db;
        _email  = email;
        _logger = logger;
    }

    /// <summary>Manually trigger monthly summary email for testing.</summary>
    [HttpPost("send-monthly-summary")]
    [Authorize]
    public async Task<IActionResult> SendMonthlySummary()
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var user   = await _db.Users.FindAsync(userId);

        if (user is null) return NotFound();

        // Use current month for manual trigger
        var now       = DateTime.UtcNow;
        var month     = now.Month;
        var year      = now.Year;
        var monthName = now.ToString("MMMM");

        var expenses = await _db.Expenses
            .Include(e => e.Category)
            .Where(e => e.UserId == userId
                     && e.Date.Month == month
                     && e.Date.Year  == year)
            .ToListAsync();

        var budgets = await _db.MonthlyBudgets
            .Where(b => b.UserId == userId
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
            TotalIncome    = 0,
            NetSavings     = -totalSpending,
            UtilizationPct = utilization,
            BudgetStatus   = budgetStatus,
            TopCategories  = topCategories
        };

        await _email.SendMonthlySummaryEmailAsync(user.Email, data);

        return Ok(new { message = $"Monthly summary sent to {user.Email}" });
    }
}