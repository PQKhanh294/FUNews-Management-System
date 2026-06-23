using FUNewsManagementSystem.DataAccess.Context;
using FUNewsManagementSystem.BusinessObjects.Entities;
using FUNewsManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementSystem.Repositories;

public sealed class TagRepository : GenericRepository<Tag>, ITagRepository
{
    public TagRepository(FUNewsManagementContext context) : base(context)
    {
    }

    public Task<List<Tag>> FindByIdsAsync(IEnumerable<int> ids)
    {
        var tagIds = ids.Distinct().ToArray();
        return Set.Where(t => tagIds.Contains(t.TagID)).ToListAsync();
    }
}

