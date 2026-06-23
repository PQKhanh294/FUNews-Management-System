using FUNewsManagementSystem.Services.Security;
using FUNewsManagementSystem.BusinessObjects.Enums;
using FUNewsManagementSystem.BusinessObjects.Options;
using FUNewsManagementSystem.DataAccess.Context;
using FUNewsManagementSystem.BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FUNewsManagementSystem.API.Infrastructure;

public static class SqlServerSchemaInitializer
{
    public static async Task EnsureSqlServerSchemaAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FUNewsManagementContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var adminOptions = scope.ServiceProvider.GetRequiredService<IOptions<AdminAccountOptions>>().Value;

        await db.Database.ExecuteSqlRawAsync("""
IF COL_LENGTH('dbo.NewsArticle', 'ApprovalStatus') IS NULL
BEGIN
    ALTER TABLE dbo.NewsArticle ADD ApprovalStatus tinyint NOT NULL CONSTRAINT DF_NewsArticle_ApprovalStatus DEFAULT 0;
END
""");
        await db.Database.ExecuteSqlRawAsync("""
IF COL_LENGTH('dbo.SystemAccount', 'GoogleId') IS NULL
BEGIN
    ALTER TABLE dbo.SystemAccount ADD GoogleId nvarchar(100) NULL;
END
""");
        await db.Database.ExecuteSqlRawAsync("""
IF COL_LENGTH('dbo.SystemAccount', 'AvatarUrl') IS NULL
BEGIN
    ALTER TABLE dbo.SystemAccount ADD AvatarUrl nvarchar(300) NULL;
END
""");
        await db.Database.ExecuteSqlRawAsync("""
IF COL_LENGTH('dbo.SystemAccount', 'IsExternalLogin') IS NULL
BEGIN
    ALTER TABLE dbo.SystemAccount ADD IsExternalLogin bit NOT NULL CONSTRAINT DF_SystemAccount_IsExternalLogin DEFAULT 0;
END
""");
        await db.Database.ExecuteSqlRawAsync("""
UPDATE dbo.NewsArticle
SET ApprovalStatus = 2
WHERE NewsStatus = 1 AND ApprovalStatus = 0;
""");

        var admin = await db.SystemAccounts.FindAsync(adminOptions.AccountId);
        if (admin is null)
        {
            await db.SystemAccounts.AddAsync(new SystemAccount
            {
                AccountID = adminOptions.AccountId,
                AccountName = adminOptions.Name,
                AccountEmail = adminOptions.Email,
                AccountRole = (int)AccountRole.Admin,
                AccountPassword = passwordHasher.Hash(adminOptions.Password)
            });
        }
        else
        {
            admin.AccountName = adminOptions.Name;
            admin.AccountEmail = adminOptions.Email;
            admin.AccountRole = (int)AccountRole.Admin;
            if (!passwordHasher.IsHashed(admin.AccountPassword))
            {
                admin.AccountPassword = passwordHasher.Hash(adminOptions.Password);
            }
        }

        foreach (var account in await db.SystemAccounts.ToListAsync())
        {
            if (!string.IsNullOrWhiteSpace(account.AccountPassword) && !passwordHasher.IsHashed(account.AccountPassword))
            {
                account.AccountPassword = passwordHasher.Hash(account.AccountPassword);
            }
        }

        await db.SaveChangesAsync();
    }
}

