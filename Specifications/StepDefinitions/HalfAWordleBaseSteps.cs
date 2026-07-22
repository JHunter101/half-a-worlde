using Beauo.Components.Settings;
using Bunit;
using Web.Client.Extensions;

namespace Specifications.StepDefinitions;

public abstract class HalfAWordleBaseSteps(
    BlazorTestContext context,
    IGameStateService stateService)
{
    protected BlazorTestContext Context { get; } = context;

    protected IGameStateService StateService { get; } = stateService;

    protected IRenderedComponent<Home> RenderedPage => Context.RenderedPage;

    protected WordRepositoryStub Repository => (WordRepositoryStub)Context.BunitContext.Services.GetRequiredService<IWordRepository>();

    [Given("I am on the home screen")]
    public void GivenIAmOnTheHomeScreen()
        => Context.ResetEvents();

    [Given("I start a game with the target word is {string}")]
    public void GivenIStartAGameWithTheTargetWordIs(string word)
    {
        Repository.SetTarget(word);
        GivenIAmOnTheHomeScreen();
        WhenISetTheWordLengthTo(word.Length);
        WhenIClickTheButton("start game");
    }

    [When("I set the word length to {int}")]
    public void WhenISetTheWordLengthTo(int length)
    {
        IElement select =
            RenderedPage
                .FindComponent<MainMenu>()
                .FindComponents<Select<int>>()
                .Single(s => s.Instance.Label == "word length")
                .Find("select");

        select.Change(length.ToString());
    }

    [When("I set the max attempts to {int}")]
    public void WhenISetTheMaxAttemptsTo(int attempts)
    {
        IElement select =
            RenderedPage
                .FindComponent<MainMenu>()
                .FindComponents<Select<int>>()
                .Single(s => s.Instance.Label == "max attempts")
                .Find("select");

        select.Change(attempts.ToString());
    }

    [Given("the following words are valid:")]
    public void GivenTheFollowingWordsAreValid(IEnumerable<string> words)
        => Repository.SetValid(words);

    [Given("the following words are valid hints:")]
    public void GivenTheFollowingWordsAreValidHints(IEnumerable<string> hints)
        => Repository.SetHints(hints);

    [Given("the following words are invalid:")]
    public void GivenTheFollowingWordsAreBanned(IEnumerable<string> banned)
        => Repository.SetBanned(banned);

    [When("I type the word {string}")]
    public void WhenITypeTheWord(string input)
        => WhenITypeTheWord(GetGameboard(), input);

    [When("I press the key {string}")]
    public void WhenIPressTheKey(string key)
        => WhenIPressTheKey(GetGameboard(), key);

    [When("I repeatedly submit the input {string}")]
    public void WhenIRepeatedlySubmitTheInput(string input)
    {
        IElement container = GetGameboard();
        int maxGuesses = Context.Settings.ShouldNotBeNull().MaxGuesses;

        for (int i = 0; i < maxGuesses; i++)
        {
            WhenITypeTheWord(container, input);
            WhenIPressTheKey(container, "Enter");

            if (Context.RenderedPage.FindComponents<GameOver>().Count > 0)
            {
                break;
            }
        }

        Context.RenderedPage.WaitForComponent<GameOver>();
    }

    [When("I click the {string} button")]
    public void WhenIClickTheButton(string text)
    {
        IRenderedComponent<Button> button =
            RenderedPage
                .FindComponents<Button>()
                .Single(c => c.Instance.Text == text);

        button.Find("button").Click();
    }

    protected static void ThenTheLetterShouldHaveStateOnTheGameBoard(IRenderedComponent<GameBoard> gameBoard, char letter, LetterState state)
    {
        gameBoard
            .FindComponents<LetterTile>()
            .Where(lt =>
                lt.Instance.GuessLetter?.Letter == letter &&
                lt.Instance.GuessLetter.State == state)
            .ShouldNotBeEmpty();
    }

    protected static void ThenTheLetterShouldHaveStateOnTheOnScreenKeyboard(IRenderedComponent<Keyboard> keyboard, char letter, LetterState state)
    {
        IElement key = keyboard
            .FindAll("[data-key]")
            .Single(element =>
                string.Equals(
                    element.GetAttribute("data-key"),
                    letter.ToString(),
                    StringComparison.OrdinalIgnoreCase));

        key.ClassList.ShouldContain(state.ToCssClass().Name);
    }

    private void WhenITypeTheWord(IElement container, string input)
    {
        foreach (char c in input)
        {
            WhenIPressTheKey(container, c.ToString());
        }
    }

    private void WhenIPressTheKey(IElement container, string key)
    {
        string actualKey = key switch
        {
            "DEL" => "Backspace",
            "ENTER" => "Enter",
            _ => key
        };

        Context.ResetEvents();
        container.KeyDown(actualKey);

        if (key == "ENTER" && Context.SuccessTriggered)
        {
            Context.ResetEvents();
        }
    }

    private IElement GetGameboard()
    {
        IRenderedComponent<GamePlayView> gamePlayView =
            RenderedPage.FindComponent<GamePlayView>();

        return gamePlayView.Find("div[tabindex='0']");
    }
}