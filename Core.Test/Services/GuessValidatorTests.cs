using Core.Services;
using Moq;
using Shouldly;

namespace Core.Test.Services;

[TestClass]
public sealed class GuessValidatorTests
{
    private readonly Mock<IWordRepository> _mockRepo = new();

    [TestInitialize]
    public void Initialize() => _mockRepo.Setup(r => r.GetWordsAsync(It.IsAny<int>())).ReturnsAsync([]);

    [TestMethod]
    public async Task IsValid_WhenLengthMismatch_ThenReturnsFalse()
    {
        bool result = await Sut().IsValid("TOO", 5);

        result.ShouldBeFalse();
    }

    [TestMethod]
    public async Task IsValid_WhenWordNotInRepository_ThenReturnsFalse()
    {
        _mockRepo.Setup(r => r.GetWordsAsync(5)).ReturnsAsync(["APPLE"]);

        bool result = await Sut().IsValid("OTHER", 5);

        result.ShouldBeFalse();
    }

    [TestMethod]
    public async Task IsValid_WhenWordPresentAndLengthMatches_ThenReturnsTrue()
    {
        _mockRepo.Setup(r => r.GetWordsAsync(5)).ReturnsAsync(["APPLE"]);

        bool result = await Sut().IsValid("APPLE", 5);

        result.ShouldBeTrue();
    }

    private GuessValidator Sut() => new(_mockRepo.Object);
}