using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Application.DTOs;

// ── Auth ────────────────────────────────────────────────────────────────────

public record RegisterRequest(
    [Required, EmailAddress] string Email,
    [Required, MinLength(6)] string Password);

public record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Password);

public record AuthResponse(string Token, string Email, Guid UserId, bool IsEmailVerified = true);
// ── Category ────────────────────────────────────────────────────────────────

public record CategoryRequest([Required, MaxLength(100)] string Name);

public record CategoryResponse(Guid Id, string Name, Guid UserId);

// ── Expense ─────────────────────────────────────────────────────────────────

public record CreateExpenseRequest(
    [Required] decimal Amount,
    [Required] DateTime Date,
    string Description,
    [Required] Guid CategoryId);

public record UpdateExpenseRequest(
    [Required] decimal Amount,
    [Required] DateTime Date,
    string Description,
    [Required] Guid CategoryId);

public record ExpenseResponse(
    Guid Id,
    decimal Amount,
    DateTime Date,
    string Description,
    Guid CategoryId,
    string CategoryName,
    Guid UserId);

public record ExpenseFilterRequest(
    DateTime? From,
    DateTime? To,
    Guid? CategoryId);

// ── MonthlyBudget ────────────────────────────────────────────────────────────

public record BudgetRequest(
    [Required] Guid CategoryId,
    [Required, Range(1, 12)] int Month,
    [Required, Range(2000, 2100)] int Year,
    [Required] decimal BudgetAmount);

public record BudgetResponse(
    Guid Id,
    Guid CategoryId,
    string CategoryName,
    int Month,
    int Year,
    decimal BudgetAmount,
    decimal AmountSpent,
    decimal UtilizationPercent,
    BudgetStatus Status);

public enum BudgetStatus { Ok, Warning, Exceeded }

// ── Dashboard ────────────────────────────────────────────────────────────────

public record MonthlySummaryResponse(
    decimal TotalSpending,
    decimal TotalBudget,
    decimal UtilizationPercent,
    IEnumerable<CategorySpend> TopCategories);

public record CategorySpend(string CategoryName, decimal Amount);

// ── Crypto ───────────────────────────────────────────────────────────────────

public record CreateCryptoRequest(
    [Required, MaxLength(10)] string Symbol,
    [Required] decimal Quantity,
    [Required] decimal AveragePurchasePrice,
    [Required] DateTime PurchaseDate);

public record UpdateCryptoRequest(
    [Required] decimal Quantity,
    [Required] decimal AveragePurchasePrice,
    [Required] DateTime PurchaseDate);

public record CryptoAssetResponse(
    Guid Id,
    string Symbol,
    decimal Quantity,
    decimal AveragePurchasePrice,
    DateTime PurchaseDate,
    Guid UserId);

public record PortfolioSummaryResponse(
    decimal TotalInvested,
    decimal CurrentValue,
    decimal ProfitLoss,
    decimal ProfitLossPercent,
    IEnumerable<CryptoPositionDetail> Positions);

public record CryptoPositionDetail(
    string Symbol,
    decimal Quantity,
    decimal AvgBuyPrice,
    decimal CurrentPrice,
    decimal CurrentValue,
    decimal ProfitLoss);
