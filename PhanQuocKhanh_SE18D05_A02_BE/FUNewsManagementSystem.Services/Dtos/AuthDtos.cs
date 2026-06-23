namespace FUNewsManagementSystem.Services.Dtos;

public sealed record LoginRequestDto(string Email, string Password);
public sealed record GoogleLoginRequestDto(string IdToken);
public sealed record AuthResponseDto(string AccessToken, DateTime ExpiresAt, short AccountId, string Email, string Role, string? AvatarUrl);

