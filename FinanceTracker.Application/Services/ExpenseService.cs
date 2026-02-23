using System.Globalization;
using System.Text;
using AutoMapper;
using FinanceTracker.Application.Common;
using FinanceTracker.Application.DTOs;
using FinanceTracker.Application.Interfaces;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Services;

public class ExpenseService : IExpenseService
{
    private readonly IExpenseRepository _repo;
    private readonly ICategoryRepository _catRepo;
    private readonly IMapper _mapper;

    public ExpenseService(IExpenseRepository repo, ICategoryRepository catRepo, IMapper mapper)
    {
        _repo = repo;
        _catRepo = catRepo;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ExpenseResponse>> GetFilteredAsync(Guid userId, ExpenseFilterRequest filter)
    {
        var expenses = await _repo.GetFilteredAsync(userId, filter.From, filter.To, filter.CategoryId);
        return _mapper.Map<IEnumerable<ExpenseResponse>>(expenses);
    }

    public async Task<ExpenseResponse> CreateAsync(Guid userId, CreateExpenseRequest request)
    {
        // Ensure category belongs to user
        await AssertCategoryOwnershipAsync(userId, request.CategoryId);

        var expense = new Expense
        {
            Amount = request.Amount,
            Date = request.Date,
            Description = request.Description,
            CategoryId = request.CategoryId,
            UserId = userId
        };

        await _repo.AddAsync(expense);
        await _repo.SaveChangesAsync();

        // Reload with navigation for mapping
        var saved = await _repo.GetByIdAsync(expense.Id);
        return _mapper.Map<ExpenseResponse>(saved!);
    }

    public async Task<ExpenseResponse> UpdateAsync(Guid userId, Guid id, UpdateExpenseRequest request)
    {
        var expense = await GetOwnedOrThrowAsync(userId, id);
        await AssertCategoryOwnershipAsync(userId, request.CategoryId);

        expense.Amount = request.Amount;
        expense.Date = request.Date;
        expense.Description = request.Description;
        expense.CategoryId = request.CategoryId;

        await _repo.UpdateAsync(expense);
        await _repo.SaveChangesAsync();

        var updated = await _repo.GetByIdAsync(expense.Id);
        return _mapper.Map<ExpenseResponse>(updated!);
    }

    public async Task DeleteAsync(Guid userId, Guid id)
    {
        var expense = await GetOwnedOrThrowAsync(userId, id);
        await _repo.DeleteAsync(expense);
        await _repo.SaveChangesAsync();
    }

    /// <summary>
    /// Export filtered expenses as CSV bytes.
    /// </summary>
    public async Task<byte[]> ExportCsvAsync(Guid userId, ExpenseFilterRequest filter)
    {
        var expenses = await _repo.GetFilteredAsync(userId, filter.From, filter.To, filter.CategoryId);

        var sb = new StringBuilder();
        sb.AppendLine("Id,Amount,Date,Description,Category");

        foreach (var e in expenses)
        {
            // Escape description for CSV (wrap in quotes, escape inner quotes)
            var desc = $"\"{e.Description.Replace("\"", "\"\"")}\"";
            sb.AppendLine($"{e.Id},{e.Amount.ToString(CultureInfo.InvariantCulture)},{e.Date:yyyy-MM-dd},{desc},{e.Category.Name}");
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private async Task<Expense> GetOwnedOrThrowAsync(Guid userId, Guid id)
    {
        var expense = await _repo.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Expense), id);

        if (expense.UserId != userId)
            throw new ForbiddenException();

        return expense;
    }

    private async Task AssertCategoryOwnershipAsync(Guid userId, Guid categoryId)
    {
        var cat = await _catRepo.GetByIdAsync(categoryId)
            ?? throw new NotFoundException(nameof(Category), categoryId);

        if (cat.UserId != userId)
            throw new ForbiddenException();
    }
}
