namespace FUNewsManagementSystem.BusinessObjects.Options;

public sealed class AdminAccountOptions
{
    public short AccountId { get; set; } = 0;
    public string Name { get; set; } = "System Admin";
    public string Email { get; set; } = "admin@FUNewsManagementSystem.org";
    public string Password { get; set; } = "@@abc123@@";
}

