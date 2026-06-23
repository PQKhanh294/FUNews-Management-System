using FUNewsManagementSystem.Services.Dtos;
using FUNewsManagementSystem.Services.Exceptions;
using FUNewsManagementSystem.Services.Mapping;
using FUNewsManagementSystem.Services.Services.Interfaces;
using FUNewsManagementSystem.BusinessObjects.Entities;
using FUNewsManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementSystem.Services.Services;

public sealed class TagService : ITagService
{
    private readonly IUnitOfWork _unitOfWork;

    public TagService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IQueryable<Tag> Query() => _unitOfWork.Tags.Query().AsNoTracking();

    public async Task<TagDto?> GetByIdAsync(int id)
    {
        var tag = await _unitOfWork.Tags.GetByIdAsync(id);
        return tag is null ? null : DtoMapper.ToDto(tag);
    }

    public async Task<TagDto> CreateAsync(TagUpsertDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.TagName))
        {
            throw new AppException("Tag name is required.");
        }

        var nextId = ((await _unitOfWork.Tags.Query().MaxAsync(t => (int?)t.TagID)) ?? 0) + 1;
        var tag = new Tag { TagID = nextId, TagName = dto.TagName, Note = dto.Note };
        await _unitOfWork.Tags.AddAsync(tag);
        await _unitOfWork.SaveChangesAsync();
        return DtoMapper.ToDto(tag);
    }

    public async Task<TagDto> UpdateAsync(int id, TagUpsertDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.TagName))
        {
            throw new AppException("Tag name is required.");
        }

        var tag = await _unitOfWork.Tags.GetByIdAsync(id) ?? throw new AppException("Tag not found.", 404);
        tag.TagName = dto.TagName;
        tag.Note = dto.Note;
        _unitOfWork.Tags.Update(tag);
        await _unitOfWork.SaveChangesAsync();
        return DtoMapper.ToDto(tag);
    }

    public async Task DeleteAsync(int id)
    {
        var tag = await _unitOfWork.Tags.GetByIdAsync(id) ?? throw new AppException("Tag not found.", 404);
        _unitOfWork.Tags.Delete(tag);
        await _unitOfWork.SaveChangesAsync();
    }
}

