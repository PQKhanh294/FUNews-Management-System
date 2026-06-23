using FUNewsManagementSystem.BusinessObjects.Entities;

namespace FUNewsManagementSystem.Repositories.Interfaces;

public interface ITagRepository : IGenericRepository<Tag>
{
    Task<List<Tag>> FindByIdsAsync(IEnumerable<int> ids);
}

