USE [FUNewsManagement];
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    -- The original seed makes every root category its own parent. A real root
    -- uses NULL so children can form a valid hierarchy without a cycle.
    UPDATE dbo.Category
    SET ParentCategoryID = NULL
    WHERE ParentCategoryID = CategoryID;

    DECLARE @AcademicID smallint =
        (SELECT TOP (1) CategoryID FROM dbo.Category WHERE CategoryName = N'Academic news');
    DECLARE @StudentAffairsID smallint =
        (SELECT TOP (1) CategoryID FROM dbo.Category WHERE CategoryName = N'Student Affairs');
    DECLARE @CampusSafetyID smallint =
        (SELECT TOP (1) CategoryID FROM dbo.Category WHERE CategoryName = N'Campus Safety');
    DECLARE @AlumniID smallint =
        (SELECT TOP (1) CategoryID FROM dbo.Category WHERE CategoryName = N'Alumni News');

    IF @AcademicID IS NULL OR @StudentAffairsID IS NULL OR @CampusSafetyID IS NULL OR @AlumniID IS NULL
        THROW 50001, 'Required root categories were not found.', 1;

    IF NOT EXISTS (
        SELECT 1 FROM dbo.Category
        WHERE CategoryName = N'Research' AND ParentCategoryID = @AcademicID)
    BEGIN
        INSERT dbo.Category (CategoryName, CategoryDesciption, ParentCategoryID, IsActive)
        VALUES (N'Research', N'Research projects, findings, publications and academic innovation.', @AcademicID, 1);
    END;

    DECLARE @ResearchID smallint =
        (SELECT TOP (1) CategoryID FROM dbo.Category
         WHERE CategoryName = N'Research' AND ParentCategoryID = @AcademicID);

    IF NOT EXISTS (
        SELECT 1 FROM dbo.Category
        WHERE CategoryName = N'Artificial Intelligence' AND ParentCategoryID = @ResearchID)
    BEGIN
        INSERT dbo.Category (CategoryName, CategoryDesciption, ParentCategoryID, IsActive)
        VALUES (N'Artificial Intelligence', N'Artificial intelligence and machine learning research news.', @ResearchID, 1);
    END;

    IF NOT EXISTS (
        SELECT 1 FROM dbo.Category
        WHERE CategoryName = N'Software Engineering' AND ParentCategoryID = @ResearchID)
    BEGIN
        INSERT dbo.Category (CategoryName, CategoryDesciption, ParentCategoryID, IsActive)
        VALUES (N'Software Engineering', N'Software engineering research, tools and practices.', @ResearchID, 1);
    END;

    IF NOT EXISTS (
        SELECT 1 FROM dbo.Category
        WHERE CategoryName = N'Admissions' AND ParentCategoryID = @AcademicID)
    BEGIN
        INSERT dbo.Category (CategoryName, CategoryDesciption, ParentCategoryID, IsActive)
        VALUES (N'Admissions', N'Admission policies, enrollment schedules and student guidance.', @AcademicID, 1);
    END;

    IF NOT EXISTS (
        SELECT 1 FROM dbo.Category
        WHERE CategoryName = N'Student Clubs' AND ParentCategoryID = @StudentAffairsID)
    BEGIN
        INSERT dbo.Category (CategoryName, CategoryDesciption, ParentCategoryID, IsActive)
        VALUES (N'Student Clubs', N'News and activities from student clubs and organizations.', @StudentAffairsID, 1);
    END;

    IF NOT EXISTS (
        SELECT 1 FROM dbo.Category
        WHERE CategoryName = N'Student Events' AND ParentCategoryID = @StudentAffairsID)
    BEGIN
        INSERT dbo.Category (CategoryName, CategoryDesciption, ParentCategoryID, IsActive)
        VALUES (N'Student Events', N'Campus events, competitions and student activities.', @StudentAffairsID, 1);
    END;

    IF NOT EXISTS (
        SELECT 1 FROM dbo.Category
        WHERE CategoryName = N'Security Notices' AND ParentCategoryID = @CampusSafetyID)
    BEGIN
        INSERT dbo.Category (CategoryName, CategoryDesciption, ParentCategoryID, IsActive)
        VALUES (N'Security Notices', N'Important campus security notices and safety guidance.', @CampusSafetyID, 1);
    END;

    IF NOT EXISTS (
        SELECT 1 FROM dbo.Category
        WHERE CategoryName = N'Alumni Achievements' AND ParentCategoryID = @AlumniID)
    BEGIN
        INSERT dbo.Category (CategoryName, CategoryDesciption, ParentCategoryID, IsActive)
        VALUES (N'Alumni Achievements', N'Career milestones and achievements of FU alumni.', @AlumniID, 1);
    END;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO

-- Verify the hierarchy after seeding.
WITH CategoryTree AS
(
    SELECT
        CategoryID,
        CategoryName,
        ParentCategoryID,
        0 AS Depth,
        CAST(CategoryName AS nvarchar(1000)) AS CategoryPath
    FROM dbo.Category
    WHERE ParentCategoryID IS NULL

    UNION ALL

    SELECT
        child.CategoryID,
        child.CategoryName,
        child.ParentCategoryID,
        parent.Depth + 1,
        CAST(parent.CategoryPath + N' > ' + child.CategoryName AS nvarchar(1000))
    FROM dbo.Category AS child
    INNER JOIN CategoryTree AS parent
        ON child.ParentCategoryID = parent.CategoryID
)
SELECT
    CategoryID,
    REPLICATE(N'    ', Depth) + CategoryName AS MenuItem,
    ParentCategoryID,
    CategoryPath
FROM CategoryTree
ORDER BY CategoryPath;
GO
