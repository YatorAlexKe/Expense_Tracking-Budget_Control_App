using FinanceTracker.Application.DTOs;
using FinanceTracker.Application.Interfaces;

namespace FinanceTracker.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IExpenseRepository _expenseRepo;
    private readonly IBudgetRepository _budgetRepo;

    public DashboardService(IExpenseRepository expenseRepo, IBudgetRepository budgetRepo)
    {
        _expenseRepo = expenseRepo;
        _budgetRepo = budgetRepo;
    }

    public async Task<MonthlySummaryResponse> GetMonthlySummaryAsync(Guid userId)
    {
        var now = DateTime.UtcNow;
        var expenses = await _expenseRepo.GetByUserAndMonthAsync(userId, now.Month, now.Year);
        var budgets  = await _budgetRepo.GetByUserAndMonthAsync(userId, now.Month, now.Year);

        var totalSpending = expenses.Sum(e => e.Amount);
        var totalBudget   = budgets.Sum(b => b.BudgetAmount);
        var utilization   = totalBudget > 0
            ? Math.Round(totalSpending / totalBudget * 100, 2)
            : 0;

        // Top 3 categories by spend
        var topCategories = expenses
            .GroupBy(e => e.Category?.Name ?? "Unknown")
            .OrderByDescending(g => g.Sum(e => e.Amount))
            .Take(3)
            .Select(g => new CategorySpend(g.Key, g.Sum(e => e.Amount)));

        return new MonthlySummaryResponse(totalSpending, totalBudget, utilization, topCategories);
    }
}
