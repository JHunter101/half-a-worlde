using Core.Models;
using Core.Services;
using Moq;
using Shouldly;

namespace Core.Test.Services;

[TestClass]
public sealed class WordGeneratorTests
{
    private readonly Mock<IWordRepository> _mockRepo = new();
    private readonly GameSettings _gameSettings = new() { WordLength = 3, MaxGuesses = 6 };

    [TestMethod]
    public async Task WordGenerator_GenerateHintsAsync_WhenRepositoryHasWords_ReturnsHintsWithinLimits()
    {
        var words = new HashSet<string> { "AAA", "AAB", "ABA", "BAA", "BBB", "CCC" };
        _mockRepo.Setup(r => r.GetWordsAsync(3)).ReturnsAsync(words);

        var hints = await Sut().GenerateHintsAsync(_gameSettings, "ABC");

        hints.ShouldAllBe(h => words.Contains(h));
        hints.Count.ShouldBeLessThanOrEqualTo(_gameSettings.MaxGuesses / 3);
    }

    [TestMethod]
    public async Task WordGenerator_GenerateHintsAsync_WhenOnlyBadHintsExist_StillCapsHints()
    {
        var words = new HashSet<string> { "XXX", "YYY", "ZZZ", "WWW" };
        _mockRepo.Setup(r => r.GetWordsAsync(3)).ReturnsAsync(words);

        var hints = await Sut().GenerateHintsAsync(_gameSettings, "ABC");

        hints.ShouldAllBe(h => words.Contains(h));
        hints.Count.ShouldBeLessThanOrEqualTo(_gameSettings.MaxGuesses / 3);
    }

    private WordGenerator Sut() => new(_mockRepo.Object);
}