using FUNewsManagementSystem.Services.Dtos;
using FUNewsManagementSystem.Services.Services.Interfaces;
using FUNewsManagementSystem.BusinessObjects.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FUNewsManagementSystem.API.Controllers;

[ApiController]
[Authorize(Policy = AuthorizationPolicies.AdminOnly)]
[Route("api/report")]
public sealed class ReportController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("news-statistics")]
    public Task<List<NewsStatisticDto>> NewsStatistics(DateTime startDate, DateTime endDate)
    {
        return _reportService.GetNewsStatisticsAsync(startDate, endDate);
    }
}

