using FinanceTracker.Application.Interfaces;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Infrastructure.Repositories;

// ── User ─────────────────────────────────────────────────────────────────────

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;
    public UserRepository(AppDbContext db) => _db = db;

    public Task<User?> GetByEmailAsync(string email) =>
        _db.Users.FirstOrDefaultAsync(u => u.Email == email);

    public Task<User?> GetByIdAsync(Guid id) =>
        _db.Users.FindAsync(id).AsTask();

    public Task<User?> GetByResetTokenAsync(string token) =>
        _db.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == token);

    public async Task AddAsync(User user) => await _db.Users.AddAsync(user);

    public Task SaveChangesAsync() => _db.SaveChangesAsync();
}

// ── Category ─────────────────────────────────────────────────────────────────

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _db;
    public CategoryRepository(AppDbContext db) => _db = db;

    public Task<IEnumerable<Category>> GetByUserIdAsync(Guid userId) =>
        Task.FromResult<IEnumerable<Category>>(
            _db.Categories.Where(c => c.UserId == userId).AsEnumerable());

    public Task<Category?> GetByIdAsync(Guid id) =>
        _db.Categories.FindAsync(id).AsTask();

    public async Task AddAsync(Category cat) => await _db.Categories.AddAsync(cat);
    public Task UpdateAsync(Category cat) { _db.Categories.Update(cat); return Task.CompletedTask; }
    public Task DeleteAsync(Category cat) { _db.Categories.Remove(cat); return Task.CompletedTask; }
    public Task SaveChangesAsync() => _db.SaveChangesAsync();
}

// ── Expense ──────────────────────────────────────────────────────────────────

public class ExpenseRepository : IExpenseRepository
{
    private readonly AppDbContext _db;
    public ExpenseRepository(AppDbContext db) => _db = db;

    public async Task<IEnumerable<Expense>> GetFilteredAsync(
        Guid userId, DateTime? from, DateTime? to, Guid? categoryId)
    {
        var q = _db.Expenses
            .Include(e => e.Category)
            .Where(e => e.UserId == userId);

        if (from.HasValue)       q = q.Where(e => e.Date >= from.Value);
        if (to.HasValue)         q = q.Where(e => e.Date <= to.Value);
        if (categoryId.HasValue) q = q.Where(e => e.CategoryId == categoryId.Value);

        return await q.OrderByDescending(e => e.Date).ToListAsync();
    }

    public Task<Expense?> GetByIdAsync(Guid id) =>
        _db.Expenses.Include(e => e.Category).FirstOrDefaultAsync(e => e.Id == id);

    public async Task<IEnumerable<Expense>> GetByUserAndMonthAsync(Guid userId, int month, int year) =>
        await _db.Expenses
            .Include(e => e.Category)
            .Where(e => e.UserId == userId && e.Date.Month == month && e.Date.Year == year)
            .ToListAsync();

    public async Task AddAsync(Expense e) => await _db.Expenses.AddAsync(e);
    public Task UpdateAsync(Expense e) { _db.Expenses.Update(e); return Task.CompletedTask; }
    public Task DeleteAsync(Expense e) { _db.Expenses.Remove(e); return Task.CompletedTask; }
    public Task SaveChangesAsync() => _db.SaveChangesAsync();
}

// ── Budget ───────────────────────────────────────────────────────────────────

public class BudgetRepository : IBudgetRepository
{
    private readonly AppDbContext _db;
    public BudgetRepository(AppDbContext db) => _db = db;

    public async Task<IEnumerable<MonthlyBudget>> GetByUserAndMonthAsync(Guid userId, int month, int year) =>
        await _db.MonthlyBudgets
            .Include(b => b.Category)
            .Where(b => b.UserId == userId && b.Month == month && b.Year == year)
            .ToListAsync();

    public Task<MonthlyBudget?> GetByIdAsync(Guid id) =>
        _db.MonthlyBudgets.Include(b => b.Category).FirstOrDefaultAsync(b => b.Id == id);

    public Task<MonthlyBudget?> GetByCategoryMonthYearAsync(Guid userId, Guid categoryId, int month, int year) =>
        _db.MonthlyBudgets.Include(b => b.Category)
            .FirstOrDefaultAsync(b => b.UserId == userId && b.CategoryId == categoryId
                                    && b.Month == month && b.Year == year);

    public async Task AddAsync(MonthlyBudget b) => await _db.MonthlyBudgets.AddAsync(b);
    public Task UpdateAsync(MonthlyBudget b) { _db.MonthlyBudgets.Update(b); return Task.CompletedTask; }
    public Task DeleteAsync(MonthlyBudget b) { _db.MonthlyBudgets.Remove(b); return Task.CompletedTask; }
    public Task SaveChangesAsync() => _db.SaveChangesAsync();
}

// ── Crypto ───────────────────────────────────────────────────────────────────

public class CryptoRepository : ICryptoRepository
{
    private readonly AppDbContext _db;
    public CryptoRepository(AppDbContext db) => _db = db;

    public async Task<IEnumerable<CryptoAsset>> GetByUserIdAsync(Guid userId) =>
        await _db.CryptoAssets.Where(a => a.UserId == userId).ToListAsync();

    public Task<CryptoAsset?> GetByIdAsync(Guid id) =>
        _db.CryptoAssets.FindAsync(id).AsTask();

    public async Task AddAsync(CryptoAsset a) => await _db.CryptoAssets.AddAsync(a);
    public Task UpdateAsync(CryptoAsset a) { _db.CryptoAssets.Update(a); return Task.CompletedTask; }
    public Task DeleteAsync(CryptoAsset a) { _db.CryptoAssets.Remove(a); return Task.CompletedTask; }
    public Task SaveChangesAsync() => _db.SaveChangesAsync();
}
