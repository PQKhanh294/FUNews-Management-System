using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FUNewsManagementSystem.Services.Dtos;
using FUNewsManagementSystem.BusinessObjects.Enums;
using FUNewsManagementSystem.BusinessObjects.Options;
using FUNewsManagementSystem.BusinessObjects.Security;
using FUNewsManagementSystem.BusinessObjects.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FUNewsManagementSystem.Services.Security;

public sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtOptions _options;

    public JwtTokenGenerator(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public AuthResponseDto Generate(SystemAccount account)
    {
        var role = account.AccountRole switch
        {
            (int)AccountRole.Admin => AuthorizationPolicies.AdminRoleName,
            (int)AccountRole.Staff => AuthorizationPolicies.StaffRoleName,
            (int)AccountRole.Lecturer => AuthorizationPolicies.LecturerRoleName,
            _ => AuthorizationPolicies.StaffRoleName
        };
        var expiresAt = DateTime.UtcNow.AddMinutes(_options.ExpireMinutes);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, account.AccountID.ToString()),
            new Claim("AccountID", account.AccountID.ToString()),
            new Claim(ClaimTypes.NameIdentifier, account.AccountID.ToString()),
            new Claim(ClaimTypes.Email, account.AccountEmail ?? string.Empty),
            new Claim(ClaimTypes.Role, role),
            new Claim("Role", role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new AuthResponseDto(
            new JwtSecurityTokenHandler().WriteToken(token),
            expiresAt,
            account.AccountID,
            account.AccountEmail ?? string.Empty,
            role,
            account.AvatarUrl);
    }
}

