namespace FUNewsManagementSystem.BusinessObjects.Options;

public sealed class JwtOptions
{
    public string Issuer { get; set; } = "FUNewsManagementSystem";
    public string Audience { get; set; } = "FUNewsManagementSystem.Client";
    public string SecretKey { get; set; } = "FUNewsManagementSystem_A02_Development_Secret_Key_Change_Me";
    public int ExpireMinutes { get; set; } = 120;
}

