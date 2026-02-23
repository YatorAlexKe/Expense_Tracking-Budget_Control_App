using AutoMapper;
using FinanceTracker.Application.Common;
using FinanceTracker.Application.DTOs;
using FinanceTracker.Application.Interfaces;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repo;
    private readonly IMapper _mapper;

    public CategoryService(ICategoryRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategoryResponse>> GetAllAsync(Guid userId)
    {
        var cats = await _repo.GetByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<CategoryResponse>>(cats);
    }

    public async Task<CategoryResponse> CreateAsync(Guid userId, CategoryRequest request)
    {
        var category = new Category { Name = request.Name, UserId = userId };
        await _repo.AddAsync(category);
        await _repo.SaveChangesAsync();
        return _mapper.Map<CategoryResponse>(category);
    }

    public async Task<CategoryResponse> UpdateAsync(Guid userId, Guid id, CategoryRequest request)
    {
        var category = await GetOwnedOrThrowAsync(userId, id);
        category.Name = request.Name;
        await _repo.UpdateAsync(category);
        await _repo.SaveChangesAsync();
        return _mapper.Map<CategoryResponse>(category);
    }

    public async Task DeleteAsync(Guid userId, Guid id)
    {
        var category = await GetOwnedOrThrowAsync(userId, id);
        await _repo.DeleteAsync(category);
        await _repo.SaveChangesAsync();
    }

    private async Task<Category> GetOwnedOrThrowAsync(Guid userId, Guid id)
    {
        var category = await _repo.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Category), id);

        if (category.UserId != userId)
            throw new ForbiddenException();

        return category;
    }
}
