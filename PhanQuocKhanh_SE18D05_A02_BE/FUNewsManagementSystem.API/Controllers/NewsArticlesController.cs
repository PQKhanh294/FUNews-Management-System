using FUNewsManagementSystem.API.Infrastructure;
using FUNewsManagementSystem.Services.Dtos;
using FUNewsManagementSystem.Services.Services.Interfaces;
using FUNewsManagementSystem.BusinessObjects.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace FUNewsManagementSystem.API.Controllers;

[Route("odata/NewsArticles")]
public sealed class NewsArticlesController : ControllerBase
{
    private readonly INewsArticleService _newsArticleService;
    private readonly INewsApprovalService _newsApprovalService;

    public NewsArticlesController(INewsArticleService newsArticleService, INewsApprovalService newsApprovalService)
    {
        _newsArticleService = newsArticleService;
        _newsApprovalService = newsApprovalService;
    }

    [AllowAnonymous]
    [EnableQuery]
    [HttpGet]
    public IActionResult Get()
    {
        var publicOnly = User.Identity?.IsAuthenticated != true;
        return Ok(_newsArticleService.QueryList(publicOnly));
    }

    [AllowAnonymous]
    [EnableQuery]
    [HttpGet("Public")]
    public IActionResult Public()
    {
        return Ok(_newsArticleService.QueryList(publicOnly: true));
    }

    [AllowAnonymous]
    [HttpGet("Public/{key}")]
    public async Task<IActionResult> Public(string key)
    {
        var article = await _newsArticleService.GetByIdAsync(key, publicOnly: true);
        return article is null ? NotFound() : Ok(article);
    }

    [AllowAnonymous]
    [HttpGet("{key}")]
    public async Task<IActionResult> Get(string key)
    {
        var publicOnly = User.Identity?.IsAuthenticated != true;
        var article = await _newsArticleService.GetByIdAsync(key, publicOnly);
        return article is null ? NotFound() : Ok(article);
    }

    [Authorize(Policy = AuthorizationPolicies.AdminOrStaff)]
    [HttpPost]
    public Task<NewsArticleDto> Post([FromBody] NewsArticleUpsertDto dto)
    {
        return _newsArticleService.CreateAsync(dto, User.ToCurrentUser());
    }

    [Authorize(Policy = AuthorizationPolicies.AdminOrStaff)]
    [HttpPut("{key}")]
    public Task<NewsArticleDto> Put(string key, [FromBody] NewsArticleUpsertDto dto)
    {
        return _newsArticleService.UpdateAsync(key, dto, User.ToCurrentUser());
    }

    [Authorize(Policy = AuthorizationPolicies.AdminOrStaff)]
    [HttpDelete("{key}")]
    public async Task<IActionResult> Delete(string key)
    {
        await _newsArticleService.DeleteAsync(key, User.ToCurrentUser());
        return NoContent();
    }

    [Authorize(Policy = AuthorizationPolicies.AdminOrStaff)]
    [HttpGet("MyHistory")]
    public Task<List<NewsArticleDto>> MyHistory()
    {
        return _newsArticleService.GetMyHistoryAsync(User.ToCurrentUser());
    }

    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [HttpGet("PendingApproval")]
    public Task<List<NewsArticleDto>> PendingApproval()
    {
        return _newsApprovalService.GetPendingApprovalAsync();
    }

    [Authorize(Policy = AuthorizationPolicies.AdminOrStaff)]
    [HttpPost("{key}/Submit")]
    public Task<NewsArticleDto> Submit(string key)
    {
        return _newsApprovalService.SubmitForApprovalAsync(key, User.ToCurrentUser());
    }

    [Authorize(Policy = AuthorizationPolicies.AdminOrStaff)]
    [HttpPost("{key}/Recall")]
    public Task<NewsArticleDto> Recall(string key)
    {
        return _newsApprovalService.RecallAsync(key, User.ToCurrentUser());
    }

    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [HttpPost("{key}/Approve")]
    public Task<NewsArticleDto> Approve(string key)
    {
        return _newsApprovalService.ApproveAsync(key, User.ToCurrentUser());
    }

    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [HttpPost("{key}/Reject")]
    public Task<NewsArticleDto> Reject(string key, [FromBody] RejectNewsDto dto)
    {
        return _newsApprovalService.RejectAsync(key, User.ToCurrentUser(), dto);
    }
}

