namespace FUNewsManagementSystem.Repositories.Interfaces;

public interface IUnitOfWork
{
    IAccountRepository Accounts { get; }
    ICategoryRepository Categories { get; }
    INewsArticleRepository NewsArticles { get; }
    ITagRepository Tags { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

