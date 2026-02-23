namespace FinanceTracker.Domain.Entities;

public class CryptoAsset
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Symbol { get; set; } = string.Empty;          // e.g. BTC, ETH
    public decimal Quantity { get; set; }
    public decimal AveragePurchasePrice { get; set; }
    public DateTime PurchaseDate { get; set; }
    public Guid UserId { get; set; }

    // Navigation
    public User User { get; set; } = null!;
}
