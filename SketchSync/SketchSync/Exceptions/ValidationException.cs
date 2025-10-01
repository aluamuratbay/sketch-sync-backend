namespace SketchSync.Exceptions;

public class ValidationException(IReadOnlyList<string> errors) : Exception("Validation failed")
{
    public readonly IReadOnlyList<string> Errors = errors;
}