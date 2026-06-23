using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace FUNewsManagementSystem.WebClient.Models;

public static class SeoSlug
{
    public static string From(string? text)
    {
        var normalized = (text ?? string.Empty).ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder();
        foreach (var character in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            builder.Append(char.IsLetterOrDigit(character) ? character : '-');
        }

        return Regex.Replace(builder.ToString().Normalize(NormalizationForm.FormC), "-+", "-").Trim('-');
    }
}
