using FUNewsManagementSystem.Services.Dtos;
using FUNewsManagementSystem.BusinessObjects.Entities;

namespace FUNewsManagementSystem.Services.Services.Interfaces;

public interface ITagService
{
    IQueryable<Tag> Query();
    Task<TagDto?> GetByIdAsync(int id);
    Task<TagDto> CreateAsync(TagUpsertDto dto);
    Task<TagDto> UpdateAsync(int id, TagUpsertDto dto);
    Task DeleteAsync(int id);
}

