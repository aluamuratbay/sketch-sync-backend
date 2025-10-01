namespace SketchSync.Entities;

public class JwtToken
{
    public required Guid Id { get; set; }
    public required string Token { get; set; } = string.Empty;
    public required DateTimeOffset ExpirationDate { get; set; }
}