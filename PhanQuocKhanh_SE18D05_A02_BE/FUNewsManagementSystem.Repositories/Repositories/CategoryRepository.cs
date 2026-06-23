using FUNewsManagementSystem.DataAccess.Context;
using FUNewsManagementSystem.BusinessObjects.Entities;
using FUNewsManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementSystem.Repositories;

public sealed class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(FUNewsManagementContext context) : base(context)
    {
    }

    public Task<bool> IsUsedByNewsAsync(short categoryId)
    {
        return Context.NewsArticles.AnyAsync(n => n.CategoryID == categoryId);
    }
}

