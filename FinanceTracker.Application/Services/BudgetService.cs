using AutoMapper;
using FinanceTracker.Application.Common;
using FinanceTracker.Application.DTOs;
using FinanceTracker.Application.Interfaces;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Services;

public class BudgetService : IBudgetService
{
    private readonly IBudgetRepository _repo;
    private readonly IExpenseRepository _expenseRepo;
    private readonly ICategoryRepository _catRepo;
    private readonly IMapper _mapper;

    public BudgetService(IBudgetRepository repo, IExpenseRepository expenseRepo,
        ICategoryRepository catRepo, IMapper mapper)
    {
        _repo = repo;
        _expenseRepo = expenseRepo;
        _catRepo = catRepo;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BudgetResponse>> GetMonthlyBudgetsAsync(Guid userId, int month, int year)
    {
        var budgets = await _repo.GetByUserAndMonthAsync(userId, month, year);
        var expenses = await _expenseRepo.GetByUserAndMonthAsync(userId, month, year);

        var spendByCat = expenses
            .GroupBy(e => e.CategoryId)
            .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));

        return budgets.Select(b => ToResponse(b, spendByCat.GetValueOrDefault(b.CategoryId, 0)));
    }

    public async Task<BudgetResponse> SetBudgetAsync(Guid userId, BudgetRequest request)
    {
        await AssertCategoryOwnershipAsync(userId, request.CategoryId);

        // Upsert: one budget per category/month/year
        var existing = await _repo.GetByCategoryMonthYearAsync(userId, request.CategoryId, request.Month, request.Year);
        if (existing is not null)
        {
            existing.BudgetAmount = request.BudgetAmount;
            await _repo.UpdateAsync(existing);
            await _repo.SaveChangesAsync();
            var spent1 = await GetSpentAsync(userId, request.CategoryId, request.Month, request.Year);
            return ToResponse(existing, spent1);
        }

        var budget = new MonthlyBudget
        {
            CategoryId = request.CategoryId,
            Month = request.Month,
            Year = request.Year,
            BudgetAmount = request.BudgetAmount,
            UserId = userId
        };

        await _repo.AddAsync(budget);
        await _repo.SaveChangesAsync();

        var saved = await _repo.GetByIdAsync(budget.Id);
        var spent = await GetSpentAsync(userId, request.CategoryId, request.Month, request.Year);
        return ToResponse(saved!, spent);
    }

    public async Task<BudgetResponse> UpdateBudgetAsync(Guid userId, Guid id, BudgetRequest request)
    {
        var budget = await GetOwnedOrThrowAsync(userId, id);
        budget.BudgetAmount = request.BudgetAmount;
        budget.Month = request.Month;
        budget.Year = request.Year;
        budget.CategoryId = request.CategoryId;

        await _repo.UpdateAsync(budget);
        await _repo.SaveChangesAsync();

        var spent = await GetSpentAsync(userId, budget.CategoryId, budget.Month, budget.Year);
        return ToResponse(budget, spent);
    }

    public async Task DeleteBudgetAsync(Guid userId, Guid id)
    {
        var budget = await GetOwnedOrThrowAsync(userId, id);
        await _repo.DeleteAsync(budget);
        await _repo.SaveChangesAsync();
    }

    // ── Budget status logic ───────────────────────────────────────────────────

    private static BudgetResponse ToResponse(MonthlyBudget b, decimal spent)
    {
        var pct = b.BudgetAmount > 0 ? Math.Round(spent / b.BudgetAmount * 100, 2) : 0;
        var status = pct switch
        {
            > 100 => BudgetStatus.Exceeded,
            >= 80  => BudgetStatus.Warning,
            _      => BudgetStatus.Ok
        };
        return new BudgetResponse(b.Id, b.CategoryId, b.Category?.Name ?? "", b.Month, b.Year,
            b.BudgetAmount, spent, pct, status);
    }

    private async Task<decimal> GetSpentAsync(Guid userId, Guid categoryId, int month, int year)
    {
        var expenses = await _expenseRepo.GetByUserAndMonthAsync(userId, month, year);
        return expenses.Where(e => e.CategoryId == categoryId).Sum(e => e.Amount);
    }

    private async Task<MonthlyBudget> GetOwnedOrThrowAsync(Guid userId, Guid id)
    {
        var budget = await _repo.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(MonthlyBudget), id);
        if (budget.UserId != userId) throw new ForbiddenException();
        return budget;
    }

    private async Task AssertCategoryOwnershipAsync(Guid userId, Guid categoryId)
    {
        var cat = await _catRepo.GetByIdAsync(categoryId)
            ?? throw new NotFoundException(nameof(Category), categoryId);
        if (cat.UserId != userId) throw new ForbiddenException();
    }
}
