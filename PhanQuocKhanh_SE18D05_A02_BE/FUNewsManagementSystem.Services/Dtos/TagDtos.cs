namespace FUNewsManagementSystem.Services.Dtos;

public sealed record TagDto(int TagID, string? TagName, string? Note);
public sealed record TagUpsertDto(string? TagName, string? Note);

