using FUNewsManagementSystem.Services.Dtos;

namespace FUNewsManagementSystem.Services.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    Task<AuthResponseDto> GoogleLoginAsync(GoogleLoginRequestDto request);
}

