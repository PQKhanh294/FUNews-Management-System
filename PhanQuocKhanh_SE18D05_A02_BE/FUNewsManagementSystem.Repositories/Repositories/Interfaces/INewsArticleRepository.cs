using FUNewsManagementSystem.BusinessObjects.Entities;

namespace FUNewsManagementSystem.Repositories.Interfaces;

public interface INewsArticleRepository : IGenericRepository<NewsArticle>
{
    IQueryable<NewsArticle> QueryWithDetails();
    Task<NewsArticle?> FindWithDetailsAsync(string id);
}

