using AutoMapper;
using FinanceTracker.Application.Common;
using FinanceTracker.Application.DTOs;
using FinanceTracker.Application.Interfaces;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Services;

public class CryptoService : ICryptoService
{
    private readonly ICryptoRepository _repo;
    private readonly ICryptoPriceService _prices;
    private readonly IMapper _mapper;

    public CryptoService(ICryptoRepository repo, ICryptoPriceService prices, IMapper mapper)
    {
        _repo = repo;
        _prices = prices;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CryptoAssetResponse>> GetAllAsync(Guid userId)
    {
        var assets = await _repo.GetByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<CryptoAssetResponse>>(assets);
    }

    public async Task<CryptoAssetResponse> CreateAsync(Guid userId, CreateCryptoRequest request)
    {
        var asset = new CryptoAsset
        {
            Symbol = request.Symbol.ToUpperInvariant(),
            Quantity = request.Quantity,
            AveragePurchasePrice = request.AveragePurchasePrice,
            PurchaseDate = request.PurchaseDate,
            UserId = userId
        };
        await _repo.AddAsync(asset);
        await _repo.SaveChangesAsync();
        return _mapper.Map<CryptoAssetResponse>(asset);
    }

    public async Task<CryptoAssetResponse> UpdateAsync(Guid userId, Guid id, UpdateCryptoRequest request)
    {
        var asset = await GetOwnedOrThrowAsync(userId, id);
        asset.Quantity = request.Quantity;
        asset.AveragePurchasePrice = request.AveragePurchasePrice;
        asset.PurchaseDate = request.PurchaseDate;
        await _repo.UpdateAsync(asset);
        await _repo.SaveChangesAsync();
        return _mapper.Map<CryptoAssetResponse>(asset);
    }

    public async Task DeleteAsync(Guid userId, Guid id)
    {
        var asset = await GetOwnedOrThrowAsync(userId, id);
        await _repo.DeleteAsync(asset);
        await _repo.SaveChangesAsync();
    }

    public async Task<PortfolioSummaryResponse> GetPortfolioSummaryAsync(Guid userId)
    {
        var assets = (await _repo.GetByUserIdAsync(userId)).ToList();

        // Fetch current prices for all unique symbols in parallel
        var symbols = assets.Select(a => a.Symbol).Distinct();
        var priceTasks = symbols.ToDictionary(s => s, s => _prices.GetCurrentPriceAsync(s));
        await Task.WhenAll(priceTasks.Values);
        var priceMap = priceTasks.ToDictionary(kv => kv.Key, kv => kv.Value.Result);

        var positions = assets.Select(a =>
        {
            var currentPrice  = priceMap.GetValueOrDefault(a.Symbol, a.AveragePurchasePrice);
            var currentValue  = currentPrice * a.Quantity;
            var costBasis     = a.AveragePurchasePrice * a.Quantity;
            return new CryptoPositionDetail(
                a.Symbol, a.Quantity, a.AveragePurchasePrice, currentPrice,
                currentValue, currentValue - costBasis);
        }).ToList();

        var totalInvested = positions.Sum(p => p.AvgBuyPrice * p.Quantity);
        var currentValue  = positions.Sum(p => p.CurrentValue);
        var profitLoss    = currentValue - totalInvested;
        var profitPct     = totalInvested > 0 ? Math.Round(profitLoss / totalInvested * 100, 2) : 0;

        return new PortfolioSummaryResponse(totalInvested, currentValue, profitLoss, profitPct, positions);
    }

    private async Task<CryptoAsset> GetOwnedOrThrowAsync(Guid userId, Guid id)
    {
        var asset = await _repo.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(CryptoAsset), id);
        if (asset.UserId != userId) throw new ForbiddenException();
        return asset;
    }
}
