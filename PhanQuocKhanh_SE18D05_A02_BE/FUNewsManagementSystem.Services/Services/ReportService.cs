using FUNewsManagementSystem.Services.Dtos;
using FUNewsManagementSystem.Services.Exceptions;
using FUNewsManagementSystem.Services.Services.Interfaces;
using FUNewsManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementSystem.Services.Services;

public sealed class ReportService : IReportService
{
    private readonly IUnitOfWork _unitOfWork;

    public ReportService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<NewsStatisticDto>> GetNewsStatisticsAsync(DateTime startDate, DateTime endDate)
    {
        if (startDate > endDate)
        {
            throw new AppException("StartDate must be less than or equal to EndDate.");
        }

        var inclusiveEnd = endDate.Date.AddDays(1);
        var rows = await _unitOfWork.NewsArticles.QueryWithDetails()
            .Where(n => n.CreatedDate >= startDate.Date && n.CreatedDate < inclusiveEnd)
            .ToListAsync();

        return rows
            .GroupBy(n => n.Category?.CategoryName ?? "Uncategorized")
            .Select(g => new NewsStatisticDto(g.Key, g.Count(), g.Max(n => n.CreatedDate)))
            .OrderByDescending(r => r.LatestCreatedDate)
            .ToList();
    }
}

