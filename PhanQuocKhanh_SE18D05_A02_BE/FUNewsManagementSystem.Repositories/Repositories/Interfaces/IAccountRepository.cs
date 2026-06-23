using FUNewsManagementSystem.BusinessObjects.Entities;

namespace FUNewsManagementSystem.Repositories.Interfaces;

public interface IAccountRepository : IGenericRepository<SystemAccount>
{
    Task<SystemAccount?> FindByEmailAsync(string email);
    Task<short> GetNextIdAsync();
    Task<bool> HasCreatedNewsAsync(short accountId);
}

