using FUNewsManagementSystem.BusinessObjects.Entities;

namespace FUNewsManagementSystem.Repositories.Interfaces;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<bool> IsUsedByNewsAsync(short categoryId);
}

