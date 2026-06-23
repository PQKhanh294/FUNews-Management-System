using FUNewsManagementSystem.Services.Dtos;

namespace FUNewsManagementSystem.Services.Tests;

public sealed class NewsArticleListDtoTests
{
    [Fact]
    public void Public_list_contract_does_not_expose_account_secrets()
    {
        var properties = typeof(NewsArticleListDto).GetProperties().Select(property => property.Name).ToList();

        Assert.DoesNotContain("AccountPassword", properties);
        Assert.DoesNotContain("GoogleId", properties);
        Assert.DoesNotContain("AvatarUrl", properties);
    }
}
