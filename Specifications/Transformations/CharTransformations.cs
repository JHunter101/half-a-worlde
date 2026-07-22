namespace Specifications.Transformations;

[Binding]
public static class CharTransformations
{
    [StepArgumentTransformation]
    public static char TransformChar(string value)
    {
        return value.Length != 1
            ? throw new ArgumentException($"Expected a single character, but got '{value}'.")
            : value[0];
    }
}