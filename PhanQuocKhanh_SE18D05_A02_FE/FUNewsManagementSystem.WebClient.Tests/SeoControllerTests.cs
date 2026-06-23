using FUNewsManagementSystem.WebClient.Controllers;
using FUNewsManagementSystem.WebClient.Models;
using FUNewsManagementSystem.WebClient.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FUNewsManagementSystem.WebClient.Tests;

public sealed class SeoControllerTests
{
    [Fact]
    public async Task Sitemap_contains_only_approved_active_articles()
    {
        var client = new FakePublicNewsApiClient([
            Article("A1", true, 2, "approved"),
            Article("A2", false, 2, "inactive"),
            Article("A3", true, 1, "pending")
        ]);
        var controller = new SeoController(client)
        {
            ControllerContext = new ControllerContext { HttpContext = HttpContext() }
        };

        var result = await controller.Sitemap(CancellationToken.None);

        var content = Assert.IsType<ContentResult>(result);
        Assert.Contains("/news/A1/approved", content.Content);
        Assert.DoesNotContain("/news/A2/inactive", content.Content);
        Assert.DoesNotContain("/news/A3/pending", content.Content);
        Assert.Equal("application/xml; charset=utf-8", content.ContentType);
    }

    [Fact]
    public void Robots_references_the_current_site_sitemap()
    {
        var controller = new SeoController(new FakePublicNewsApiClient([]))
        {
            ControllerContext = new ControllerContext { HttpContext = HttpContext() }
        };

        var result = controller.Robots();

        var content = Assert.IsType<ContentResult>(result);
        Assert.Contains("Sitemap: https://localhost:7036/sitemap.xml", content.Content);
        Assert.Contains("Disallow: /Home/Approval", content.Content);
    }

    private static DefaultHttpContext HttpContext()
    {
        var context = new DefaultHttpContext();
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("localhost", 7036);
        return context;
    }

    private static NewsArticleViewModel Article(string id, bool status, byte approval, string slug) => new()
    {
        NewsArticleID = id,
        NewsTitle = id,
        Headline = id,
        NewsStatus = status,
        ApprovalStatus = approval,
        SeoMeta = new SeoMetaViewModel { Title = id, Description = id, Slug = slug }
    };

    private sealed class FakePublicNewsApiClient(IReadOnlyList<NewsArticleViewModel> articles) : IPublicNewsApiClient
    {
        public Task<IReadOnlyList<NewsArticleViewModel>> GetArticlesAsync(string? search, IReadOnlyCollection<short>? categoryIds, CancellationToken cancellationToken) => Task.FromResult(articles);
        public Task<NewsArticleViewModel?> GetArticleAsync(string id, CancellationToken cancellationToken) => Task.FromResult<NewsArticleViewModel?>(articles.FirstOrDefault(a => a.NewsArticleID == id));
        public Task<IReadOnlyList<CategoryTreeViewModel>> GetCategoryTreeAsync(CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<CategoryTreeViewModel>>([]);
    }
}
