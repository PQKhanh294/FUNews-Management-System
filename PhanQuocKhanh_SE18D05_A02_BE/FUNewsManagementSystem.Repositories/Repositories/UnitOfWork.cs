using FUNewsManagementSystem.DataAccess.Context;
using FUNewsManagementSystem.Repositories.Interfaces;

namespace FUNewsManagementSystem.Repositories;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly FUNewsManagementContext _context;

    public UnitOfWork(
        FUNewsManagementContext context,
        IAccountRepository accounts,
        ICategoryRepository categories,
        INewsArticleRepository newsArticles,
        ITagRepository tags)
    {
        _context = context;
        Accounts = accounts;
        Categories = categories;
        NewsArticles = newsArticles;
        Tags = tags;
    }

    public IAccountRepository Accounts { get; }
    public ICategoryRepository Categories { get; }
    public INewsArticleRepository NewsArticles { get; }
    public ITagRepository Tags { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}

