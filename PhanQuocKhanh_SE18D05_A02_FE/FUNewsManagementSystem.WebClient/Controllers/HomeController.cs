using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FUNewsManagementSystem.WebClient.Models;
using FUNewsManagementSystem.WebClient.Services;

namespace FUNewsManagementSystem.WebClient.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IPublicNewsApiClient _publicNewsApiClient;
    private readonly IRichTextSanitizer _richTextSanitizer;

    public HomeController(
        ILogger<HomeController> logger,
        IPublicNewsApiClient publicNewsApiClient,
        IRichTextSanitizer richTextSanitizer)
    {
        _logger = logger;
        _publicNewsApiClient = publicNewsApiClient;
        _richTextSanitizer = richTextSanitizer;
    }

    public async Task<IActionResult> Index(
        string? search,
        short? category,
        CancellationToken cancellationToken)
    {
        try
        {
            var categories = await _publicNewsApiClient.GetCategoryTreeAsync(cancellationToken);
            IReadOnlyCollection<short>? categoryIds = null;
            if (category is not null)
            {
                categoryIds = FindCategory(categories, category.Value)?.FlattenIds() ?? [category.Value];
            }

            var articles = await _publicNewsApiClient.GetArticlesAsync(search, categoryIds, cancellationToken);
            return View(new PublicNewsPageViewModel
            {
                Articles = articles,
                Categories = categories,
                Search = search,
                SelectedCategoryID = category
            });
        }
        catch (HttpRequestException exception)
        {
            _logger.LogError(exception, "Unable to load public news from the API.");
            return View(new PublicNewsPageViewModel
            {
                Search = search,
                SelectedCategoryID = category,
                ErrorMessage = "Public news is temporarily unavailable."
            });
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Login() => View();

    public IActionResult News() => View();

    [HttpGet("/news/{id}/{slug?}", Name = "NewsArticle")]
    [HttpGet("/Home/NewsDetails/{id}")]
    public async Task<IActionResult> NewsDetails(
        string id,
        string? slug,
        CancellationToken cancellationToken)
    {
        var article = await _publicNewsApiClient.GetArticleAsync(id, cancellationToken);
        if (article is null || article.NewsStatus != true || article.ApprovalStatus != 2)
        {
            return NotFound();
        }

        var canonicalSlug = article.SeoMeta.Slug;
        if (!string.Equals(slug, canonicalSlug, StringComparison.OrdinalIgnoreCase) ||
            Request.Path.StartsWithSegments("/Home/NewsDetails"))
        {
            return RedirectToRoutePermanent("NewsArticle", new { id, slug = canonicalSlug });
        }

        var canonicalUrl = $"{Request.Scheme}://{Request.Host}/news/{Uri.EscapeDataString(id)}/{canonicalSlug}";
        return View(new NewsDetailsViewModel
        {
            Article = article,
            NewsContentHtml = _richTextSanitizer.Sanitize(article.NewsContent),
            CanonicalUrl = canonicalUrl
        });
    }

    public IActionResult Accounts() => View();

    public IActionResult Dashboard() => View();

    public IActionResult Approval() => View();

    public IActionResult History() => RedirectToAction(nameof(News));

    public IActionResult Profile() => View();

    private static CategoryTreeViewModel? FindCategory(
        IEnumerable<CategoryTreeViewModel> categories,
        short id)
    {
        foreach (var category in categories)
        {
            if (category.CategoryID == id)
            {
                return category;
            }

            var match = FindCategory(category.Children, id);
            if (match is not null)
            {
                return match;
            }
        }

        return null;
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

