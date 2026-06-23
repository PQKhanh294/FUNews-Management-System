using FUNewsManagementSystem.Services.Dtos;
using FUNewsManagementSystem.Services.Exceptions;
using FUNewsManagementSystem.Services.Helpers;
using FUNewsManagementSystem.Services.Mapping;
using FUNewsManagementSystem.Services.Services.Interfaces;
using FUNewsManagementSystem.BusinessObjects.Enums;
using FUNewsManagementSystem.BusinessObjects.Security;
using FUNewsManagementSystem.BusinessObjects.Entities;
using FUNewsManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementSystem.Services.Services;

public sealed class NewsApprovalService : INewsApprovalService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISeoMetadataBuilder _seoMetadataBuilder;
    private readonly INotificationService _notificationService;

    public NewsApprovalService(IUnitOfWork unitOfWork, ISeoMetadataBuilder seoMetadataBuilder, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _seoMetadataBuilder = seoMetadataBuilder;
        _notificationService = notificationService;
    }

    public async Task<NewsArticleDto> SubmitForApprovalAsync(string id, CurrentUser user)
    {
        var article = await GetEditableOwnedArticleAsync(id, user);
        if (article.ApprovalStatus == (byte)ApprovalStatus.Approved)
        {
            throw new AppException("Approved news cannot be submitted again.", 403);
        }

        article.ApprovalStatus = (byte)ApprovalStatus.PendingApproval;
        article.ModifiedDate = DateTime.Now;
        await _unitOfWork.SaveChangesAsync();
        return ToDto(article);
    }

    public async Task<NewsArticleDto> RecallAsync(string id, CurrentUser user)
    {
        var article = await _unitOfWork.NewsArticles.FindWithDetailsAsync(id) ?? throw new AppException("News article not found.", 404);
        if (article.CreatedByID != user.AccountId || article.ApprovalStatus != (byte)ApprovalStatus.PendingApproval)
        {
            throw new AppException("Only pending articles created by you can be recalled.", 403);
        }

        article.ApprovalStatus = (byte)ApprovalStatus.Draft;
        article.ModifiedDate = DateTime.Now;
        await _unitOfWork.SaveChangesAsync();
        return ToDto(article);
    }

    public async Task<NewsArticleDto> ApproveAsync(string id, CurrentUser user)
    {
        if (!user.IsAdmin)
        {
            throw new AppException("Only admin can approve news.", 403);
        }

        var article = await _unitOfWork.NewsArticles.FindWithDetailsAsync(id) ?? throw new AppException("News article not found.", 404);
        article.ApprovalStatus = (byte)ApprovalStatus.Approved;
        article.NewsStatus = true;
        article.UpdatedByID = user.AccountId;
        article.ModifiedDate = DateTime.Now;
        await _unitOfWork.SaveChangesAsync();
        await _notificationService.NotifyNewsApprovedAsync(article);
        return ToDto(article);
    }

    public async Task<NewsArticleDto> RejectAsync(string id, CurrentUser user, RejectNewsDto dto)
    {
        if (!user.IsAdmin)
        {
            throw new AppException("Only admin can reject news.", 403);
        }

        var article = await _unitOfWork.NewsArticles.FindWithDetailsAsync(id) ?? throw new AppException("News article not found.", 404);
        article.ApprovalStatus = (byte)ApprovalStatus.Rejected;
        article.NewsStatus = false;
        article.UpdatedByID = user.AccountId;
        article.ModifiedDate = DateTime.Now;
        await _unitOfWork.SaveChangesAsync();
        return ToDto(article);
    }

    public async Task<List<NewsArticleDto>> GetPendingApprovalAsync()
    {
        var articles = await _unitOfWork.NewsArticles.QueryWithDetails()
            .Where(n => n.ApprovalStatus == (byte)ApprovalStatus.PendingApproval)
            .OrderByDescending(n => n.CreatedDate)
            .ToListAsync();
        return articles.Select(ToDto).ToList();
    }

    private async Task<NewsArticle> GetEditableOwnedArticleAsync(string id, CurrentUser user)
    {
        var article = await _unitOfWork.NewsArticles.FindWithDetailsAsync(id) ?? throw new AppException("News article not found.", 404);
        if (article.CreatedByID != user.AccountId)
        {
            throw new AppException("You can only submit your own news articles.", 403);
        }

        return article;
    }

    private NewsArticleDto ToDto(NewsArticle article)
    {
        return DtoMapper.ToDto(article, _seoMetadataBuilder.Build(article));
    }
}

