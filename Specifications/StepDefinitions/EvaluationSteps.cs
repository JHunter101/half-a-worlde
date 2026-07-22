using Bunit;

namespace Specifications.StepDefinitions;

[Binding]
[Scope(Feature = "Word Evaluation")]
public sealed class WordEvaluationUiSteps(
    BlazorTestContext context,
    IGameStateService stateService)
    : HalfAWordleBaseSteps(context, stateService)
{
    [Then("the letter {string} should have state '{LetterState}' on the game board")]
    public void ThenTheLetterShouldHaveStateOnTheGameBoard(string letter, LetterState state)
        => ThenTheLetterShouldHaveStateOnTheGameBoard(RenderedPage.FindComponent<GameBoard>(), letter.Single(), state);

    [Then("the following letters should have state '{LetterState}' on the game board:")]
    public void ThenTheFollowingLettersShouldHaveStateOnTheGameBoard(LetterState state, IEnumerable<string> letters)
    {
        IRenderedComponent<GameBoard> gameBoard = RenderedPage.FindComponent<GameBoard>();

        foreach (string letter in letters)
        {
            ThenTheLetterShouldHaveStateOnTheGameBoard(gameBoard, letter.Single(), state);
        }
    }

    [Then("the following letters should have the specified states on the game board:")]
    public void ThenTheFollowingLettersShouldHaveTheSpecifiedStatesOnTheGameBoard(IEnumerable<LetterStateRow> rows)
    {
        IRenderedComponent<GameBoard> gameBoard = RenderedPage.FindComponent<GameBoard>();

        foreach (LetterStateRow row in rows)
        {
            ThenTheLetterShouldHaveStateOnTheGameBoard(gameBoard, row.Letter.Single(), row.State);
        }
    }

    [Then("the letter {string} should have state '{LetterState}' on the on screen keyboard")]
    public void ThenTheLetterShouldHaveStateOnTheOnScreenKeyboard(string letter,
        LetterState state)
    {
        IRenderedComponent<Keyboard> keyboard = RenderedPage.FindComponent<Keyboard>();

        ThenTheLetterShouldHaveStateOnTheOnScreenKeyboard(keyboard, letter.Single(), state);
    }

    [Then("the following letters should have the specified states on the on screen keyboard:")]
    public void ThenTheFollowingLettersShouldHaveTheSpecifiedStatesOnTheOnScreenKeyboard(IEnumerable<LetterStateRow> rows)
    {
        IRenderedComponent<Keyboard> keyboard = RenderedPage.FindComponent<Keyboard>();

        foreach (LetterStateRow row in rows)
        {
            ThenTheLetterShouldHaveStateOnTheOnScreenKeyboard(keyboard, row.Letter.Single(), row.State);
        }
    }

    [Then("the game should trigger an invalid input state")]
    public void ThenTheGameShouldTriggerAnInvalidInputState()
        => Context.InvalidTriggered.ShouldBeTrue();

    [Then("I should see the game over screen")]
    public void ThenIShouldSeeTheGameOverScreen()
        => RenderedPage.FindComponent<GameOver>().ShouldNotBeNull();
}