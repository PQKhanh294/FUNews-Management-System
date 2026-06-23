using System.Net;
using System.Text;
using FUNewsManagementSystem.WebClient.Services;

namespace FUNewsManagementSystem.WebClient.Tests;

public sealed class PublicNewsApiClientTests
{
    [Fact]
    public async Task GetArticles_accepts_direct_array_returned_by_current_odata_endpoint()
    {
        const string json = """
            [{"NewsArticleID":"N1","NewsTitle":"Story","Headline":"Story","NewsStatus":true,"ApprovalStatus":2}]
            """;
        var httpClient = new HttpClient(new JsonHandler(json))
        {
            BaseAddress = new Uri("https://api.test")
        };
        var client = new PublicNewsApiClient(httpClient);

        var articles = await client.GetArticlesAsync(null, null, CancellationToken.None);

        Assert.Equal("N1", Assert.Single(articles).NewsArticleID);
    }

    private sealed class JsonHandler(string json) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Assert.DoesNotContain("CreatedBy", request.RequestUri!.Query);
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });
        }
    }
}
