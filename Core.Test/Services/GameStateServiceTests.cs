using CommunityToolkit.Diagnostics;
using Core.Models;
using Core.Services;
using Moq;
using Shouldly;

namespace Core.Test.Services;

[TestClass]
public sealed class GameStateServiceTests
{
    private readonly Mock<IWordRepository> _mockWordRepository = new();
    private readonly Mock<IGuessValidator> _mockValidator = new();
    private readonly Mock<IWordGenerator> _mockGenerator = new();
    private readonly GameSettings _gameSettings = new() { WordLength = 5, MaxGuesses = 6 };
    private GameStateService _sut = null!;

    [TestInitialize]
    public void Initialize()
    {
        _mockWordRepository.Setup(s => s.GetAllowedTargets(It.IsAny<int>())).ReturnsAsync(["APPLE"]);
        _mockValidator.Setup(v => v.IsValid(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(true);
        _mockGenerator.Setup(g => g.GenerateHintsAsync(It.IsAny<GameSettings>(), It.IsAny<string>())).ReturnsAsync([]);
        _sut = new GameStateService(_mockWordRepository.Object, _mockValidator.Object, _mockGenerator.Object);
    }

    [TestMethod]
    public async Task HandleInputAsync_WhenEnterAndGuessInvalid_InvokesOnInvalidAndDoesNotAddGuessAsync()
    {
        _mockValidator.Setup(v => v.IsValid(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(false);

        bool invalidCalled = false;
        _sut.OnInvalid += () => invalidCalled = true;

        await _sut.StartNewGameAsync(_gameSettings);
        Guard.IsNotNull(_sut.Session);

        foreach (char c in "ABCDE")
        {
            await _sut.HandleInputAsync(c.ToString());
        }

        await _sut.HandleInputAsync("ENTER");

        invalidCalled.ShouldBeTrue();
        _sut.Session.Rows.Count.ShouldBe(0);
    }

    [TestMethod]
    public async Task HandleInputAsync_WhenEnterAndGuessValid_AddsGuessAndInvokesOnSuccessAsync()
    {
        bool successCalled = false;
        _sut.OnSuccess += () => successCalled = true;

        await _sut.StartNewGameAsync(_gameSettings);
        Guard.IsNotNull(_sut.Session);

        foreach (char c in "APPLE")
        {
            await _sut.HandleInputAsync(c.ToString());
        }

        await _sut.HandleInputAsync("ENTER");

        successCalled.ShouldBeTrue();
        _sut.Session.Rows.Count.ShouldBe(1);
        _sut.Session.State.ShouldBe(SessionState.Won);
    }

    [TestMethod]
    [DataRow("BACKSPACE")]
    [DataRow("DEL")]
    public async Task HandleInputAsync_WhenDeleteInput_RemovesLetterAndInvokesOnChangeAsync(string input)
    {
        await _sut.StartNewGameAsync(_gameSettings);
        Guard.IsNotNull(_sut.Session);

        await _sut.HandleInputAsync("A");

        bool changed = false;
        _sut.OnChange += () => changed = true;

        await _sut.HandleInputAsync(input);

        changed.ShouldBeTrue();
        _sut.Session.Rows.Count.ShouldBe(0);
        _sut.Session.CurrentInput.Length.ShouldBe(0);
    }

    [TestMethod]
    public async Task StartNewGameAsync_WhenHintsPending_ProcessesFirstHintAndLocksSessionAsync()
    {
        _mockGenerator.Setup(g => g.GenerateHintsAsync(It.IsAny<GameSettings>(), It.IsAny<string>()))
            .ReturnsAsync(["APPLE"]);

        bool successCalled = false;
        _sut.OnSuccess += () => successCalled = true;

        await _sut.StartNewGameAsync(_gameSettings);
        Guard.IsNotNull(_sut.Session);

        successCalled.ShouldBeTrue();
        _sut.Session.Rows.Count.ShouldBe(1);
        _sut.Session.IsLocked.ShouldBeTrue();
    }

    [TestMethod]
    public async Task NotifyAnimationComplete_WhenPendingHints_ProcessesAllHintsAndEventuallyUnlocksAsync()
    {
        _mockGenerator.Setup(g => g.GenerateHintsAsync(It.IsAny<GameSettings>(), It.IsAny<string>()))
            .ReturnsAsync(["APPLE", "APPLE"]);

        await _sut.StartNewGameAsync(_gameSettings);
        Guard.IsNotNull(_sut.Session);

        _sut.Session.Rows.Count.ShouldBe(1);
        _sut.Session.IsLocked.ShouldBeTrue();

        _sut.NotifyAnimationComplete();

        _sut.Session.Rows.Count.ShouldBe(2);
        _sut.Session.IsLocked.ShouldBeTrue();

        _sut.NotifyAnimationComplete();

        _sut.Session.IsLocked.ShouldBeFalse();
    }

    [TestMethod]
    public async Task HandleInputAsync_WhenSessionNull_DoesNotInvokeOnChangeAsync()
    {
        var localSut = new GameStateService(_mockWordRepository.Object, _mockValidator.Object, _mockGenerator.Object);
        bool changed = false;
        localSut.OnChange += () => changed = true;

        await localSut.HandleInputAsync("A");

        changed.ShouldBeFalse();
    }

    [TestMethod]
    public async Task HandleInputAsync_WhenSessionLocked_IgnoresInputAsync()
    {
        await _sut.StartNewGameAsync(_gameSettings);
        Guard.IsNotNull(_sut.Session);

        _sut.Session.Lock();

        bool changed = false;
        _sut.OnChange += () => changed = true;

        await _sut.HandleInputAsync("A");

        changed.ShouldBeFalse();
    }
}