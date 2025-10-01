namespace SketchSync.Database;

public sealed class DatabaseOptions
{
    public required string Postgres { get; init; } = null!;
}