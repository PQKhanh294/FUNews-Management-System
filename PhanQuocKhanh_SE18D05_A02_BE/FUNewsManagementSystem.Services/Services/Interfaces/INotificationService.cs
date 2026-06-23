using FUNewsManagementSystem.BusinessObjects.Entities;

namespace FUNewsManagementSystem.Services.Services.Interfaces;

public interface INotificationService
{
    Task NotifyNewsApprovedAsync(NewsArticle article);
}

