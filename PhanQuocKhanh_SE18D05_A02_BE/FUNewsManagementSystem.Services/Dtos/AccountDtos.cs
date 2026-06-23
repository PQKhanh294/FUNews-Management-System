namespace FUNewsManagementSystem.Services.Dtos;

public sealed record AccountDto(short AccountID, string? AccountName, string? AccountEmail, int? AccountRole, bool IsExternalLogin, string? AvatarUrl);
public sealed record AccountUpsertDto(string? AccountName, string? AccountEmail, int AccountRole, string? AccountPassword);
public sealed record ProfileUpdateDto(string? AccountName, string? CurrentPassword, string? NewPassword);

