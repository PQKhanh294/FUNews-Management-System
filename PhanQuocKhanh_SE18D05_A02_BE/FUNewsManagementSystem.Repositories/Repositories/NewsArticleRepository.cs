using FUNewsManagementSystem.DataAccess.Context;
using FUNewsManagementSystem.BusinessObjects.Entities;
using FUNewsManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementSystem.Repositories;

public sealed class NewsArticleRepository : GenericRepository<NewsArticle>, INewsArticleRepository
{
    public NewsArticleRepository(FUNewsManagementContext context) : base(context)
    {
    }

    public IQueryable<NewsArticle> QueryWithDetails()
    {
        return Set.Include(n => n.Category)
            .Include(n => n.CreatedBy)
            .Include(n => n.Tags);
    }

    public Task<NewsArticle?> FindWithDetailsAsync(string id)
    {
        return QueryWithDetails().FirstOrDefaultAsync(n => n.NewsArticleID == id);
    }
}

