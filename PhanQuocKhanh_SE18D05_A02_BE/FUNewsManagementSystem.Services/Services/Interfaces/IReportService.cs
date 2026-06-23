using FUNewsManagementSystem.Services.Dtos;

namespace FUNewsManagementSystem.Services.Services.Interfaces;

public interface IReportService
{
    Task<List<NewsStatisticDto>> GetNewsStatisticsAsync(DateTime startDate, DateTime endDate);
}

