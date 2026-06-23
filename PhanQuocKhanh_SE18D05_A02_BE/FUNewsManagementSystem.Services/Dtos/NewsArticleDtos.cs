namespace FUNewsManagementSystem.Services.Dtos;

public sealed record SeoMetaDto(string Title, string Description, string Keywords, string Slug);

public sealed class NewsArticleListDto
{
    public string NewsArticleID { get; init; } = string.Empty;
    public string? NewsTitle { get; init; }
    public string Headline { get; init; } = string.Empty;
    public DateTime? CreatedDate { get; init; }
    public short? CategoryID { get; init; }
    public string? CategoryName { get; init; }
    public bool? NewsStatus { get; init; }
    public byte ApprovalStatus { get; init; }
    public short? CreatedByID { get; init; }
    public string? CreatedByName { get; init; }
    public DateTime? ModifiedDate { get; init; }
}

public sealed record NewsArticleDto(
    string NewsArticleID,
    string? NewsTitle,
    string Headline,
    DateTime? CreatedDate,
    string? NewsContent,
    string? NewsSource,
    short? CategoryID,
    string? CategoryName,
    bool? NewsStatus,
    byte ApprovalStatus,
    short? CreatedByID,
    string? CreatedByName,
    DateTime? ModifiedDate,
    List<TagDto> Tags,
    SeoMetaDto SeoMeta);

public sealed record NewsArticleUpsertDto(
    string? NewsTitle,
    string Headline,
    string? NewsContent,
    string? NewsSource,
    short CategoryID,
    bool NewsStatus,
    List<int> TagIds);

public sealed record RejectNewsDto(string? Reason);

