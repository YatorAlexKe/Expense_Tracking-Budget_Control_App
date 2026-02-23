namespace FinanceTracker.Domain.Entities;

public class Expense
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public Guid UserId { get; set; }

    // Navigation
    public Category Category { get; set; } = null!;
    public User User { get; set; } = null!;
}
