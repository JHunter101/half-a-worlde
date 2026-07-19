namespace Core.Services;

public interface IWordRepository
{
    Task<HashSet<string>> GetWordsAsync(int length);
}

public class WordRepository(HttpClient http) : IWordRepository
{
    private readonly Dictionary<int, Lazy<Task<HashSet<string>>>> _cache = [];

    public Task<HashSet<string>> GetWordsAsync(int length)
    {
        if (!_cache.TryGetValue(length, out var lazy))
        {
            lazy = new Lazy<Task<HashSet<string>>>(() => LoadWordsAsync($"data/{length}.txt"));
            _cache[length] = lazy;
        }

        return lazy.Value;
    }

    private async Task<HashSet<string>> LoadWordsAsync(string filePath)
    {
        string text = await http.GetStringAsync(filePath);
        return [.. text.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(w => w.Trim().ToUpperInvariant())];
    }
}