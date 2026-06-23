using System.Security.Claims;
using FUNewsManagementSystem.BusinessObjects.Security;

namespace FUNewsManagementSystem.API.Infrastructure;

public static class ClaimsPrincipalExtensions
{
    public static CurrentUser ToCurrentUser(this ClaimsPrincipal principal)
    {
        var idValue = principal.FindFirstValue("AccountID") ?? principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = principal.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
        var role = principal.FindFirstValue(ClaimTypes.Role) ?? principal.FindFirstValue("Role") ?? string.Empty;
        return new CurrentUser(short.Parse(idValue ?? "0"), email, role);
    }
}

