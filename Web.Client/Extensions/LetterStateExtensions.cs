using Beauo.Types.Style.Base;

namespace Web.Client.Extensions;

public static class LetterStateExtensions
{
    public static CssClass ToCssClass(this LetterState state)
    {
        return state switch
        {
            LetterState.Pending => new LetterStateCssClass("pending"),
            LetterState.NoMoreMatches => new LetterStateCssClass("no-more-matches"),
            LetterState.Elsewhere => new LetterStateCssClass("elsewhere"),
            LetterState.ExactMatch => new LetterStateCssClass("exact"),
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
        };
    }

    private sealed class LetterStateCssClass(string name) : CssClass(name);
}