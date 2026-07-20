using Core.Services;
using Moq;
using Shouldly;

namespace Core.Test.Services;

[TestClass]
public sealed class WordServiceTests
{
    private readonly Mock<IWordRepository> _mockRepo = new();

    [TestMethod]
    public async Task GetWordAsync_ReturnsWordFromRepository()
    {
        HashSet<string> wordlist = ["APPLE", "OTHER"];
        _mockRepo.Setup(r => r.GetWordsAsync(5)).ReturnsAsync(wordlist);

        string word = await Sut().GetWordAsync(5);

        wordlist.ShouldContain(word);
    }

    private WordService Sut() => new(_mockRepo.Object);
}