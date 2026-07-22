using Core.Types;

namespace Core.Models;

public record GuessWord(IReadOnlyList<GuessLetter> Letters);

public record GuessLetter(char Letter, LetterState State);