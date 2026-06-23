namespace FUNewsManagementSystem.Services.Dtos;

public sealed record CategoryDto(short CategoryID, string CategoryName, string CategoryDesciption, short? ParentCategoryID, bool? IsActive);
public sealed record CategoryUpsertDto(string CategoryName, string CategoryDesciption, short? ParentCategoryID, bool IsActive);
public sealed record CategoryTreeDto(short CategoryID, string CategoryName, string CategoryDesciption, bool? IsActive, List<CategoryTreeDto> Children);

