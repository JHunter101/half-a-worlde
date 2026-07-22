using Core.Services;
using Moq;
using Shouldly;

namespace Core.Test.Services;

[TestClass]
public sealed class GuessValidatorTests
{
    private readonly Mock<IWordRepository> _mockRepo = new();

    [TestInitialize]
    public void Initialize() => _mockRepo.Setup(r => r.GetAllowedGuesses(It.IsAny<int>())).ReturnsAsync(["APPLE"]);

    [TestMethod]
    public async Task IsValid_WhenLengthMismatch_ThenReturnsFalse()
        => (await Sut().IsValid("TOO", 5)).ShouldBeFalse();

    [TestMethod]
    public async Task IsValid_WhenWordNotInRepository_ThenReturnsFalse()
        => (await Sut().IsValid("OTHER", 5)).ShouldBeFalse();

    [TestMethod]
    public async Task IsValid_WhenWordPresentAndLengthMatches_ThenReturnsTrue()
        => (await Sut().IsValid("APPLE", 5)).ShouldBeTrue();

    private GuessValidator Sut() => new(_mockRepo.Object);
}