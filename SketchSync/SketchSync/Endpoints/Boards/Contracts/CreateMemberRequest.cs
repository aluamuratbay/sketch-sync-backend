using SketchSync.Enums;

namespace SketchSync.Endpoints.Boards.Contracts;

public class CreateMemberRequest
{
    public required Guid BoardId { get; init; }
    public required string Email { get; init; }
    public required RoleEnum Role { get; init; }
}