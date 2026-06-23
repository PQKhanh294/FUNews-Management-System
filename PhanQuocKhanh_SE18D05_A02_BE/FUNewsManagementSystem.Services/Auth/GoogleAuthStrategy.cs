using FUNewsManagementSystem.Services.Dtos;
using FUNewsManagementSystem.Services.Exceptions;
using FUNewsManagementSystem.Services.Security;
using FUNewsManagementSystem.BusinessObjects.Enums;
using FUNewsManagementSystem.BusinessObjects.Options;
using FUNewsManagementSystem.BusinessObjects.Entities;
using FUNewsManagementSystem.Repositories.Interfaces;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;

namespace FUNewsManagementSystem.Services.Auth;

public sealed class GoogleAuthStrategy : IAuthenticationStrategy
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly GoogleAuthOptions _options;

    public GoogleAuthStrategy(IUnitOfWork unitOfWork, IJwtTokenGenerator jwtTokenGenerator, IOptions<GoogleAuthOptions> options)
    {
        _unitOfWork = unitOfWork;
        _jwtTokenGenerator = jwtTokenGenerator;
        _options = options.Value;
    }

    public bool CanHandle(string provider) => provider.Equals("google", StringComparison.OrdinalIgnoreCase);

    public async Task<AuthResponseDto> AuthenticateAsync(LoginContext context)
    {
        if (string.IsNullOrWhiteSpace(_options.ClientId))
        {
            throw new AppException("Google ClientId is not configured.", 500);
        }

        if (string.IsNullOrWhiteSpace(context.IdToken))
        {
            throw new AppException("Google id_token is required.");
        }

        var payload = await GoogleJsonWebSignature.ValidateAsync(
            context.IdToken,
            new GoogleJsonWebSignature.ValidationSettings { Audience = new[] { _options.ClientId } });

        var account = await _unitOfWork.Accounts.FindByEmailAsync(payload.Email);
        if (account is null)
        {
            account = new SystemAccount
            {
                AccountID = await _unitOfWork.Accounts.GetNextIdAsync(),
                AccountEmail = payload.Email,
                AccountName = payload.Name,
                AccountRole = (int)AccountRole.Staff,
                AccountPassword = null,
                GoogleId = payload.Subject,
                AvatarUrl = payload.Picture,
                IsExternalLogin = true
            };
            await _unitOfWork.Accounts.AddAsync(account);
        }
        else
        {
            account.GoogleId = payload.Subject;
            account.AvatarUrl = payload.Picture;
            account.IsExternalLogin = true;
            _unitOfWork.Accounts.Update(account);
        }

        await _unitOfWork.SaveChangesAsync();
        return _jwtTokenGenerator.Generate(account);
    }
}

