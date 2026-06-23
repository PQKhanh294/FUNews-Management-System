using Ganss.Xss;

namespace FUNewsManagementSystem.WebClient.Services;

public interface IRichTextSanitizer
{
    string Sanitize(string? html);
}

public sealed class RichTextSanitizer : IRichTextSanitizer
{
    private readonly HtmlSanitizer _sanitizer = new();

    public RichTextSanitizer()
    {
        _sanitizer.AllowedAttributes.Add("class");
        _sanitizer.AllowedSchemes.Add("mailto");
    }

    public string Sanitize(string? html) => _sanitizer.Sanitize(html ?? string.Empty);
}
