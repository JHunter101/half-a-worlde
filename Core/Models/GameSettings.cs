namespace Core.Models;

public class GameSettings
{
    public int WordLength { get; set; } = 5;
    public int MaxGuesses { get; set; } = 6;
    public int TargetColoredLettersFromHints => WordLength / 3;
    public int MaxColoredLettersFromHints => WordLength / 2;
}