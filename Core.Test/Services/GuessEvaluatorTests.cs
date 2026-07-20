using Core.Services;
using Core.Types;
using Shouldly;

namespace Core.Test.Services;

[TestClass]
public sealed class GuessEvaluatorTests
{
    [TestMethod]
    public void EvaluateGuess_WhenGuessEqualsTarget_ThenAllLettersAreExact()
    {
        ReadOnlySpan<char> GUESS = "APPLE";
        ReadOnlySpan<char> TARGET = "APPLE";
        var expected = new[]
        {
            LetterState.ExactMatch,
            LetterState.ExactMatch,
            LetterState.ExactMatch,
            LetterState.ExactMatch,
            LetterState.ExactMatch
        };

        var actual = GuessEvaluator.EvaluateGuess(GUESS, TARGET);

        actual.Letters.Count.ShouldBe(expected.Length, "Expected state length to match GUESS length.");
        for (int i = 0; i < expected.Length; i++)
        {
            actual.Letters[i].State.ShouldBe(expected[i]);
        }
    }

    [TestMethod]
    public void EvaluateGuess_WhenNoLettersMatch_ThenAllLettersNoMoreMatches()
    {
        const string GUESS = "ZZZZZ";
        const string TARGET = "APPLE";
        var expected = new[]
        {
            LetterState.NoMoreMatches,
            LetterState.NoMoreMatches,
            LetterState.NoMoreMatches,
            LetterState.NoMoreMatches,
            LetterState.NoMoreMatches
        };

        var actual = GuessEvaluator.EvaluateGuess(GUESS, TARGET);

        actual.Letters.Count.ShouldBe(expected.Length);
        for (int i = 0; i < expected.Length; i++)
        {
            actual.Letters[i].State.ShouldBe(expected[i]);
        }
    }

    [TestMethod]
    public void EvaluateGuess_WhenLettersArePresentElsewhere_ThenAllLettersElsewhere()
    {
        const string GUESS = "CBAAB";
        const string TARGET = "AABBC";
        var expected = new[]
        {
            LetterState.Elsewhere,
            LetterState.Elsewhere,
            LetterState.Elsewhere,
            LetterState.Elsewhere,
            LetterState.Elsewhere
        };

        var actual = GuessEvaluator.EvaluateGuess(GUESS, TARGET);

        actual.Letters.Count.ShouldBe(expected.Length);
        for (int i = 0; i < expected.Length; i++)
        {
            actual.Letters[i].State.ShouldBe(expected[i]);
        }
    }

    [TestMethod]
    public void EvaluateGuess_WhenLettersElsewhereButAlreadyConsumed_ThenReturnsThoseNoMoreMatches()
    {
        const string GUESS = "LLLXXX";
        const string TARGET = "XXXLLX";
        var expected = new[]
        {
            LetterState.Elsewhere,
            LetterState.Elsewhere,
            LetterState.NoMoreMatches,
            LetterState.Elsewhere,
            LetterState.Elsewhere,
            LetterState.ExactMatch
        };

        var actual = GuessEvaluator.EvaluateGuess(GUESS, TARGET);

        actual.Letters.Count.ShouldBe(expected.Length);
        for (int i = 0; i < expected.Length; i++)
        {
            actual.Letters[i].State.ShouldBe(expected[i]);
        }
    }
}