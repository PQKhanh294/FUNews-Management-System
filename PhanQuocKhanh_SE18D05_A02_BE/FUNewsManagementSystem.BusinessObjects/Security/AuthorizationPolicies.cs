namespace FUNewsManagementSystem.BusinessObjects.Security;

public static class AuthorizationPolicies
{
    public const string AdminOnly = "AdminOnly";
    public const string StaffOnly = "StaffOnly";
    public const string AdminOrStaff = "AdminOrStaff";

    public const string AdminRoleName = "Admin";
    public const string StaffRoleName = "Staff";
    public const string LecturerRoleName = "Lecturer";
}

