namespace FUNewsManagementSystem.WebClient.Models;

public sealed class SeoMetaViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Keywords { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
}

public sealed class TagViewModel
{
    public int TagID { get; set; }
    public string TagName { get; set; } = string.Empty;
}

public sealed class CategorySummaryViewModel
{
    public short CategoryID { get; set; }
    public string CategoryName { get; set; } = string.Empty;
}

public sealed class AccountSummaryViewModel
{
    public short AccountID { get; set; }
    public string? AccountName { get; set; }
}

public sealed class NewsArticleViewModel
{
    public string NewsArticleID { get; set; } = string.Empty;
    public string? NewsTitle { get; set; }
    public string Headline { get; set; } = string.Empty;
    public DateTime? CreatedDate { get; set; }
    public string? NewsContent { get; set; }
    public string? NewsSource { get; set; }
    public short? CategoryID { get; set; }
    public string? CategoryName { get; set; }
    public CategorySummaryViewModel? Category { get; set; }
    public bool? NewsStatus { get; set; }
    public byte ApprovalStatus { get; set; }
    public short? CreatedByID { get; set; }
    public string? CreatedByName { get; set; }
    public AccountSummaryViewModel? CreatedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public List<TagViewModel> Tags { get; set; } = [];
    public SeoMetaViewModel SeoMeta { get; set; } = new();

    public string DisplayTitle => string.IsNullOrWhiteSpace(NewsTitle) ? Headline : NewsTitle;
    public string DisplayCategory => CategoryName ?? Category?.CategoryName ?? "General";
    public string? DisplayAuthor => CreatedByName ?? CreatedBy?.AccountName;
    public string CanonicalSlug => string.IsNullOrWhiteSpace(SeoMeta.Slug) ? SeoSlug.From(DisplayTitle) : SeoMeta.Slug;
}

public sealed class CategoryTreeViewModel
{
    public short CategoryID { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryDesciption { get; set; } = string.Empty;
    public bool? IsActive { get; set; }
    public List<CategoryTreeViewModel> Children { get; set; } = [];

    public IReadOnlyCollection<short> FlattenIds()
    {
        var ids = new List<short> { CategoryID };
        foreach (var child in Children)
        {
            ids.AddRange(child.FlattenIds());
        }

        return ids;
    }
}

public sealed class PublicNewsPageViewModel
{
    public IReadOnlyList<NewsArticleViewModel> Articles { get; init; } = [];
    public IReadOnlyList<CategoryTreeViewModel> Categories { get; init; } = [];
    public string? Search { get; init; }
    public short? SelectedCategoryID { get; init; }
    public string? ErrorMessage { get; init; }
}

public sealed class NewsDetailsViewModel
{
    public required NewsArticleViewModel Article { get; init; }
    public required string NewsContentHtml { get; init; }
    public required string CanonicalUrl { get; init; }
}

internal sealed class ODataEnvelope<T>
{
    public List<T> Value { get; set; } = [];
}
