using FUNewsManagementSystem.Services.Dtos;

namespace FUNewsManagementSystem.Services.Auth;

public sealed record LoginContext(string? Email, string? Password, string? IdToken);

public interface IAuthenticationStrategy
{
    bool CanHandle(string provider);
    Task<AuthResponseDto> AuthenticateAsync(LoginContext context);
}

