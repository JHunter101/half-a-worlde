[Binding]
public sealed class CharTransformations
{
    [StepArgumentTransformation]
    public char TransformChar(string value)
    {
        return value.Length != 1
            ? throw new ArgumentException($"Expected a single character, but got '{value}'.")
            : value[0];
    }
}