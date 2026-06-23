using FUNewsManagementSystem.Services.Dtos;
using FUNewsManagementSystem.BusinessObjects.Security;
using FUNewsManagementSystem.BusinessObjects.Entities;

namespace FUNewsManagementSystem.Services.Services.Interfaces;

public interface IAccountService
{
    IQueryable<SystemAccount> Query();
    Task<AccountDto?> GetByIdAsync(short id);
    Task<AccountDto> CreateAsync(AccountUpsertDto dto);
    Task<AccountDto> UpdateAsync(short id, AccountUpsertDto dto);
    Task DeleteAsync(short id);
    Task<AccountDto> UpdateProfileAsync(CurrentUser user, ProfileUpdateDto dto);
}

