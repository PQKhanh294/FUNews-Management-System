using FUNewsManagementSystem.Services.Dtos;
using FUNewsManagementSystem.Services.Exceptions;
using FUNewsManagementSystem.Services.Security;
using FUNewsManagementSystem.BusinessObjects.Enums;
using FUNewsManagementSystem.BusinessObjects.Options;
using FUNewsManagementSystem.BusinessObjects.Entities;
using FUNewsManagementSystem.Repositories.Interfaces;
using Microsoft.Extensions.Options;

namespace FUNewsManagementSystem.Services.Auth;

public sealed class JwtPasswordAuthStrategy : IAuthenticationStrategy
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly AdminAccountOptions _adminOptions;

    public JwtPasswordAuthStrategy(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IOptions<AdminAccountOptions> adminOptions)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _adminOptions = adminOptions.Value;
    }

    public bool CanHandle(string provider) => provider.Equals("password", StringComparison.OrdinalIgnoreCase);

    public async Task<AuthResponseDto> AuthenticateAsync(LoginContext context)
    {
        if (string.IsNullOrWhiteSpace(context.Email) || string.IsNullOrWhiteSpace(context.Password))
        {
            throw new AppException("Email and password are required.", 400);
        }

        if (context.Email.Equals(_adminOptions.Email, StringComparison.OrdinalIgnoreCase)
            && context.Password == _adminOptions.Password)
        {
            return _jwtTokenGenerator.Generate(new SystemAccount
            {
                AccountID = _adminOptions.AccountId,
                AccountEmail = _adminOptions.Email,
                AccountName = _adminOptions.Name,
                AccountRole = (int)AccountRole.Admin
            });
        }

        var account = await _unitOfWork.Accounts.FindByEmailAsync(context.Email);
        if (account is null || !_passwordHasher.Verify(context.Password, account.AccountPassword))
        {
            throw new AppException("Invalid email or password.", 401);
        }

        if (!_passwordHasher.IsHashed(account.AccountPassword))
        {
            account.AccountPassword = _passwordHasher.Hash(context.Password);
            _unitOfWork.Accounts.Update(account);
            await _unitOfWork.SaveChangesAsync();
        }

        return _jwtTokenGenerator.Generate(account);
    }
}

