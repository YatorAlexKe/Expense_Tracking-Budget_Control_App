using FinanceTracker.Application.Interfaces;

namespace FinanceTracker.Infrastructure.Services;

/// <summary>
/// Mock price service with hard-coded prices.
/// Replace with a call to CoinGecko / Binance / etc. in production.
/// </summary>
public class MockCryptoPriceService : ICryptoPriceService
{
    private static readonly Dictionary<string, decimal> Prices = new(StringComparer.OrdinalIgnoreCase)
    {
        ["BTC"]  = 68_500m,
        ["ETH"]  = 3_800m,
        ["SOL"]  = 175m,
        ["BNB"]  = 605m,
        ["ADA"]  = 0.65m,
        ["DOT"]  = 9.5m,
        ["DOGE"] = 0.18m,
    };

    public Task<decimal> GetCurrentPriceAsync(string symbol)
    {
        // Fall back to 0 if symbol is unknown — surface as no current price data
        var price = Prices.TryGetValue(symbol, out var p) ? p : 0m;
        return Task.FromResult(price);
    }
}
