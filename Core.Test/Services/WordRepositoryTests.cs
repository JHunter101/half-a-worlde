using System.Net;
using Core.Services;
using Shouldly;

namespace Core.Test.Services;

[TestClass]
public sealed class WordRepositoryTests
{
    [TestMethod]
    public async Task GetWordsAsync_LoadsAndCachesWords()
    {
        var sut = Sut("APPLE\nBANAN\n");

        var words = await sut.GetWordsAsync(5);
        words.ShouldContain("APPLE");
        words.ShouldContain("BANAN");

        var cachedWords = await sut.GetWordsAsync(5);
        ReferenceEquals(words, cachedWords).ShouldBeTrue();
    }

    private static WordRepository Sut(string response)
    {
        var http = new HttpClient(new TestHttpMessageHandler(response))
        {
            BaseAddress = new Uri("http://localhost/")
        };

        return new WordRepository(http);
    }

    private sealed class TestHttpMessageHandler(string response) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(response)
            });
        }
    }
}