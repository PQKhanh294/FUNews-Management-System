using FUNewsManagementSystem.Services.Dtos;
using FUNewsManagementSystem.Services.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FUNewsManagementSystem.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public Task<AuthResponseDto> Login(LoginRequestDto request) => _authService.LoginAsync(request);

    [HttpPost("google")]
    public Task<AuthResponseDto> GoogleLogin(GoogleLoginRequestDto request) => _authService.GoogleLoginAsync(request);
}

