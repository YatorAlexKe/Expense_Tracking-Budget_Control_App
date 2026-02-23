using AutoMapper;
using FinanceTracker.Application.DTOs;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Category
        CreateMap<Category, CategoryResponse>();

        // Expense
        CreateMap<Expense, ExpenseResponse>()
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name));

        // Budget (no budget utilisation calculated here — done in service)
        CreateMap<MonthlyBudget, BudgetResponse>()
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name))
            .ForMember(d => d.AmountSpent, o => o.Ignore())
            .ForMember(d => d.UtilizationPercent, o => o.Ignore())
            .ForMember(d => d.Status, o => o.Ignore());

        // Crypto
        CreateMap<CryptoAsset, CryptoAssetResponse>();
    }
}
