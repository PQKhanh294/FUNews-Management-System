using FUNewsManagementSystem.API.Infrastructure;
using FUNewsManagementSystem.Services.Dtos;
using FUNewsManagementSystem.Services.Services.Interfaces;
using FUNewsManagementSystem.BusinessObjects.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FUNewsManagementSystem.API.Controllers;

[ApiController]
[Authorize(Policy = AuthorizationPolicies.AdminOrStaff)]
[Route("api/profile")]
public sealed class ProfileController : ControllerBase
{
    private readonly IAccountService _accountService;

    public ProfileController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var current = User.ToCurrentUser();
        var profile = await _accountService.GetByIdAsync(current.AccountId);
        return profile is null ? NotFound() : Ok(profile);
    }

    [HttpPut]
    public Task<AccountDto> Put([FromBody] ProfileUpdateDto dto)
    {
        return _accountService.UpdateProfileAsync(User.ToCurrentUser(), dto);
    }
}

