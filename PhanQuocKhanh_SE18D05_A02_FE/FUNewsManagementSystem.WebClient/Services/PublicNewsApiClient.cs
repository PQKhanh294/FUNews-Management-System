using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FUNewsManagementSystem.WebClient.Models;

namespace FUNewsManagementSystem.WebClient.Services;

public interface IPublicNewsApiClient
{
    Task<IReadOnlyList<NewsArticleViewModel>> GetArticlesAsync(string? search, IReadOnlyCollection<short>? categoryIds, CancellationToken cancellationToken);
    Task<NewsArticleViewModel?> GetArticleAsync(string id, CancellationToken cancellationToken);
    Task<IReadOnlyList<CategoryTreeViewModel>> GetCategoryTreeAsync(CancellationToken cancellationToken);
}

public sealed class PublicNewsApiClient(HttpClient httpClient) : IPublicNewsApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<IReadOnlyList<NewsArticleViewModel>> GetArticlesAsync(
        string? search,
        IReadOnlyCollection<short>? categoryIds,
        CancellationToken cancellationToken)
    {
        var query = new List<string> { "$orderby=CreatedDate desc" };
        var filters = new List<string>();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var escaped = search.Trim().Replace("'", "''", StringComparison.Ordinal);
            filters.Add($"contains(tolower(NewsTitle),tolower('{escaped}'))");
        }

        if (categoryIds is { Count: > 0 })
        {
            filters.Add("(" + string.Join(" or ", categoryIds.Distinct().Select(id => $"CategoryID eq {id}")) + ")");
        }

        if (filters.Count > 0)
        {
            query.Add("$filter=" + string.Join(" and ", filters));
        }

        var requestUri = "/odata/NewsArticles/Public?" + string.Join("&", query.Select(EncodeQueryPart));
        using var response = await httpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        var payload = document.RootElement.ValueKind == JsonValueKind.Array
            ? document.RootElement
            : document.RootElement.GetProperty("value");
        return payload.Deserialize<List<NewsArticleViewModel>>(JsonOptions) ?? [];
    }

    public async Task<NewsArticleViewModel?> GetArticleAsync(string id, CancellationToken cancellationToken)
    {
        using var response = await httpClient.GetAsync("/odata/NewsArticles/Public/" + Uri.EscapeDataString(id), cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<NewsArticleViewModel>(JsonOptions, cancellationToken);
    }

    public async Task<IReadOnlyList<CategoryTreeViewModel>> GetCategoryTreeAsync(CancellationToken cancellationToken)
    {
        return await httpClient.GetFromJsonAsync<List<CategoryTreeViewModel>>(
            "/odata/Categories/Tree/Public",
            JsonOptions,
            cancellationToken) ?? [];
    }

    private static string EncodeQueryPart(string part)
    {
        var separator = part.IndexOf('=');
        return separator < 0
            ? part
            : part[..(separator + 1)] + Uri.EscapeDataString(part[(separator + 1)..]);
    }
}
