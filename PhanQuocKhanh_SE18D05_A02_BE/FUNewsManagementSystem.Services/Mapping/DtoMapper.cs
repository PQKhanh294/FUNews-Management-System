using FUNewsManagementSystem.Services.Dtos;
using FUNewsManagementSystem.BusinessObjects.Entities;

namespace FUNewsManagementSystem.Services.Mapping;

public static class DtoMapper
{
    public static AccountDto ToDto(SystemAccount account)
    {
        return new AccountDto(account.AccountID, account.AccountName, account.AccountEmail, account.AccountRole, account.IsExternalLogin, account.AvatarUrl);
    }

    public static CategoryDto ToDto(Category category)
    {
        return new CategoryDto(category.CategoryID, category.CategoryName, category.CategoryDesciption, category.ParentCategoryID, category.IsActive);
    }

    public static TagDto ToDto(Tag tag)
    {
        return new TagDto(tag.TagID, tag.TagName, tag.Note);
    }

    public static NewsArticleDto ToDto(NewsArticle article, SeoMetaDto seo)
    {
        return new NewsArticleDto(
            article.NewsArticleID,
            article.NewsTitle,
            article.Headline,
            article.CreatedDate,
            article.NewsContent,
            article.NewsSource,
            article.CategoryID,
            article.Category?.CategoryName,
            article.NewsStatus,
            article.ApprovalStatus,
            article.CreatedByID,
            article.CreatedBy?.AccountName,
            article.ModifiedDate,
            article.Tags.Select(ToDto).ToList(),
            seo);
    }
}

