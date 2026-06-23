using FUNewsManagementSystem.Services.Dtos;
using FUNewsManagementSystem.Services.Exceptions;
using FUNewsManagementSystem.Services.Helpers;
using FUNewsManagementSystem.Services.Mapping;
using FUNewsManagementSystem.Services.Services.Interfaces;
using FUNewsManagementSystem.Services.Validators;
using FUNewsManagementSystem.BusinessObjects.Enums;
using FUNewsManagementSystem.BusinessObjects.Security;
using FUNewsManagementSystem.BusinessObjects.Entities;
using FUNewsManagementSystem.Repositories.Interfaces;
using Ganss.Xss;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementSystem.Services.Services;

public sealed class NewsArticleService : INewsArticleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INewsArticleValidator _validator;
    private readonly ISeoMetadataBuilder _seoMetadataBuilder;
    private readonly HtmlSanitizer _htmlSanitizer = new();

    public NewsArticleService(IUnitOfWork unitOfWork, INewsArticleValidator validator, ISeoMetadataBuilder seoMetadataBuilder)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
        _seoMetadataBuilder = seoMetadataBuilder;
    }

    public IQueryable<NewsArticle> Query(bool publicOnly)
    {
        var query = _unitOfWork.NewsArticles.QueryWithDetails().AsNoTracking();
        return publicOnly
            ? query.Where(n => n.NewsStatus == true && n.ApprovalStatus == (byte)ApprovalStatus.Approved)
            : query;
    }

    public IQueryable<NewsArticleListDto> QueryList(bool publicOnly)
    {
        var query = _unitOfWork.NewsArticles.Query().AsNoTracking();
        if (publicOnly)
        {
            query = query.Where(n => n.NewsStatus == true && n.ApprovalStatus == (byte)ApprovalStatus.Approved);
        }

        return query.Select(n => new NewsArticleListDto
        {
            NewsArticleID = n.NewsArticleID,
            NewsTitle = n.NewsTitle,
            Headline = n.Headline,
            CreatedDate = n.CreatedDate,
            CategoryID = n.CategoryID,
            CategoryName = n.Category != null ? n.Category.CategoryName : null,
            NewsStatus = n.NewsStatus,
            ApprovalStatus = n.ApprovalStatus,
            CreatedByID = n.CreatedByID,
            CreatedByName = n.CreatedBy != null ? n.CreatedBy.AccountName : null,
            ModifiedDate = n.ModifiedDate
        });
    }

    public async Task<NewsArticleDto?> GetByIdAsync(string id, bool publicOnly)
    {
        var article = await Query(publicOnly).FirstOrDefaultAsync(n => n.NewsArticleID == id);
        return article is null ? null : DtoMapper.ToDto(article, _seoMetadataBuilder.Build(article));
    }

    public async Task<NewsArticleDto> CreateAsync(NewsArticleUpsertDto dto, CurrentUser user)
    {
        _validator.Validate(dto);
        await EnsureTitleIsUniqueAsync(dto.NewsTitle);
        var tags = await LoadTagsAsync(dto.TagIds);
        var article = new NewsArticle
        {
            NewsArticleID = GenerateNewsId(),
            NewsTitle = dto.NewsTitle!.Trim(),
            Headline = dto.Headline,
            NewsContent = _htmlSanitizer.Sanitize(dto.NewsContent ?? string.Empty),
            NewsSource = dto.NewsSource,
            CategoryID = dto.CategoryID,
            NewsStatus = false,
            ApprovalStatus = ResolveApprovalStatus(dto.NewsStatus),
            CreatedByID = user.AccountId,
            CreatedDate = DateTime.Now,
            ModifiedDate = DateTime.Now
        };

        foreach (var tag in tags)
        {
            article.Tags.Add(tag);
        }

        await _unitOfWork.NewsArticles.AddAsync(article);
        await _unitOfWork.SaveChangesAsync();
        return (await GetByIdAsync(article.NewsArticleID, false))!;
    }

    public async Task<NewsArticleDto> UpdateAsync(string id, NewsArticleUpsertDto dto, CurrentUser user)
    {
        _validator.Validate(dto);
        var article = await _unitOfWork.NewsArticles.FindWithDetailsAsync(id) ?? throw new AppException("News article not found.", 404);
        EnsureCanEdit(article, user);
        await EnsureTitleIsUniqueAsync(dto.NewsTitle, id);

        article.NewsTitle = dto.NewsTitle!.Trim();
        article.Headline = dto.Headline;
        article.NewsContent = _htmlSanitizer.Sanitize(dto.NewsContent ?? string.Empty);
        article.NewsSource = dto.NewsSource;
        article.CategoryID = dto.CategoryID;
        article.NewsStatus = false;
        article.ApprovalStatus = ResolveApprovalStatus(dto.NewsStatus);
        article.UpdatedByID = user.AccountId;
        article.ModifiedDate = DateTime.Now;

        article.Tags.Clear();
        foreach (var tag in await LoadTagsAsync(dto.TagIds))
        {
            article.Tags.Add(tag);
        }

        _unitOfWork.NewsArticles.Update(article);
        await _unitOfWork.SaveChangesAsync();
        return (await GetByIdAsync(id, false))!;
    }

    public async Task DeleteAsync(string id, CurrentUser user)
    {
        var article = await _unitOfWork.NewsArticles.FindWithDetailsAsync(id) ?? throw new AppException("News article not found.", 404);
        EnsureOwnsArticle(article, user);
        article.Tags.Clear();
        await _unitOfWork.SaveChangesAsync();
        _unitOfWork.NewsArticles.Delete(article);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<List<NewsArticleDto>> GetMyHistoryAsync(CurrentUser user)
    {
        var articles = await _unitOfWork.NewsArticles.QueryWithDetails()
            .Where(n => n.CreatedByID == user.AccountId)
            .OrderByDescending(n => n.CreatedDate)
            .ToListAsync();
        return articles.Select(a => DtoMapper.ToDto(a, _seoMetadataBuilder.Build(a))).ToList();
    }

    private async Task EnsureTitleIsUniqueAsync(string? title, string? currentArticleId = null)
    {
        var normalizedTitle = title!.Trim().ToLower();
        var query = _unitOfWork.NewsArticles.Query()
            .Where(n => n.NewsTitle != null && n.NewsTitle.Trim().ToLower() == normalizedTitle);

        if (!string.IsNullOrWhiteSpace(currentArticleId))
        {
            query = query.Where(n => n.NewsArticleID != currentArticleId);
        }

        var exists = await query.AnyAsync();

        if (exists)
        {
            throw new AppException("News title already exists.");
        }
    }

    private async Task<List<Tag>> LoadTagsAsync(IEnumerable<int> tagIds)
    {
        var ids = tagIds.Distinct().ToArray();
        var tags = await _unitOfWork.Tags.FindByIdsAsync(ids);
        if (tags.Count != ids.Length)
        {
            throw new AppException("One or more tags do not exist.");
        }

        return tags;
    }

    private static void EnsureCanEdit(NewsArticle article, CurrentUser user)
    {
        EnsureOwnsArticle(article, user);

        if (article.ApprovalStatus == (byte)ApprovalStatus.Approved)
        {
            throw new AppException("You cannot edit an article after it has been approved by the Admin.", 403);
        }
    }

    private static void EnsureOwnsArticle(NewsArticle article, CurrentUser user)
    {
        if (article.CreatedByID != user.AccountId)
        {
            throw new AppException("You can only manage your own news articles.", 403);
        }
    }

    private static string GenerateNewsId()
    {
        return $"NEWS{DateTime.Now:yyyyMMddHHmmss}";
    }

    private static byte ResolveApprovalStatus(bool requestedActive)
    {
        return requestedActive
            ? (byte)ApprovalStatus.PendingApproval
            : (byte)ApprovalStatus.Draft;
    }
}

