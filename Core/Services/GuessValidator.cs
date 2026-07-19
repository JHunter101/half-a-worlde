namespace Core.Services;

public interface IGuessValidator
{
    Task<bool> IsValid(string guess, int wordLength);
}

public class GuessValidator(IWordRepository wordRepository) : IGuessValidator
{
    public async Task<bool> IsValid(string guess, int wordLength)
    {
        if (guess.Length != wordLength)
        {
            return false;
        }

        var words = await wordRepository.GetWordsAsync(wordLength);

        return words.Contains(guess);
    }
}