using System.Linq.Expressions;
using FUNewsManagementSystem.DataAccess.Context;
using FUNewsManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementSystem.Repositories;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    protected readonly FUNewsManagementContext Context;
    protected readonly DbSet<TEntity> Set;

    public GenericRepository(FUNewsManagementContext context)
    {
        Context = context;
        Set = context.Set<TEntity>();
    }

    public IQueryable<TEntity> Query() => Set.AsQueryable();

    public Task<TEntity?> GetByIdAsync(params object[] keyValues) => Set.FindAsync(keyValues).AsTask();

    public Task<List<TEntity>> ListAsync(Expression<Func<TEntity, bool>>? predicate = null)
    {
        var query = predicate is null ? Set.AsQueryable() : Set.Where(predicate);
        return query.ToListAsync();
    }

    public Task AddAsync(TEntity entity) => Set.AddAsync(entity).AsTask();

    public void Update(TEntity entity) => Set.Update(entity);

    public void Delete(TEntity entity) => Set.Remove(entity);
}

