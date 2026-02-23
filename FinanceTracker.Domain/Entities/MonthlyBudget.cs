namespace FinanceTracker.Domain.Entities;

public class MonthlyBudget
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CategoryId { get; set; }
    public int Month { get; set; }   // 1–12
    public int Year { get; set; }
    public decimal BudgetAmount { get; set; }
    public Guid UserId { get; set; }

    // Navigation
    public Category Category { get; set; } = null!;
    public User User { get; set; } = null!;
}
