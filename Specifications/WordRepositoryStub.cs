namespace Specifications;

public sealed class WordRepositoryStub : IWordRepository
{
    private Dictionary<int, HashSet<string>> _targets = [];
    private Dictionary<int, HashSet<string>> _hints = [];
    private Dictionary<int, HashSet<string>> _valid = [];
    private HashSet<string> _bannedWords = [];

    public WordRepositoryStub()
    {
        for (int length = 1; length <= 20; length++)
        {
            _targets[length] = [new string('T', length)];
            _hints[length] = [new string('H', length)];
            _valid[length] = [new string('G', length)];
        }
    }

    public Task<HashSet<string>> GetAllowedGuesses(int length)
        => Task.FromResult(GetSet(_valid, length));

    public Task<HashSet<string>> GetAllowedHints(int length)
        => Task.FromResult(GetSet(_hints, length));

    public Task<HashSet<string>> GetAllowedTargets(int length)
        => Task.FromResult(GetSet(_targets, length));

    public void SetTarget(string word)
    {
        word = word.ToUpperInvariant();

        ValidateNotBanned(word);

        _targets = new()
        {
            [word.Length] = [word]
        };

        AddValid(word);
    }

    public void SetBanned(IEnumerable<string> words)
    {
        var normalizedWords = words
            .Select(word => word.ToUpperInvariant())
            .ToHashSet();

        foreach (string word in normalizedWords)
        {
            if (_targets.GetValueOrDefault(word.Length)?.Contains(word) == true ||
                _hints.GetValueOrDefault(word.Length)?.Contains(word) == true ||
                _valid.GetValueOrDefault(word.Length)?.Contains(word) == true)
            {
                throw new InvalidOperationException(
                    $"Word '{word}' cannot be banned because it already exists in another word list.");
            }
        }

        _bannedWords = normalizedWords;
    }

    public void SetHints(IEnumerable<string> words)
    {
        var normalizedWords = words
            .Select(word => word.ToUpperInvariant())
            .GroupBy(word => word.Length)
            .ToDictionary(
                group => group.Key,
                group => group.ToHashSet());

        foreach (string word in normalizedWords.Values.SelectMany(words => words))
        {
            ValidateNotBanned(word);
        }

        _hints = normalizedWords;
    }

    public void SetValid(IEnumerable<string> words)
    {
        var normalizedWords = words
            .Select(word => word.ToUpperInvariant())
            .GroupBy(word => word.Length)
            .ToDictionary(
                group => group.Key,
                group => group.ToHashSet());

        foreach (string word in normalizedWords.Values.SelectMany(words => words))
        {
            ValidateNotBanned(word);
        }

        _valid = normalizedWords;
    }

    public void AddValid(string word)
    {
        word = word.ToUpperInvariant();

        ValidateNotBanned(word);

        GetSet(_valid, word.Length).Add(word);
    }

    private static HashSet<string> GetSet(Dictionary<int, HashSet<string>> dictionary, int length)
    {
        if (!dictionary.TryGetValue(length, out HashSet<string>? set))
        {
            set = [];
            dictionary[length] = set;
        }

        return set;
    }

    private void ValidateNotBanned(string word)
    {
        if (_bannedWords.Contains(word))
        {
            throw new ArgumentException($"Word '{word}' is banned.", nameof(word));
        }
    }
}