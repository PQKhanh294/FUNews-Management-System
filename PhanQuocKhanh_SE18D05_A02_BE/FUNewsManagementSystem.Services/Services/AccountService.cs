using FUNewsManagementSystem.Services.Dtos;
using FUNewsManagementSystem.Services.Exceptions;
using FUNewsManagementSystem.Services.Mapping;
using FUNewsManagementSystem.Services.Security;
using FUNewsManagementSystem.Services.Services.Interfaces;
using FUNewsManagementSystem.BusinessObjects.Enums;
using FUNewsManagementSystem.BusinessObjects.Security;
using FUNewsManagementSystem.BusinessObjects.Entities;
using FUNewsManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementSystem.Services.Services;

public sealed class AccountService : IAccountService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public AccountService(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public IQueryable<SystemAccount> Query()
    {
        return _unitOfWork.Accounts.Query().AsNoTracking();
    }

    public async Task<AccountDto?> GetByIdAsync(short id)
    {
        var account = await _unitOfWork.Accounts.GetByIdAsync(id);
        return account is null ? null : DtoMapper.ToDto(account);
    }

    public async Task<AccountDto> CreateAsync(AccountUpsertDto dto)
    {
        ValidateAccount(dto, true);
        if (await _unitOfWork.Accounts.FindByEmailAsync(dto.AccountEmail!) is not null)
        {
            throw new AppException("Email already exists.");
        }

        var account = new SystemAccount
        {
            AccountID = await _unitOfWork.Accounts.GetNextIdAsync(),
            AccountName = dto.AccountName,
            AccountEmail = dto.AccountEmail,
            AccountRole = dto.AccountRole,
            AccountPassword = _passwordHasher.Hash(dto.AccountPassword!)
        };

        await _unitOfWork.Accounts.AddAsync(account);
        await _unitOfWork.SaveChangesAsync();
        return DtoMapper.ToDto(account);
    }

    public async Task<AccountDto> UpdateAsync(short id, AccountUpsertDto dto)
    {
        ValidateAccount(dto, false);
        var account = await _unitOfWork.Accounts.GetByIdAsync(id) ?? throw new AppException("Account not found.", 404);
        if (dto.AccountRole == (int)AccountRole.Admin)
        {
            throw new AppException("Admin role cannot be assigned from account form.");
        }

        account.AccountName = dto.AccountName;
        account.AccountEmail = dto.AccountEmail;
        account.AccountRole = dto.AccountRole;
        if (!string.IsNullOrWhiteSpace(dto.AccountPassword))
        {
            account.AccountPassword = _passwordHasher.Hash(dto.AccountPassword);
        }

        _unitOfWork.Accounts.Update(account);
        await _unitOfWork.SaveChangesAsync();
        return DtoMapper.ToDto(account);
    }

    public async Task DeleteAsync(short id)
    {
        if (await _unitOfWork.Accounts.HasCreatedNewsAsync(id))
        {
            throw new AppException("Cannot delete account because it has created news articles.");
        }

        var account = await _unitOfWork.Accounts.GetByIdAsync(id) ?? throw new AppException("Account not found.", 404);
        _unitOfWork.Accounts.Delete(account);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<AccountDto> UpdateProfileAsync(CurrentUser user, ProfileUpdateDto dto)
    {
        var account = await _unitOfWork.Accounts.GetByIdAsync(user.AccountId) ?? throw new AppException("Account not found.", 404);
        account.AccountName = dto.AccountName;
        if (!string.IsNullOrWhiteSpace(dto.NewPassword))
        {
            if (account.IsExternalLogin)
            {
                throw new AppException("Google accounts cannot change password.");
            }

            if (!_passwordHasher.Verify(dto.CurrentPassword ?? string.Empty, account.AccountPassword))
            {
                throw new AppException("Current password is invalid.");
            }

            account.AccountPassword = _passwordHasher.Hash(dto.NewPassword);
        }

        _unitOfWork.Accounts.Update(account);
        await _unitOfWork.SaveChangesAsync();
        return DtoMapper.ToDto(account);
    }

    private static void ValidateAccount(AccountUpsertDto dto, bool requirePassword)
    {
        if (string.IsNullOrWhiteSpace(dto.AccountEmail))
        {
            throw new AppException("Email is required.");
        }

        if (dto.AccountRole == (int)AccountRole.Admin)
        {
            throw new AppException("Admin role cannot be assigned from account form.");
        }

        if (requirePassword && string.IsNullOrWhiteSpace(dto.AccountPassword))
        {
            throw new AppException("Password is required.");
        }
    }
}

