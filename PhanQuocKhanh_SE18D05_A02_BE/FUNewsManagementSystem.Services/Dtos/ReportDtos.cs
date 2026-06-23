namespace FUNewsManagementSystem.Services.Dtos;

public sealed record NewsStatisticDto(string CategoryName, int TotalNews, DateTime? LatestCreatedDate);

