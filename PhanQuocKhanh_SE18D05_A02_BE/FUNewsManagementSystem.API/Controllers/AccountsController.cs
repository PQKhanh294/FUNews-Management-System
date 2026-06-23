using FUNewsManagementSystem.Services.Dtos;
using FUNewsManagementSystem.Services.Services.Interfaces;
using FUNewsManagementSystem.BusinessObjects.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace FUNewsManagementSystem.API.Controllers;

[Authorize(Policy = AuthorizationPolicies.AdminOnly)]
[Route("odata/Accounts")]
public sealed class AccountsController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountsController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [EnableQuery]
    [HttpGet]
    public IActionResult Get() => Ok(_accountService.Query());

    [HttpGet("{key}")]
    public async Task<IActionResult> Get(short key)
    {
        var account = await _accountService.GetByIdAsync(key);
        return account is null ? NotFound() : Ok(account);
    }

    [HttpPost]
    public Task<AccountDto> Post([FromBody] AccountUpsertDto dto) => _accountService.CreateAsync(dto);

    [HttpPut("{key}")]
    public Task<AccountDto> Put(short key, [FromBody] AccountUpsertDto dto) => _accountService.UpdateAsync(key, dto);

    [HttpDelete("{key}")]
    public async Task<IActionResult> Delete(short key)
    {
        await _accountService.DeleteAsync(key);
        return NoContent();
    }
}

