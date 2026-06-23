using FUNewsManagementSystem.Services.Dtos;
using FUNewsManagementSystem.BusinessObjects.Security;
using FUNewsManagementSystem.BusinessObjects.Entities;

namespace FUNewsManagementSystem.Services.Services.Interfaces;

public interface INewsArticleService
{
    IQueryable<NewsArticle> Query(bool publicOnly);
    IQueryable<NewsArticleListDto> QueryList(bool publicOnly);
    Task<NewsArticleDto?> GetByIdAsync(string id, bool publicOnly);
    Task<NewsArticleDto> CreateAsync(NewsArticleUpsertDto dto, CurrentUser user);
    Task<NewsArticleDto> UpdateAsync(string id, NewsArticleUpsertDto dto, CurrentUser user);
    Task DeleteAsync(string id, CurrentUser user);
    Task<List<NewsArticleDto>> GetMyHistoryAsync(CurrentUser user);
}

