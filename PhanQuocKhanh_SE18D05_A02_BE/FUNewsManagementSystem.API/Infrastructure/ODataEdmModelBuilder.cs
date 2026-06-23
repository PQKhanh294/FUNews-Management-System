using FUNewsManagementSystem.BusinessObjects.Entities;
using Microsoft.OData.ModelBuilder;

namespace FUNewsManagementSystem.API.Infrastructure;

public static class ODataEdmModelBuilder
{
    public static Microsoft.OData.Edm.IEdmModel GetEdmModel()
    {
        var builder = new ODataConventionModelBuilder();
        builder.EntityType<SystemAccount>().HasKey(a => a.AccountID);
        builder.EntityType<Category>().HasKey(c => c.CategoryID);
        builder.EntityType<NewsArticle>().HasKey(n => n.NewsArticleID);
        builder.EntityType<Tag>().HasKey(t => t.TagID);
        builder.EntitySet<SystemAccount>("Accounts");
        builder.EntitySet<Category>("Categories");
        builder.EntitySet<NewsArticle>("NewsArticles");
        builder.EntitySet<Tag>("Tags");
        return builder.GetEdmModel();
    }
}

