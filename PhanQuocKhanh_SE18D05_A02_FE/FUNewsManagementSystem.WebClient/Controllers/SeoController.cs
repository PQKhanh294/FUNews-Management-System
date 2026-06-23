using System.Text;
using System.Xml.Linq;
using FUNewsManagementSystem.WebClient.Services;
using Microsoft.AspNetCore.Mvc;

namespace FUNewsManagementSystem.WebClient.Controllers;

public sealed class SeoController(IPublicNewsApiClient publicNewsApiClient) : Controller
{
    [HttpGet("/sitemap.xml")]
    [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> Sitemap(CancellationToken cancellationToken)
    {
        var articles = await publicNewsApiClient.GetArticlesAsync(null, null, cancellationToken);
        XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
        var document = new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            new XElement(ns + "urlset",
                new XElement(ns + "url",
                    new XElement(ns + "loc", SiteUrl("/")),
                    new XElement(ns + "changefreq", "daily"),
                    new XElement(ns + "priority", "1.0")),
                articles
                    .Where(article => article.NewsStatus == true && article.ApprovalStatus == 2)
                    .Select(article => new XElement(ns + "url",
                        new XElement(ns + "loc", SiteUrl($"/news/{Uri.EscapeDataString(article.NewsArticleID)}/{article.CanonicalSlug}")),
                        new XElement(ns + "lastmod", (article.ModifiedDate ?? article.CreatedDate ?? DateTime.UtcNow).ToString("yyyy-MM-dd")),
                        new XElement(ns + "changefreq", "weekly"),
                        new XElement(ns + "priority", "0.8")))));

        return Content(document.ToString(), "application/xml", Encoding.UTF8);
    }

    [HttpGet("/robots.txt")]
    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
    public IActionResult Robots()
    {
        var content = $"""
            User-agent: *
            Allow: /
            Disallow: /Home/Login
            Disallow: /Home/Accounts
            Disallow: /Home/Approval
            Disallow: /Home/Dashboard
            Disallow: /Home/News

            Sitemap: {SiteUrl("/sitemap.xml")}
            """;
        return Content(content, "text/plain", Encoding.UTF8);
    }

    private string SiteUrl(string path) => $"{Request.Scheme}://{Request.Host}{path}";
}
