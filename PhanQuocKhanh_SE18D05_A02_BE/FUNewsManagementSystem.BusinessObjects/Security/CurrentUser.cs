namespace FUNewsManagementSystem.BusinessObjects.Security;

public sealed record CurrentUser(short AccountId, string Email, string Role)
{
    public bool IsAdmin => Role == AuthorizationPolicies.AdminRoleName;
    public bool IsStaff => Role == AuthorizationPolicies.StaffRoleName;
}

