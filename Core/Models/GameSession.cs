using Core.Types;

namespace Core.Models;

public class GameSession
{
    public required GameSettings Settings { get; init; }
    public required string TargetWord { get; init; }
    public IReadOnlyList<GuessWord> Rows => _guesses;
    public int CurrentRowIndex => Rows.Count;

    public string CurrentInput { get; private set; } = "";

    public IReadOnlyDictionary<char, LetterState> VisibleKeyStates { get; private set; } = InitializeKeyStates();

    public bool HasPendingHints => _pendingHints.Count > 0;
    public bool IsLocked { get; private set; }

    public SessionState State => _guesses.Count switch
    {
        0 => SessionState.Playing,
        _ when _guesses[^1].Letters.All(l => l.State == LetterState.ExactMatch) => SessionState.Won,
        _ when _guesses.Count >= Settings.MaxGuesses => SessionState.Lost,
        _ => SessionState.Playing
    };

    private readonly List<GuessWord> _guesses = [];
    private readonly Queue<string> _pendingHints = new();
    private readonly Dictionary<char, LetterState> _keyStates = InitializeKeyStates();

    public void Lock() => IsLocked = true;

    public void Unlock() => IsLocked = false;

    public void RevealKeyStates() => VisibleKeyStates = new Dictionary<char, LetterState>(_keyStates);

    public void AddLetter(char letter)
    {
        if (CurrentInput.Length < Settings.WordLength && char.IsLetter(letter))
        {
            CurrentInput += char.ToUpperInvariant(letter);
        }
    }

    public void RemoveLetter()
    {
        if (CurrentInput.Length > 0)
        {
            CurrentInput = CurrentInput[..^1];
        }
    }

    public void ClearInput() => CurrentInput = "";

    public void AddGuess(GuessWord guess)
    {
        _guesses.Add(guess);
        UpdateKeyStates(guess);
    }

    public void QueueHints(IEnumerable<string> hints)
    {
        foreach (string hint in hints)
        {
            _pendingHints.Enqueue(hint);
        }
    }

    public string? TryGetNextHint() => _pendingHints.Count > 0 ? _pendingHints.Dequeue() : null;

    private void UpdateKeyStates(GuessWord guess)
    {
        foreach (var letter in guess.Letters)
        {
            if (letter.State > _keyStates[letter.Letter])
            {
                _keyStates[letter.Letter] = letter.State;
            }
        }
    }

    private static Dictionary<char, LetterState> InitializeKeyStates()
    {
        var states = new Dictionary<char, LetterState>(26);
        for (char c = 'A'; c <= 'Z'; c++)
        {
            states[c] = LetterState.Pending;
        }
        return states;
    }
}

public enum SessionState
{
    Playing,
    Won,
    Lost
}