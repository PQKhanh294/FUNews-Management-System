using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using FUNewsManagementSystem.Services.Dtos;
using FUNewsManagementSystem.BusinessObjects.Entities;

namespace FUNewsManagementSystem.Services.Helpers;

public interface ISeoMetadataBuilder
{
    SeoMetaDto Build(NewsArticle article);
    string StripHtml(string? html);
    string Slugify(string? text);
}

public sealed class SeoMetadataBuilder : ISeoMetadataBuilder
{
    public SeoMetaDto Build(NewsArticle article)
    {
        var title = string.IsNullOrWhiteSpace(article.NewsTitle) ? article.Headline : article.NewsTitle!;
        var plainText = StripHtml(article.NewsContent);
        var description = plainText.Length <= 155 ? plainText : plainText[..155].TrimEnd() + "...";
        var keywords = string.Join(", ", article.Tags.Select(t => t.TagName).Where(t => !string.IsNullOrWhiteSpace(t)));
        return new SeoMetaDto(title, description, keywords, Slugify(title));
    }

    public string StripHtml(string? html)
    {
        var withoutTags = Regex.Replace(html ?? string.Empty, "<.*?>", string.Empty);
        return Regex.Replace(withoutTags, "\\s+", " ").Trim();
    }

    public string Slugify(string? text)
    {
        var normalized = (text ?? string.Empty).ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder();
        foreach (var character in normalized)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(character);
            if (category == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            builder.Append(char.IsLetterOrDigit(character) ? character : '-');
        }

        return Regex.Replace(builder.ToString().Normalize(NormalizationForm.FormC), "-+", "-").Trim('-');
    }
}

