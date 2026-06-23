using FUNewsManagementSystem.Services.Services.Interfaces;
using FUNewsManagementSystem.BusinessObjects.Entities;
using Microsoft.Extensions.Logging;

namespace FUNewsManagementSystem.Services.Services;

public sealed class LogNotificationService : INotificationService
{
    private readonly ILogger<LogNotificationService> _logger;

    public LogNotificationService(ILogger<LogNotificationService> logger)
    {
        _logger = logger;
    }

    public Task NotifyNewsApprovedAsync(NewsArticle article)
    {
        _logger.LogInformation("News article {NewsArticleID} was approved.", article.NewsArticleID);
        return Task.CompletedTask;
    }
}

