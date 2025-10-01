namespace SketchSync.Entities;

public class UserJwtToken
{
    public Guid Id { get; set; }
    public Guid JwtTokenId { get; set; }
    public Guid UserId { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset ActivatedAt { get; set; }
    public DateTimeOffset DeactivatedAt { get; set; }
}