namespace FUNewsManagementSystem.Services.Security;

public sealed class BCryptPasswordHasher : IPasswordHasher
{
    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password);

    public bool Verify(string password, string? storedPassword)
    {
        if (string.IsNullOrWhiteSpace(storedPassword))
        {
            return false;
        }

        return IsHashed(storedPassword)
            ? BCrypt.Net.BCrypt.Verify(password, storedPassword)
            : password == storedPassword;
    }

    public bool IsHashed(string? storedPassword)
    {
        return storedPassword?.StartsWith("$2", StringComparison.Ordinal) == true;
    }
}

