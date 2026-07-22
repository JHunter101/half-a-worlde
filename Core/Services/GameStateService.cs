using CommunityToolkit.Diagnostics;
using Core.Models;

namespace Core.Services;

public interface IGameStateService
{
    GameSession? Session { get; }

    event Action? OnChange;

    event Action? OnInvalid;

    event Action? OnSuccess;

    Task StartNewGameAsync(GameSettings? settings = null);

    Task HandleInputAsync(string key);

    void NotifyAnimationComplete();

    void ClearSession();
}

public class GameStateService(
    IWordRepository wordRepository,
    IGuessValidator guessValidator,
    IWordGenerator wordGenerator) : IGameStateService
{
    public GameSession? Session { get; private set; }

    public event Action? OnChange;

    public event Action? OnInvalid;

    public event Action? OnSuccess;

    public async Task StartNewGameAsync(GameSettings? settings = null)
    {
        settings ??= Session?.Settings;
        Guard.IsNotNull(settings);
        string targetWord = await GetTargetWord(settings);

        Session = new GameSession
        {
            Settings = settings,
            TargetWord = targetWord
        };

        IReadOnlyList<string> hints = await wordGenerator.GenerateHintsAsync(settings, targetWord);
        Session.QueueHints(hints);

        OnChange?.Invoke();

        if (Session.HasPendingHints)
        {
            await Task.Yield();
            ProcessNextHint();
        }
    }

    public void ClearSession()
    {
        Session = null;
        OnChange?.Invoke();
    }

    public async Task HandleInputAsync(string key)
    {
        if (Session is null || Session.State != SessionState.Playing || Session.IsLocked || Session.HasPendingHints)
        {
            return;
        }

        switch (key.ToUpperInvariant())
        {
            case "ENTER":
                await TrySubmitWordAsync();
                break;

            case "BACKSPACE":
            case "DEL":
                Session.RemoveLetter();
                OnChange?.Invoke();
                break;

            default:
                if (key.Length == 1)
                {
                    Session.AddLetter(key[0]);
                    OnChange?.Invoke();
                }

                break;
        }
    }

    public void NotifyAnimationComplete()
    {
        Guard.IsNotNull(Session);

        Session.RevealKeyStates();
        OnChange?.Invoke();

        if (Session.HasPendingHints)
        {
            ProcessNextHint();
        }
        else
        {
            Session.Unlock();
        }
    }

    private async Task<string> GetTargetWord(GameSettings settings)
    {
        HashSet<string> words = await wordRepository.GetAllowedTargets(settings.WordLength);
        string targetWord = words.ElementAt(Random.Shared.Next(words.Count));
        return targetWord;
    }

    private void ProcessNextHint()
    {
        Guard.IsNotNull(Session);

        string? hint = Session.TryGetNextHint();
        if (hint is not null)
        {
            SubmitWord(hint);
        }
    }

    private async Task TrySubmitWordAsync()
    {
        Guard.IsNotNull(Session);

        string word = Session.CurrentInput.ToUpperInvariant();

        if (!await guessValidator.IsValid(word, Session.Settings.WordLength))
        {
            OnInvalid?.Invoke();
            return;
        }

        SubmitWord(word);
    }

    private void SubmitWord(string word)
    {
        Guard.IsNotNull(Session);

        GuessWord guess = GuessEvaluator.EvaluateGuess(word, Session.TargetWord);
        Session.AddGuess(guess);
        Session.ClearInput();

        Session.Lock();

        OnSuccess?.Invoke();
        OnChange?.Invoke();
    }
}