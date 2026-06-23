USE [FUNewsManagement];
GO

IF COL_LENGTH('dbo.NewsArticle', 'ApprovalStatus') IS NULL
BEGIN
    ALTER TABLE dbo.NewsArticle
    ADD ApprovalStatus tinyint NOT NULL
        CONSTRAINT DF_NewsArticle_ApprovalStatus DEFAULT 0;
END
GO

IF COL_LENGTH('dbo.SystemAccount', 'GoogleId') IS NULL
BEGIN
    ALTER TABLE dbo.SystemAccount ADD GoogleId nvarchar(100) NULL;
END
GO

IF COL_LENGTH('dbo.SystemAccount', 'AvatarUrl') IS NULL
BEGIN
    ALTER TABLE dbo.SystemAccount ADD AvatarUrl nvarchar(300) NULL;
END
GO

IF COL_LENGTH('dbo.SystemAccount', 'IsExternalLogin') IS NULL
BEGIN
    ALTER TABLE dbo.SystemAccount
    ADD IsExternalLogin bit NOT NULL
        CONSTRAINT DF_SystemAccount_IsExternalLogin DEFAULT 0;
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SystemAccount WHERE AccountID = 0)
BEGIN
    INSERT dbo.SystemAccount(AccountID, AccountName, AccountEmail, AccountRole, AccountPassword, IsExternalLogin)
    VALUES (0, N'System Admin', N'admin@FUNewsManagementSystem.org', 0, N'@@abc123@@', 0);
END
GO

UPDATE dbo.NewsArticle
SET ApprovalStatus = 2
WHERE NewsStatus = 1 AND ApprovalStatus = 0;
GO

