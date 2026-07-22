using System.Net;
using Core.Services;
using Shouldly;

namespace Core.Test.Services;

[TestClass]
public sealed class WordRepositoryTests
{
    [TestMethod]
    public async Task GetAllowedTargets_LoadsAndCachesWords()
    {
        const string TARGET_DATA = "TRGT1\nTRGT2\n";
        WordRepository sut = Sut(TARGET_DATA);

        HashSet<string> words = await sut.GetAllowedTargets(5);
        words.ShouldContain("TRGT1");
        words.ShouldContain("TRGT2");

        HashSet<string> cachedWords = await sut.GetAllowedTargets(5);
        ReferenceEquals(words, cachedWords).ShouldBeTrue();
    }

    [TestMethod]
    public async Task GetAllowedGuesses_LoadsAndCachesWords()
    {
        const string GUESS_DATA = "GUES1\nGUES2\n";
        WordRepository sut = Sut(GUESS_DATA);

        HashSet<string> words = await sut.GetAllowedGuesses(5);
        words.ShouldContain("GUES1");
        words.ShouldContain("GUES2");

        HashSet<string> cachedWords = await sut.GetAllowedGuesses(5);
        ReferenceEquals(words, cachedWords).ShouldBeTrue();
    }

    [TestMethod]
    public async Task GetAllowedHints_LoadsAndCachesWords()
    {
        const string HINT_DATA = "HINT1\nHINT2\n";
        WordRepository sut = Sut(HINT_DATA);

        HashSet<string> words = await sut.GetAllowedHints(5);
        words.ShouldContain("HINT1");
        words.ShouldContain("HINT2");

        HashSet<string> cachedWords = await sut.GetAllowedHints(5); // Fixed: was calling GetAllowedGuesses
        ReferenceEquals(words, cachedWords).ShouldBeTrue();
    }

    [TestMethod]
    public async Task GetAllowed_AllMethodsShareTheSameCacheInstance()
    {
        WordRepository sut = Sut("");

        HashSet<string> hints = await sut.GetAllowedHints(5);
        HashSet<string> guesses = await sut.GetAllowedGuesses(5);
        HashSet<string> targets = await sut.GetAllowedTargets(5);

        ReferenceEquals(hints, guesses).ShouldBeTrue();
        ReferenceEquals(guesses, targets).ShouldBeTrue();
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
            request.RequestUri!.ToString().ShouldMatch(@"^http://localhost/data/\d+\.txt$");

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(response)
            });
        }
    }
}