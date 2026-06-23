using FUNewsManagementSystem.Services.Dtos;
using FUNewsManagementSystem.BusinessObjects.Security;

namespace FUNewsManagementSystem.Services.Services.Interfaces;

public interface INewsApprovalService
{
    Task<NewsArticleDto> SubmitForApprovalAsync(string id, CurrentUser user);
    Task<NewsArticleDto> RecallAsync(string id, CurrentUser user);
    Task<NewsArticleDto> ApproveAsync(string id, CurrentUser user);
    Task<NewsArticleDto> RejectAsync(string id, CurrentUser user, RejectNewsDto dto);
    Task<List<NewsArticleDto>> GetPendingApprovalAsync();
}

