using FUNewsManagementSystem.Services.Dtos;
using FUNewsManagementSystem.Services.Exceptions;
using FUNewsManagementSystem.Services.Helpers;

namespace FUNewsManagementSystem.Services.Validators;

public interface INewsArticleValidator
{
    void Validate(NewsArticleUpsertDto dto);
}

public sealed class NewsArticleValidator : INewsArticleValidator
{
    private readonly ISeoMetadataBuilder _seoMetadataBuilder;

    public NewsArticleValidator(ISeoMetadataBuilder seoMetadataBuilder)
    {
        _seoMetadataBuilder = seoMetadataBuilder;
    }

    public void Validate(NewsArticleUpsertDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.NewsTitle))
        {
            throw new AppException("News title is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.Headline))
        {
            throw new AppException("Headline is required.");
        }

        if (dto.CategoryID <= 0)
        {
            throw new AppException("Category is required.");
        }

        if (string.IsNullOrWhiteSpace(_seoMetadataBuilder.StripHtml(dto.NewsContent)))
        {
            throw new AppException("News content is required.");
        }
    }
}

