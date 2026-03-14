using FinanceTracker.Application.DTOs;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Interfaces;

// ── Repositories ──────────────────────────────────────────────────────────────

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByResetTokenAsync(string token);
    Task AddAsync(User user);
    Task SaveChangesAsync();
}

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetByUserIdAsync(Guid userId);
    Task<Category?> GetByIdAsync(Guid id);
    Task AddAsync(Category category);
    Task UpdateAsync(Category category);
    Task DeleteAsync(Category category);
    Task SaveChangesAsync();
}

public interface IExpenseRepository
{
    Task<IEnumerable<Expense>> GetFilteredAsync(Guid userId, DateTime? from, DateTime? to, Guid? categoryId);
    Task<Expense?> GetByIdAsync(Guid id);
    Task<IEnumerable<Expense>> GetByUserAndMonthAsync(Guid userId, int month, int year);
    Task AddAsync(Expense expense);
    Task UpdateAsync(Expense expense);
    Task DeleteAsync(Expense expense);
    Task SaveChangesAsync();
}

public interface IBudgetRepository
{
    Task<IEnumerable<MonthlyBudget>> GetByUserAndMonthAsync(Guid userId, int month, int year);
    Task<MonthlyBudget?> GetByIdAsync(Guid id);
    Task<MonthlyBudget?> GetByCategoryMonthYearAsync(Guid userId, Guid categoryId, int month, int year);
    Task AddAsync(MonthlyBudget budget);
    Task UpdateAsync(MonthlyBudget budget);
    Task DeleteAsync(MonthlyBudget budget);
    Task SaveChangesAsync();
}

public interface ICryptoRepository
{
    Task<IEnumerable<CryptoAsset>> GetByUserIdAsync(Guid userId);
    Task<CryptoAsset?> GetByIdAsync(Guid id);
    Task AddAsync(CryptoAsset asset);
    Task UpdateAsync(CryptoAsset asset);
    Task DeleteAsync(CryptoAsset asset);
    Task SaveChangesAsync();
}

// ── Services ──────────────────────────────────────────────────────────────────

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task ForgotPasswordAsync(ForgotPasswordRequest request);
    Task ResetPasswordAsync(ResetPasswordRequest request);
}

public interface ICategoryService
{
    Task<IEnumerable<CategoryResponse>> GetAllAsync(Guid userId);
    Task<CategoryResponse> CreateAsync(Guid userId, CategoryRequest request);
    Task<CategoryResponse> UpdateAsync(Guid userId, Guid id, CategoryRequest request);
    Task DeleteAsync(Guid userId, Guid id);
}

public interface IExpenseService
{
    Task<IEnumerable<ExpenseResponse>> GetFilteredAsync(Guid userId, ExpenseFilterRequest filter);
    Task<ExpenseResponse> CreateAsync(Guid userId, CreateExpenseRequest request);
    Task<ExpenseResponse> UpdateAsync(Guid userId, Guid id, UpdateExpenseRequest request);
    Task DeleteAsync(Guid userId, Guid id);
    Task<byte[]> ExportCsvAsync(Guid userId, ExpenseFilterRequest filter);
}

public interface IBudgetService
{
    Task<IEnumerable<BudgetResponse>> GetMonthlyBudgetsAsync(Guid userId, int month, int year);
    Task<BudgetResponse> SetBudgetAsync(Guid userId, BudgetRequest request);
    Task<BudgetResponse> UpdateBudgetAsync(Guid userId, Guid id, BudgetRequest request);
    Task DeleteBudgetAsync(Guid userId, Guid id);
}

public interface IDashboardService
{
    Task<MonthlySummaryResponse> GetMonthlySummaryAsync(Guid userId);
}

public interface ICryptoService
{
    Task<IEnumerable<CryptoAssetResponse>> GetAllAsync(Guid userId);
    Task<CryptoAssetResponse> CreateAsync(Guid userId, CreateCryptoRequest request);
    Task<CryptoAssetResponse> UpdateAsync(Guid userId, Guid id, UpdateCryptoRequest request);
    Task DeleteAsync(Guid userId, Guid id);
    Task<PortfolioSummaryResponse> GetPortfolioSummaryAsync(Guid userId);
}

/// <summary>
/// Mock price service — replace with a real exchange API in production.
/// </summary>
public interface ICryptoPriceService
{
    Task<decimal> GetCurrentPriceAsync(string symbol);
}

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(string toEmail, string resetLink);
}
