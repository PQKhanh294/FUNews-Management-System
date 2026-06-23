using System.Linq.Expressions;

namespace FUNewsManagementSystem.Repositories.Interfaces;

public interface IGenericRepository<TEntity> where TEntity : class
{
    IQueryable<TEntity> Query();
    Task<TEntity?> GetByIdAsync(params object[] keyValues);
    Task<List<TEntity>> ListAsync(Expression<Func<TEntity, bool>>? predicate = null);
    Task AddAsync(TEntity entity);
    void Update(TEntity entity);
    void Delete(TEntity entity);
}

