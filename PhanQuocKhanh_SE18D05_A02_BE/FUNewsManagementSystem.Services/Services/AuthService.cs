using FUNewsManagementSystem.Services.Auth;
using FUNewsManagementSystem.Services.Dtos;
using FUNewsManagementSystem.Services.Exceptions;
using FUNewsManagementSystem.Services.Services.Interfaces;

namespace FUNewsManagementSystem.Services.Services;

public sealed class AuthService : IAuthService
{
    private readonly IEnumerable<IAuthenticationStrategy> _strategies;

    public AuthService(IEnumerable<IAuthenticationStrategy> strategies)
    {
        _strategies = strategies;
    }

    public Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        return GetStrategy("password").AuthenticateAsync(new LoginContext(request.Email, request.Password, null));
    }

    public Task<AuthResponseDto> GoogleLoginAsync(GoogleLoginRequestDto request)
    {
        return GetStrategy("google").AuthenticateAsync(new LoginContext(null, null, request.IdToken));
    }

    private IAuthenticationStrategy GetStrategy(string provider)
    {
        return _strategies.FirstOrDefault(s => s.CanHandle(provider))
            ?? throw new AppException($"Authentication provider '{provider}' is not registered.", 500);
    }
}

