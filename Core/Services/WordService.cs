namespace Core.Services;

public interface IWordService
{
    Task<string> GetWordAsync(int wordLength);
}

public class WordService(IWordRepository wordRepository) : IWordService
{
    public async Task<string> GetWordAsync(int wordLength)
    {
        var words = await wordRepository.GetWordsAsync(wordLength);
        return words.ElementAt(Random.Shared.Next(words.Count));
    }
}