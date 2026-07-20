using Core.Models;
using Core.Types;
using Shouldly;

namespace Core.Test.Models;

[TestClass]
public sealed class GameSessionTests
{
    private const int TEST_WORD_LENGTH = 1;
    private static readonly GuessWord IncorrectGuess = new([new('A', LetterState.NoMoreMatches)]);
    private static readonly GuessWord CorrectGuess = new([new('A', LetterState.ExactMatch)]);

    [TestMethod]
    public void AddGuess_WhenIncorrectGuess_AndNotAtMaxGuesses_ThenStateRemainsPlaying()
    {
        var session = Sut();

        session.AddGuess(IncorrectGuess);

        session.State.ShouldBe(SessionState.Playing);
    }

    [TestMethod]
    public void AddGuess_WhenIncorrectGuess_AndAtMaxGuesses_ThenStateIsLost()
    {
        var session = Sut(maxGuesses: 1);

        session.AddGuess(IncorrectGuess);

        session.State.ShouldBe(SessionState.Lost);
    }

    [TestMethod]
    public void AddGuess_WhenCorrectGuess_ThenStateIsWon()
    {
        var session = Sut();

        session.AddGuess(CorrectGuess);

        session.State.ShouldBe(SessionState.Won);
    }

    [TestMethod]
    public void AddLetter_WhenNonLetterOrExceedsLength_ThenIgnoresAndEnforcesLength()
    {
        var session = Sut(wordLength: 3);

        session.AddLetter('a'); // A
        session.AddLetter('1'); // ignored
        session.AddLetter('b'); // AB
        session.AddLetter('c'); // ABC
        session.AddLetter('d'); // ignored due to length

        session.CurrentInput.ShouldBe("ABC");
    }

    [TestMethod]
    public void RemoveLetter_WhenCalled_ThenRemovesLastCharacter()
    {
        var session = Sut(wordLength: 5);

        session.AddLetter('x');
        session.AddLetter('y');
        session.AddLetter('z');

        session.CurrentInput.ShouldBe("XYZ");

        session.RemoveLetter();
        session.CurrentInput.ShouldBe("XY");

        session.RemoveLetter();
        session.RemoveLetter();
        session.CurrentInput.ShouldBe(string.Empty);
    }

    [TestMethod]
    public void ClearInput_WhenCalled_ThenEmptiesCurrentInput()
    {
        var session = Sut(wordLength: 3);

        session.AddLetter('a');
        session.AddLetter('b');
        session.CurrentInput.ShouldNotBeEmpty();

        session.ClearInput();
        session.CurrentInput.ShouldBe(string.Empty);
    }

    [TestMethod]
    public void RevealKeyStates_WhenCalledAfterAddGuess_ThenUpdatesVisibleKeyStates()
    {
        var session = Sut(wordLength: 3);

        session.VisibleKeyStates['A'].ShouldBe(LetterState.Pending);

        var guess = new GuessWord(new[]
        {
            new GuessLetter('A', LetterState.Elsewhere),
            new GuessLetter('B', LetterState.ExactMatch),
            new GuessLetter('C', LetterState.NoMoreMatches)
        });

        session.AddGuess(guess);
        session.VisibleKeyStates['A'].ShouldBe(LetterState.Pending);

        session.RevealKeyStates();
        session.VisibleKeyStates['A'].ShouldBe(LetterState.Elsewhere);
        session.VisibleKeyStates['B'].ShouldBe(LetterState.ExactMatch);
        session.VisibleKeyStates['C'].ShouldBe(LetterState.NoMoreMatches);
    }

    [TestMethod]
    public void AddGuess_WhenLaterGuessHasHigherState_ThenKeyStatePromotesToHighest()
    {
        var session = Sut(wordLength: 3);

        var first = new GuessWord([new GuessLetter('A', LetterState.Elsewhere)]);
        session.AddGuess(first);
        session.RevealKeyStates();
        session.VisibleKeyStates['A'].ShouldBe(LetterState.Elsewhere);

        var second = new GuessWord([new GuessLetter('A', LetterState.ExactMatch)]);
        session.AddGuess(second);
        session.RevealKeyStates();

        session.VisibleKeyStates['A'].ShouldBe(LetterState.ExactMatch);
    }

    private static GameSession Sut(int maxGuesses = 6, int wordLength = TEST_WORD_LENGTH) => new()
    {
        Settings = new GameSettings { WordLength = wordLength, MaxGuesses = maxGuesses },
        TargetWord = new string('X', wordLength)
    };
}