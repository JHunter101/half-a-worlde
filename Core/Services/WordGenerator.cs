using Core.Models;
using Core.Types;

namespace Core.Services;

public interface IWordGenerator
{
    Task<IReadOnlyList<string>> GenerateHintsAsync(GameSettings settings, string target);
}

public class WordGenerator(IWordRepository wordRepository) : IWordGenerator
{
    public async Task<IReadOnlyList<string>> GenerateHintsAsync(GameSettings settings, string target)
    {
        string[] candidates = [.. await wordRepository.GetAllowedHints(settings.WordLength)];

        Shuffle(candidates);

        List<string> hints = [];
        HashSet<char> revealedLetters = [];

        foreach (string candidate in candidates)
        {
            if (revealedLetters.Count >= settings.TargetColoredLettersFromHints)
            {
                break;
            }

            if (hints.Count >= settings.MaxGuesses / 3)
            {
                break;
            }

            GuessWord guess = GuessEvaluator.EvaluateGuess(candidate, target);

            int exactMatches = guess.Letters.Count(l => l.State == LetterState.ExactMatch);

            if (exactMatches > settings.MaxColoredLettersFromHints)
            {
                continue;
            }

            guess.Letters
                .Where(l => l.State is LetterState.ExactMatch or LetterState.Elsewhere)
                .Select(l => l.Letter)
                .ToList()
                .ForEach(l => revealedLetters.Add(l));

            hints.Add(candidate);
        }

        return hints;
    }

    private static void Shuffle(string[] list)
    {
        for (int i = list.Length - 1; i > 0; i--)
        {
            int j = Random.Shared.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}