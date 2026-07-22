namespace Specifications.Transformations;

[Binding]
public class StringTransformations
{
    [StepArgumentTransformation]
    public IEnumerable<string> TransformStrings(Table table)
        => table.Rows.Select(row => row.Values.Single());
}