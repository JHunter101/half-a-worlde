namespace Specifications.Transformations;

[Binding]
public static class StringTransformations
{
    [StepArgumentTransformation]
    public static IEnumerable<string> TransformStrings(Table table)
        => table.Rows.Select(row => row.Values.Single());
}