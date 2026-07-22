namespace Specifications.Transformations;

[Binding]
public static class LetterStateRowTransformations
{
    [StepArgumentTransformation]
    public static IEnumerable<LetterStateRow> TransformLetterStateRows(Table table)
        => table.Rows.Select(row => new LetterStateRow(row["letter"], Enum.Parse<LetterState>(row["state"], ignoreCase: true)));
}

public sealed record LetterStateRow(string Letter, LetterState State);