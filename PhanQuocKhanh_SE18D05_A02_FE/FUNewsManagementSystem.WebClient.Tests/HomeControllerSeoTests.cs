using FUNewsManagementSystem.WebClient.Controllers;
using FUNewsManagementSystem.WebClient.Models;
using FUNewsManagementSystem.WebClient.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace FUNewsManagementSystem.WebClient.Tests;

public sealed class HomeControllerSeoTests
{
    [Fact]
    public async Task NewsDetails_redirects_missing_slug_to_canonical_route()
    {
        var client = new FakePublicNewsApiClient { Article = TestArticle() };
        var controller = CreateController(client);

        var result = await controller.NewsDetails("N001", null, CancellationToken.None);

        var redirect = Assert.IsType<RedirectToRouteResult>(result);
        Assert.True(redirect.Permanent);
        Assert.Equal("NewsArticle", redirect.RouteName);
        Assert.Equal("first-campus-story", redirect.RouteValues!["slug"]);
    }

    [Fact]
    public async Task NewsDetails_returns_404_when_article_is_not_public()
    {
        var controller = CreateController(new FakePublicNewsApiClient());

        var result = await controller.NewsDetails("missing", "missing", CancellationToken.None);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task NewsDetails_builds_absolute_canonical_url_and_sanitizes_content()
    {
        var client = new FakePublicNewsApiClient { Article = TestArticle() };
        var controller = CreateController(client);

        var result = await controller.NewsDetails("N001", "first-campus-story", CancellationToken.None);

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<NewsDetailsViewModel>(view.Model);
        Assert.Equal("https://localhost:7036/news/N001/first-campus-story", model.CanonicalUrl);
        Assert.Equal("<p>Safe content</p>", model.NewsContentHtml);
    }

    private static HomeController CreateController(IPublicNewsApiClient client)
    {
        var controller = new HomeController(
            NullLogger<HomeController>.Instance,
            client,
            new FakeRichTextSanitizer());
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        controller.Request.Scheme = "https";
        controller.Request.Host = new HostString("localhost", 7036);
        return controller;
    }

    private static NewsArticleViewModel TestArticle() => new()
    {
        NewsArticleID = "N001",
        NewsTitle = "First campus story",
        Headline = "Headline",
        NewsContent = "<p>Unsafe source</p>",
        NewsStatus = true,
        ApprovalStatus = 2,
        SeoMeta = new SeoMetaViewModel
        {
            Title = "First campus story",
            Description = "Campus description",
            Slug = "first-campus-story"
        }
    };

    private sealed class FakePublicNewsApiClient : IPublicNewsApiClient
    {
        public NewsArticleViewModel? Article { get; init; }

        public Task<IReadOnlyList<NewsArticleViewModel>> GetArticlesAsync(
            string? search,
            IReadOnlyCollection<short>? categoryIds,
            CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<NewsArticleViewModel>>(Article is null ? [] : [Article]);

        public Task<NewsArticleViewModel?> GetArticleAsync(string id, CancellationToken cancellationToken) =>
            Task.FromResult(Article);

        public Task<IReadOnlyList<CategoryTreeViewModel>> GetCategoryTreeAsync(CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<CategoryTreeViewModel>>([]);
    }

    private sealed class FakeRichTextSanitizer : IRichTextSanitizer
    {
        public string Sanitize(string? html) => "<p>Safe content</p>";
    }
}
