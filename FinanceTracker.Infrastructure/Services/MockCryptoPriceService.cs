using System.Text.Json;
using FinanceTracker.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace FinanceTracker.Infrastructure.Services;

/// <summary>
/// Fetches real-time crypto prices from Binance public API.
/// No API key required. Updates in real time.
/// </summary>
public class CoinGeckoPriceService : ICryptoPriceService
{
    private readonly HttpClient _http;
    private readonly ILogger<CoinGeckoPriceService> _logger;

    // Cache prices for 60 seconds to avoid hitting rate limits
    private readonly Dictionary<string, (decimal Price, DateTime FetchedAt)> _cache = new();
    private readonly TimeSpan _cacheDuration = TimeSpan.FromSeconds(60);

    public CoinGeckoPriceService(HttpClient http, ILogger<CoinGeckoPriceService> logger)
    {
        _http   = http;
        _logger = logger;
    }

    public async Task<decimal> GetCurrentPriceAsync(string symbol)
    {
        var upperSymbol = symbol.ToUpper();

        // ── Check cache first ─────────────────────────────────
        if (_cache.TryGetValue(upperSymbol, out var cached) &&
            DateTime.UtcNow - cached.FetchedAt < _cacheDuration)
        {
            _logger.LogInformation("Cache hit for {Symbol}: {Price}", upperSymbol, cached.Price);
            return cached.Price;
        }

        try
        {
            // ── Call Binance API ──────────────────────────────
            // Binance uses trading pairs e.g. BTCUSDT, ETHUSDT
            var pair     = $"{upperSymbol}USDT";
            var url      = $"https://api.binance.com/api/v3/ticker/price?symbol={pair}";

            _logger.LogInformation("Fetching price for {Symbol} from Binance", upperSymbol);

            var response = await _http.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Binance returned {Status} for {Symbol}", response.StatusCode, upperSymbol);
                return GetFallbackPrice(upperSymbol);
            }

            var json     = await response.Content.ReadAsStringAsync();
            var doc      = JsonDocument.Parse(json);
            var priceString = doc.RootElement.GetProperty("price").GetString();
            var price       = decimal.Parse(priceString!, System.Globalization.CultureInfo.InvariantCulture);

            // ── Cache the result ──────────────────────────────
            _cache[upperSymbol] = (price, DateTime.UtcNow);

            _logger.LogInformation("Fetched {Symbol} price: {Price}", upperSymbol, price);
            return price;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch price for {Symbol}", upperSymbol);
            return GetFallbackPrice(upperSymbol);
        }
    }

    /// <summary>
    /// Fallback prices if Binance is unreachable.
    /// These are approximate values — not real time.
    /// </summary>
    private static decimal GetFallbackPrice(string symbol) => symbol.ToUpper() switch
    {
        "BTC"  => 65000m,
        "ETH"  => 3500m,
        "SOL"  => 150m,
        "BNB"  => 580m,
        "ADA"  => 0.45m,
        "XRP"  => 0.55m,
        "DOT"  => 7.5m,
        "DOGE" => 0.12m,
        "AVAX" => 35m,
        "MATIC"=> 0.85m,
        _      => 1m
    };
}