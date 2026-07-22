using Bunit;

namespace Specifications.StepDefinitions;

[Binding]
[Scope(Feature = "Initialization")]
public sealed class InitializationUiSteps(BlazorTestContext context, IGameStateService stateService)
    : HalfAWordleBaseSteps(context, stateService)
{
    [Given("No hints are available")]
    public void GivenNoHintsAreAvailable()
        => Repository.SetHints([]);

    [Then("I should see the main menu")]
    public void ThenIShouldSeeTheMainMenu()
        => RenderedPage.FindComponent<MainMenu>().ShouldNotBeNull();

    [Then("the game board should be visible")]
    public void ThenTheGameBoardShouldBeVisible()
        => RenderedPage.WaitForComponent<GameBoard>();

    [Then("I should see {int} letter tiles on the screen")]
    public void ThenIShouldSeeLetterTilesOnTheScreen(int count)
        => RenderedPage
            .FindComponent<GameBoard>()
            .FindComponents<LetterTile>()
            .Count
            .ShouldBe(count);

    [Then("all letters should have state '{LetterState}' on the on screen keyboard")]
    public void ThenAllLettersShouldHaveStateOnTheOnScreenKeyboard(LetterState state)
    {
        IRenderedComponent<Keyboard> keyboard = RenderedPage.FindComponent<Keyboard>();

        foreach (char letter in "ABCDEFGHIJKLMNOPQRSTUVWXYZ")
        {
            ThenTheLetterShouldHaveStateOnTheOnScreenKeyboard(keyboard, letter, state);
        }
    }

    [Then("the following words should be visible on the game board:")]
    public void ThenTheFollowingWordsShouldBeVisibleOnTheGameBoard(IEnumerable<string> words)
    {
        Console.WriteLine(Repository);
        foreach (string word in words)
        {
            char?[] letters = [.. RenderedPage
                .FindComponent<GameBoard>()
                .FindComponents<LetterTile>()
                .Select(lt => lt.Instance.GuessLetter?.Letter)];

            letters
                .Chunk(word.Length)
                .Any(row => row
                    .OfType<char>()
                    .SequenceEqual(word.Select(char.ToUpperInvariant)))
                .ShouldBeTrue();
        }
    }
}