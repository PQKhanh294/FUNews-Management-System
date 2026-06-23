using FUNewsManagementSystem.Services.Dtos;
using FUNewsManagementSystem.BusinessObjects.Entities;

namespace FUNewsManagementSystem.Services.Services.Interfaces;

public interface ICategoryService
{
    IQueryable<Category> Query();
    Task<List<CategoryTreeDto>> GetTreeAsync(bool activeOnly);
    Task<CategoryDto?> GetByIdAsync(short id);
    Task<CategoryDto> CreateAsync(CategoryUpsertDto dto);
    Task<CategoryDto> UpdateAsync(short id, CategoryUpsertDto dto);
    Task DeleteAsync(short id);
}

