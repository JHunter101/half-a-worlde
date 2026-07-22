using Core.Models;
using Core.Types;

namespace Core.Services;

public static class GuessEvaluator
{
    private const char CONSUMED_MARKER = '\0';

    public static GuessWord EvaluateGuess(ReadOnlySpan<char> guess, ReadOnlySpan<char> target)
    {
        var resultState = new LetterState[guess.Length];
        Array.Fill(resultState, LetterState.NoMoreMatches);

        char[] remaining = target.ToArray();

        MarkExactMatches(guess, remaining, resultState);
        MarkElsewhereMatches(guess, remaining, resultState);

        var letters = new List<GuessLetter>(guess.Length);
        for (int i = 0; i < guess.Length; i++)
        {
            letters.Add(new GuessLetter(guess[i], resultState[i]));
        }

        return new GuessWord(letters);
    }

    private static void MarkExactMatches(ReadOnlySpan<char> guess, char[] remaining, LetterState[] resultState)
    {
        for (int i = 0; i < guess.Length; i++)
        {
            if (guess[i] == remaining[i])
            {
                resultState[i] = LetterState.ExactMatch;
                remaining[i] = CONSUMED_MARKER;
            }
        }
    }

    private static void MarkElsewhereMatches(ReadOnlySpan<char> guess, char[] remaining, LetterState[] resultState)
    {
        for (int i = 0; i < guess.Length; i++)
        {
            if (resultState[i] == LetterState.ExactMatch)
            {
                continue;
            }

            int matchIndex = Array.IndexOf(remaining, guess[i]);
            if (matchIndex >= 0)
            {
                resultState[i] = LetterState.Elsewhere;
                remaining[matchIndex] = CONSUMED_MARKER;
            }
        }
    }
}