using FUNewsManagementSystem.DataAccess.Context;
using FUNewsManagementSystem.BusinessObjects.Entities;
using FUNewsManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementSystem.Repositories;

public sealed class AccountRepository : GenericRepository<SystemAccount>, IAccountRepository
{
    public AccountRepository(FUNewsManagementContext context) : base(context)
    {
    }

    public Task<SystemAccount?> FindByEmailAsync(string email)
    {
        var normalized = email.Trim().ToLower();
        return Set.FirstOrDefaultAsync(a => a.AccountEmail != null && a.AccountEmail.ToLower() == normalized);
    }

    public async Task<short> GetNextIdAsync()
    {
        var maxId = await Set.Select(a => (short?)a.AccountID).MaxAsync() ?? 0;
        return (short)(maxId + 1);
    }

    public Task<bool> HasCreatedNewsAsync(short accountId)
    {
        return Context.NewsArticles.AnyAsync(n => n.CreatedByID == accountId);
    }
}

