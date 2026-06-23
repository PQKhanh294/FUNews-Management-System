using FUNewsManagementSystem.Services.Dtos;
using FUNewsManagementSystem.Services.Exceptions;
using FUNewsManagementSystem.Services.Mapping;
using FUNewsManagementSystem.Services.Helpers;
using FUNewsManagementSystem.Services.Services.Interfaces;
using FUNewsManagementSystem.BusinessObjects.Entities;
using FUNewsManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementSystem.Services.Services;

public sealed class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IQueryable<Category> Query() => _unitOfWork.Categories.Query().AsNoTracking();

    public async Task<List<CategoryTreeDto>> GetTreeAsync(bool activeOnly)
    {
        var query = _unitOfWork.Categories.Query().AsNoTracking();
        if (activeOnly)
        {
            query = query.Where(c => c.IsActive == true);
        }

        var categories = await query.OrderBy(c => c.CategoryName).ToListAsync();
        return CategoryHierarchy.BuildTree(categories);
    }

    public async Task<CategoryDto?> GetByIdAsync(short id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        return category is null ? null : DtoMapper.ToDto(category);
    }

    public async Task<CategoryDto> CreateAsync(CategoryUpsertDto dto)
    {
        Validate(dto);
        await EnsureParentExistsAsync(dto.ParentCategoryID);
        var category = new Category
        {
            CategoryName = dto.CategoryName,
            CategoryDesciption = dto.CategoryDesciption,
            ParentCategoryID = dto.ParentCategoryID,
            IsActive = dto.IsActive
        };

        await _unitOfWork.Categories.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();
        return DtoMapper.ToDto(category);
    }

    public async Task<CategoryDto> UpdateAsync(short id, CategoryUpsertDto dto)
    {
        Validate(dto);
        var category = await _unitOfWork.Categories.GetByIdAsync(id) ?? throw new AppException("Category not found.", 404);
        await EnsureParentExistsAsync(dto.ParentCategoryID);
        var categories = await _unitOfWork.Categories.Query().AsNoTracking().ToListAsync();
        if (CategoryHierarchy.WouldCreateCycle(categories, id, dto.ParentCategoryID))
        {
            throw new AppException("The selected parent would create a category cycle.");
        }

        category.CategoryName = dto.CategoryName;
        category.CategoryDesciption = dto.CategoryDesciption;
        category.ParentCategoryID = dto.ParentCategoryID;
        category.IsActive = dto.IsActive;
        _unitOfWork.Categories.Update(category);
        await _unitOfWork.SaveChangesAsync();
        return DtoMapper.ToDto(category);
    }

    public async Task DeleteAsync(short id)
    {
        if (await _unitOfWork.Categories.IsUsedByNewsAsync(id))
        {
            throw new AppException("Cannot delete category because it is used by news articles.");
        }

        var category = await _unitOfWork.Categories.GetByIdAsync(id) ?? throw new AppException("Category not found.", 404);
        _unitOfWork.Categories.Delete(category);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task EnsureParentExistsAsync(short? parentId)
    {
        if (parentId is not null && await _unitOfWork.Categories.GetByIdAsync(parentId.Value) is null)
        {
            throw new AppException("Parent category not found.", 404);
        }
    }

    private static void Validate(CategoryUpsertDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.CategoryName))
        {
            throw new AppException("Category name is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.CategoryDesciption))
        {
            throw new AppException("Category description is required.");
        }
    }
}

