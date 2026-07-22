using Core.Models;
using Core.Services;
using Moq;
using Shouldly;

namespace Core.Test.Services;

[TestClass]
public sealed class WordGeneratorTests
{
    private const string TARGET_WORD = "ABC";

    private const string OKAY_HINT_1 = "AAA";
    private const string OKAY_HINT_2 = "AAB";
    private const string OKAY_HINT_3 = "ABA";
    private const string OKAY_HINT_4 = "BAA";
    private const string OKAY_HINT_5 = "BBB";
    private const string OKAY_HINT_6 = "CCC";

    private const string BAD_HINT_1 = "WWW";
    private const string BAD_HINT_2 = "XXX";
    private const string BAD_HINT_3 = "YYY";
    private const string BAD_HINT_4 = "ZZZ";
    private readonly GameSettings _gameSettings = new() { WordLength = 3, MaxGuesses = 6 };
    private readonly Mock<IWordRepository> _mockRepo = new();

    [TestMethod]
    public async Task GenerateHintsAsync_WhenRepositoryHasWords_ReturnsHintsWithinLimits()
    {
        var words = new HashSet<string> { OKAY_HINT_1, OKAY_HINT_2, OKAY_HINT_3, OKAY_HINT_4, OKAY_HINT_5, OKAY_HINT_6 };
        _mockRepo.Setup(r => r.GetAllowedHints(3)).ReturnsAsync(words);

        IReadOnlyList<string> hints = await Sut().GenerateHintsAsync(_gameSettings, TARGET_WORD);

        hints.ShouldAllBe(h => words.Contains(h));
        hints.Count.ShouldBeLessThanOrEqualTo(_gameSettings.MaxGuesses / 3);
    }

    [TestMethod]
    public async Task GenerateHintsAsync_WhenOnlyBadHintsExist_StillCapsHints()
    {
        var words = new HashSet<string> { BAD_HINT_1, BAD_HINT_2, BAD_HINT_3, BAD_HINT_4 };
        _mockRepo.Setup(r => r.GetAllowedHints(3)).ReturnsAsync(words);

        IReadOnlyList<string> hints = await Sut().GenerateHintsAsync(_gameSettings, TARGET_WORD);

        hints.ShouldAllBe(h => words.Contains(h));
        hints.Count.ShouldBeLessThanOrEqualTo(_gameSettings.MaxGuesses / 3);
    }

    [TestMethod]
    public async Task GenerateHintsAsync_WhenFoundPerfectHint_Ignores()
    {
        var words = new HashSet<string> { TARGET_WORD, BAD_HINT_1 };
        _mockRepo.Setup(r => r.GetAllowedHints(3)).ReturnsAsync(words);

        IReadOnlyList<string> hints = await Sut().GenerateHintsAsync(_gameSettings, TARGET_WORD);

        hints.ShouldHaveSingleItem(BAD_HINT_1);
    }

    [TestMethod]
    public async Task GenerateHintsAsync_WhenNoValidHintsExist_ReturnsEmptyList()
    {
        var words = new HashSet<string> { TARGET_WORD };
        _mockRepo.Setup(r => r.GetAllowedHints(3)).ReturnsAsync(words);

        IReadOnlyList<string> hints = await Sut().GenerateHintsAsync(_gameSettings, TARGET_WORD);

        hints.ShouldBeEmpty();
    }

    private WordGenerator Sut() => new(_mockRepo.Object);
}